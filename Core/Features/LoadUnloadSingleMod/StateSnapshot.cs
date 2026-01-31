using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ModReloader.Core.Features.LoadUnloadSingleMod;

interface IStateSnapshot
{
	object Restore();
}

interface IInfoToSnapshot
{
	IStateSnapshot CreateSnapshot(object value);
	List<FieldElementToSnapshot> NestedFields { get; }
	CollectionElementToSnapshot CollectionElementInfo { get; }
	List<TupleElementToSnapshot> TupleElementsInfo { get; }
}

class CollectionElementToSnapshot : IInfoToSnapshot
{
	public List<FieldElementToSnapshot> NestedFields { get; }
	public CollectionElementToSnapshot CollectionElementInfo { get; }

	public List<TupleElementToSnapshot> TupleElementsInfo { get; }
	public CollectionElementToSnapshot(
		List<FieldElementToSnapshot> nestedElements = null,
		CollectionElementToSnapshot collectionElementInfo = null,
		List<TupleElementToSnapshot> tupleElementsInfo = null)
	{
		NestedFields = nestedElements;
		CollectionElementInfo = collectionElementInfo;
		TupleElementsInfo = tupleElementsInfo;
	}
	public IStateSnapshot CreateSnapshot(object value)
	{
		if (value == null)
			return new ValueSnapshot(null, null, null, null);
		return new ValueSnapshot(value, NestedFields, CollectionElementInfo, TupleElementsInfo);
	}
}

class FieldElementToSnapshot : CollectionElementToSnapshot
{
	public string Name { get; }

	public FieldElementToSnapshot(string fieldName,
		List<FieldElementToSnapshot> nestedElements = null,
		CollectionElementToSnapshot collectionElementInfo = null,
		List<TupleElementToSnapshot> tupleElementsInfo = null) : base(nestedElements, collectionElementInfo, tupleElementsInfo)
	{
		Name = fieldName;
	}
}

class TupleElementToSnapshot : CollectionElementToSnapshot
{
	public int Index { get; }
	public TupleElementToSnapshot(int index,
		List<FieldElementToSnapshot> nestedElements = null,
		CollectionElementToSnapshot collectionElementInfo = null,
		List<TupleElementToSnapshot> tupleElementsInfo = null) : base(nestedElements, collectionElementInfo, tupleElementsInfo)
	{
		Index = index;
	}
}

/// <summary>
/// Snapshot for FIELDS ONLY (public + private + protected)
/// No properties - direct field access bypasses getters/setters
/// </summary>
class ValueSnapshot : IStateSnapshot
{
	public object? StoredValue { get; private set; }
	protected Dictionary<FieldInfo, IStateSnapshot> FieldSnapshots { get; } = new();
	public CollectionSnapshot collectionSnapshot = null;
	public TupleSnapshot tupleSnapshot = null;

	public ValueSnapshot(object? value, List<FieldElementToSnapshot> nestedFields, CollectionElementToSnapshot collectionElementInfo, List<TupleElementToSnapshot> tupleElementsInfo)
	{
		StoredValue = value;

		if (StoredValue == null)
			return;

		// Process nested fields
		if (nestedFields != null && nestedFields.Count > 0) {
			var type = StoredValue.GetType();

			// Get ALL fields
			var fieldInfoMap = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.ToDictionary(f => f.Name, f => f);

			FieldSnapshots = nestedFields
				.Where(nf => {
					if (!fieldInfoMap.ContainsKey(nf.Name)) {
						throw new ArgumentException($"Field '{nf.Name}' not found in type '{type.FullName}'.");
					}
					return true;
				})
				.Select(nf => {
					FieldInfo? field = fieldInfoMap[nf.Name];
					object? fieldValue = field.GetValue(StoredValue);
					var snapshot = nf.CreateSnapshot(fieldValue);
					return KeyValuePair.Create(field, snapshot);
				})
				.ToDictionary();
		}

		// Process collection
		if (collectionElementInfo != null) {
			if (StoredValue is not IEnumerable enumerable) {
				throw new InvalidOperationException($"CollectionElementInfo specified but value is not a collection: {StoredValue.GetType()}.");
			}
			collectionSnapshot = new CollectionSnapshot(enumerable, collectionElementInfo);
		}

		// Process tuple elements
		if (tupleElementsInfo != null) {
			if (StoredValue is not ITuple tuple) {
				throw new InvalidOperationException($"TupleElementsInfo specified but value is not a tuple: {StoredValue.GetType()}.");
			}
			tupleSnapshot = new TupleSnapshot(tuple, tupleElementsInfo);
		}
	}

	public object Restore()
	{
		if (StoredValue == null)
			return null;

		// Restore fields
		foreach ((FieldInfo field, IStateSnapshot snapshot) in FieldSnapshots) {
			field.SetValue(StoredValue, snapshot.Restore());
		}

		// Restore collection
		if (collectionSnapshot != null) {
			var restored = collectionSnapshot.Restore();
			if (!ReferenceEquals(restored, StoredValue)) {
				throw new InvalidOperationException(
					$"Collection restoration did not return the original collection instance. " +
					$"Expected: {StoredValue?.GetType()}, Got: {restored?.GetType()}");
			}
		}

		// Restore tuple elements
		if (tupleSnapshot != null) {
			var restored = tupleSnapshot.Restore();
			if (!ReferenceEquals(restored, StoredValue)) {
				throw new InvalidOperationException(
					$"Tuple restoration did not return the original tuple instance. " +
					$"Expected: {StoredValue?.GetType()}, Got: {restored?.GetType()}");
			}
		}

		return StoredValue;
	}
}

class CollectionSnapshot : IStateSnapshot
{
	public IEnumerable Collection { get; private set; }
	public List<IStateSnapshot> ElementSnapshots { get; } = new();

	public CollectionSnapshot(IEnumerable collection, CollectionElementToSnapshot elementInfo)
	{
		if (collection == null) {
			throw new ArgumentNullException(nameof(collection));
		}

		Collection = collection;

		foreach (var element in Collection) {
			var snapshot = elementInfo.CreateSnapshot(element);
			ElementSnapshots.Add(snapshot);
		}
	}

	public object Restore()
	{
		if (Collection == null)
			return null;

		if (Collection is Array array) {
			Array.Clear(array);
			for (int i = 0; i < Math.Min(array.Length, ElementSnapshots.Count); i++) {
				array.SetValue(ElementSnapshots[i].Restore(), i);
			}
			return array;
		}

		if (Collection is IList list) {
			list.Clear();
			foreach (var snapshot in ElementSnapshots)
				list.Add(snapshot.Restore());
			return list;
		}

		if (Collection is IDictionary dict) {
			dict.Clear();
			foreach (var snapshot in ElementSnapshots) {
				var restoredElement = snapshot.Restore();

				// Use reflection to unpack KeyValuePair<TKey, TValue>
				var kvpType = restoredElement.GetType();
				var keyField = kvpType.GetField("key", BindingFlags.NonPublic | BindingFlags.Instance);
				var valueField = kvpType.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);

				if (keyField == null || valueField == null) {
					throw new InvalidOperationException($"Cannot find key/value fields in {kvpType.FullName}");
				}

				var key = keyField.GetValue(restoredElement);
				var value = valueField.GetValue(restoredElement);

				dict.Add(key, value);
			}
			return dict;
		}

		// Other collections (HashSet, etc.)
		var type = Collection.GetType();
		var clearMethod = type.GetMethod("Clear");
		var addMethod = type.GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, [typeof(object)]);

		if (clearMethod == null || addMethod == null) {
			throw new InvalidOperationException($"Collection type {type.FullName} does not have Clear or Add methods.");
		}

		clearMethod.Invoke(Collection, null);
		foreach (var snapshot in ElementSnapshots) {
			addMethod.Invoke(Collection, [snapshot.Restore()]);
		}

		return Collection;
	}
}

class TupleSnapshot : IStateSnapshot
{
	public ITuple Tuple { get; private set; }
	public Dictionary<int, IStateSnapshot> ElementSnapshots { get; } = new();
	public TupleSnapshot(ITuple tuple, List<TupleElementToSnapshot> elementInfos)
	{
		if (tuple == null) {
			throw new ArgumentNullException(nameof(tuple));
		}

		Tuple = tuple;
		foreach (var elementInfo in elementInfos) {
			if (elementInfo.Index < 0 || elementInfo.Index >= Tuple.Length) {
				throw new ArgumentOutOfRangeException($"Tuple index {elementInfo.Index} is out of range for tuple of length {Tuple.Length}.");
			}
			var element = Tuple[elementInfo.Index];
			var snapshot = elementInfo.CreateSnapshot(element);
			ElementSnapshots[elementInfo.Index] = snapshot;
		}
	}
	public object Restore()
	{
		if (Tuple == null)
			return null;

		var tupleType = Tuple.GetType();

		foreach ((int index, IStateSnapshot snapshot) in ElementSnapshots) {
			var restoredValue = snapshot.Restore();

			// Шукаємо КОНКРЕТНЕ поле ItemN
			string fieldName = $"Item{index + 1}"; // index 0 → Item1
			var field = tupleType.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

			if (field == null) {
				throw new InvalidOperationException($"Tuple field '{fieldName}' not found in type {tupleType.FullName}");
			}

			field.SetValue(Tuple, restoredValue);
		}

		return Tuple;
	}
}
