using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ModReloader.Core.Features.LoadUnloadSingleMod;

public class ManualSnapshotRegistry
{
	private readonly Dictionary<object, ManualValueSnapshot> _snapshotMap = new(SnapshotEqualityComparer.Instance);
	private readonly List<FieldReferenceSnapshot> _fieldRefSnapshots = new();

	public void Add(object target, ManualValueSnapshot snapshot)
	{
		_snapshotMap[target] = snapshot;
	}

	private bool TryGet(object target, out ManualValueSnapshot snapshot)
	{
		return _snapshotMap.TryGetValue(target, out snapshot);
	}

	/// <summary>
	/// Snapshot a field with custom descriptors for nested fields, collections, and tuples.
	/// </summary>
	public void SnapshotRefField(
		Type declaringType, 
		string fieldName,
		List<FieldDescriptor> nestedFields = null,
		CollectionDescriptor collectionElementInfo = null,
		List<TupleElementDescriptor> tupleElementsInfo = null)
	{
		SnapshotRefField(null, declaringType, fieldName, nestedFields, collectionElementInfo, tupleElementsInfo);
	}

	/// <summary>
	/// Snapshot an instance field with custom descriptors for nested fields, collections, and tuples.
	/// </summary>
	public void SnapshotRefField(
		object target,
		string fieldName,
		List<FieldDescriptor> nestedFields = null,
		CollectionDescriptor collectionElementInfo = null,
		List<TupleElementDescriptor> tupleElementsInfo = null)
	{
		ArgumentNullException.ThrowIfNull(target);
		SnapshotRefField(target, target.GetType(), fieldName, nestedFields, collectionElementInfo, tupleElementsInfo);
	}

	private void SnapshotRefField(
		object target, 
		Type declaringType, 
		string fieldName,
		List<FieldDescriptor> nestedFields,
		CollectionDescriptor collectionElementInfo,
		List<TupleElementDescriptor> tupleElementsInfo)
	{
		var field = declaringType.GetField(fieldName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			?? declaringType.GetField($"<{fieldName}>k__BackingField", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
		
		if (field == null)
			throw new InvalidOperationException($"Field or auto-property backing field '{fieldName}' not found on {declaringType.FullName}");

		var value = field.GetValue(target);
		_fieldRefSnapshots.Add(new FieldReferenceSnapshot(target, field));
		
		// Create snapshot with descriptors if provided
		CreateSnapshot(value, nestedFields, collectionElementInfo, tupleElementsInfo, 1);
	}

	public void CreateSnapshot(object value, List<FieldDescriptor> nestedFields, CollectionDescriptor collectionElementInfo, List<TupleElementDescriptor> tupleElementsInfo, int depth)
	{
		if (value == null)
			return;

		var type = value.GetType();

		if (type.IsPrimitive || type.IsEnum || type == typeof(string))
			return;

		if (TryGet(value, out var snapshot))
			return;

		snapshot = new ManualValueSnapshot(this, value, depth);
		Add(value, snapshot);
		snapshot.Init(nestedFields, collectionElementInfo, tupleElementsInfo);
	}

	public void RestoreAll()
	{
		foreach (var fr in _fieldRefSnapshots)
		{
			fr.Restore();
		}

		foreach (var snapshot in _snapshotMap.Values)
		{
			snapshot.Restore();
		}
	}
}

/// <summary>
/// Snapshot for FIELDS ONLY (public + private + protected)
/// No properties - direct field access bypasses getters/setters
/// </summary>
public class ManualValueSnapshot
{
	private readonly ManualSnapshotRegistry _registry;
	public object? StoredValue { get; private set; }
	protected Dictionary<FieldInfo, object> FieldSnapshots = new();
	public ManualCollectionSnapshot collectionSnapshot = null;
	public ManualTupleSnapshot tupleSnapshot = null;
	public int Depth;

	public ManualValueSnapshot(ManualSnapshotRegistry registry, object? value, int depth)
	{
		_registry = registry;
		StoredValue = value;
		Depth = depth;
	}

	public void Init(List<FieldDescriptor> nestedFields, CollectionDescriptor collectionElementInfo, List<TupleElementDescriptor> tupleElementsInfo)
	{
		if (StoredValue == null)
			return;

		var type = StoredValue.GetType();

		// Process nested fields
		if (nestedFields != null && nestedFields.Count > 0) {
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
					nf.CreateSnapshot(fieldValue, _registry, Depth + 1);
					return KeyValuePair.Create(field, fieldValue);
				})
				.ToDictionary();
		}

		// Process collection
		if (collectionElementInfo != null) {
			if (StoredValue is not IEnumerable enumerable) {
				throw new InvalidOperationException($"CollectionElementInfo specified but value is not a collection: {StoredValue.GetType()}.");
			}
			collectionSnapshot = new ManualCollectionSnapshot(_registry, enumerable, collectionElementInfo, Depth);
		}

		// Process tuple elements
		if (tupleElementsInfo != null) {
			if (StoredValue is not ITuple tuple) {
				throw new InvalidOperationException($"TupleElementsInfo specified but value is not a tuple: {StoredValue.GetType()}.");
			}
			tupleSnapshot = new ManualTupleSnapshot(_registry, tuple, tupleElementsInfo, Depth);
		}
	}

	public void Restore()
	{
		if (StoredValue == null)
			return;

		// Restore fields
		foreach ((FieldInfo field, object value) in FieldSnapshots) {
			field.SetValue(StoredValue, value);
		}

		// Restore collection
		if (collectionSnapshot != null) {
			var restored = collectionSnapshot.RestoreAndReturn();
			if (!ReferenceEquals(restored, StoredValue)) {
				throw new InvalidOperationException(
					$"Collection restoration did not return the original collection instance. " +
					$"Expected: {StoredValue?.GetType()}, Got: {restored?.GetType()}");
			}
		}

		// Restore tuple elements
		if (tupleSnapshot != null) {
			var restored = tupleSnapshot.RestoreAndReturn();
			if (!ReferenceEquals(restored, StoredValue)) {
				throw new InvalidOperationException(
					$"Tuple restoration did not return the original tuple instance. " +
					$"Expected: {StoredValue?.GetType()}, Got: {restored?.GetType()}");
			}
		}
	}
}

public class ManualCollectionSnapshot
{
	public IEnumerable Collection { get; private set; }
	public List<object> Elements { get; } = new();
	int Depth;

	public ManualCollectionSnapshot(ManualSnapshotRegistry registry, IEnumerable collection, CollectionDescriptor elementInfo, int depth)
	{
		Depth = depth;
		ArgumentNullException.ThrowIfNull(collection);

		Collection = collection;

		foreach (var element in Collection) {
			elementInfo.CreateSnapshot(element, registry, Depth + 1);
			Elements.Add(element);
		}
	}

	public object RestoreAndReturn()
	{
		if (Collection == null)
			return null;

		if (Collection is Array array) {
			Array.Clear(array);
			for (int i = 0; i < Math.Min(array.Length, Elements.Count); i++) {
				array.SetValue(Elements[i], i);
			}
			return array;
		}

		if (Collection is IList list) {
			list.Clear();
			foreach (var element in Elements)
				list.Add(element);
			return list;
		}

		if (Collection is IDictionary dict) {
			dict.Clear();
			foreach (var element in Elements) {
				// Use reflection to unpack KeyValuePair<TKey, TValue>
				var kvpType = element.GetType();
				var keyField = kvpType.GetField("key", BindingFlags.NonPublic | BindingFlags.Instance);
				var valueField = kvpType.GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);

				if (keyField == null || valueField == null) {
					throw new InvalidOperationException($"Cannot find key/value fields in {kvpType.FullName}");
				}

				var key = keyField.GetValue(element);
				var value = valueField.GetValue(element);

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
		foreach (var element in Elements) {
			addMethod.Invoke(Collection, [element]);
		}

		return Collection;
	}
}

public class ManualTupleSnapshot
{
	public ITuple Tuple { get; private set; }
	public Dictionary<int, object> ElementSnapshots { get; } = new();
	int Depth;

	public ManualTupleSnapshot(ManualSnapshotRegistry registry, ITuple tuple, List<TupleElementDescriptor> elementInfos, int depth)
	{
		Depth = depth;
		ArgumentNullException.ThrowIfNull(tuple);

		Tuple = tuple;
		foreach (var elementInfo in elementInfos) {
			if (elementInfo.Index < 0 || elementInfo.Index >= Tuple.Length) {
				throw new ArgumentOutOfRangeException($"Tuple index {elementInfo.Index} is out of range for tuple of length {Tuple.Length}.");
			}
			var element = Tuple[elementInfo.Index];
			elementInfo.CreateSnapshot(element, registry, Depth + 1);
			ElementSnapshots[elementInfo.Index] = element;
		}
	}

	public object RestoreAndReturn()
	{
		if (Tuple == null)
			return null;

		var tupleType = Tuple.GetType();

		foreach ((int index, object value) in ElementSnapshots) {
			// Шукаємо КОНКРЕТНЕ поле ItemN
			string fieldName = $"Item{index + 1}"; // index 0 → Item1
			var field = tupleType.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

			if (field == null) {
				throw new InvalidOperationException($"Tuple field '{fieldName}' not found in type {tupleType.FullName}");
			}

			field.SetValue(Tuple, value);
		}

		return Tuple;
	}
}
