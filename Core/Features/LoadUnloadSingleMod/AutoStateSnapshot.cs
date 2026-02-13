using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ModReloader.Common.Helpers;

namespace ModReloader.Core.Features.LoadUnloadSingleMod;

public class FieldReferenceSnapshot
{
    private readonly object _target;
    private readonly FieldInfo _field;
    private readonly object _storedValue;
    public FieldReferenceSnapshot(object target, FieldInfo field)
    {
        _target = target;
        _field = field;
        _storedValue = field.GetValue(target);
    }

    public void Restore()
    {
        if (_field.IsInitOnly)
        {
            Log.Warn($"Cannot restore field {_field.Name} on {_field.DeclaringType} because it is readonly.");
            return;
        }
        _field.SetValue(_target, _storedValue);
    }
}

public class AutoSnapshotRegistry
{
    private readonly Dictionary<object, ObjectStateSnapshot> _snapshotMap = new(SnapshotEqualityComparer.Instance);
    private readonly List<FieldReferenceSnapshot> _fieldRefSnapshots = new();
    private readonly HashSet<object> _excludedObjects = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<Type> _excludedTypes = new();
    private readonly HashSet<Type> _excludedTypeHierarchies = new();

    public AutoSnapshotRegistry(IEnumerable<object> excludedObjects = null, IEnumerable<Type> excludedTypes = null)
    {
        if (excludedObjects != null)
            foreach (var obj in excludedObjects)
                _excludedObjects.Add(obj);

        if (excludedTypes != null)
            foreach (var type in excludedTypes)
                _excludedTypes.Add(type);
    }

    public void ExcludeObject(object obj) => _excludedObjects.Add(obj);
    public void ExcludeType(Type type) => _excludedTypes.Add(type);
    public void ExcludeType<T>() => ExcludeType(typeof(T));
    public void ExcludeTypeHierarchy(Type type) => _excludedTypeHierarchies.Add(type);
    public void ExcludeTypeHierarchy<T>() => ExcludeTypeHierarchy(typeof(T));

    public void Add(object target, ObjectStateSnapshot snapshot)
    {
        _snapshotMap[target] = snapshot;
    }

    public void SnapshotRefField(Type declaringType, string fieldName)
    {
        SnapshotRefField(null, declaringType, fieldName);
    }

    public void SnapshotRefField(object target, string fieldName)
    {
        ArgumentNullException.ThrowIfNull(target);

        SnapshotRefField(target, target.GetType(), fieldName);

    }

    private void SnapshotRefField(object target, Type declaringType, string fieldName)
    {
        var field = declaringType.GetField(fieldName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? declaringType.GetField($"<{fieldName}>k__BackingField", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
            throw new InvalidOperationException($"Field or auto-property backing field '{fieldName}' not found on {declaringType.FullName}");

        var value = field.GetValue(target);
        Log.Info($"Registering {field.Name} of type {field.FieldType} in {field.DeclaringType}; Depth: 0");
        _fieldRefSnapshots.Add(new FieldReferenceSnapshot(target, field));
        CreateSnapshot(value, 1);
    }

    public void CreateSnapshot(object value, int depth)
    {
        if (value == null)
            return;

        var type = value.GetType();

        if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            return;

        // Skip excluded types
        if (IsExcludedType(type) || IsExcludedByHierarchy(type) || _excludedObjects.Contains(value))
        {
            Log.Info($"Skipping snapshot of type {type} due to exclusion.");
            return;
        }

        if (TryGet(value, out var snapshot))
        {
            Log.Warn($"Snapshot {value} of type {type} already in; loop or duplicated reference.");
            return;
        }

        //Log.Info($"Creating snapshot {value?.ToString() ?? ""} of type {type?.ToString() ?? ""}");
        snapshot = new ObjectStateSnapshot(this, value, depth);
        Add(value, snapshot);
        snapshot.Init();
    }

    private bool TryGet(object target, out ObjectStateSnapshot snapshot)
    {
        return _snapshotMap.TryGetValue(target, out snapshot);
    }

    public void RestoreAll()
    {
        foreach (var fr in _fieldRefSnapshots)
        {
            try
            {
                fr.Restore();
            }
            catch (FieldAccessException ex)
            {
                Log.Warn(ex.Message);
            }

        }

        foreach (var snapshot in _snapshotMap.Values)
        {
            try
            {
                snapshot.Restore();
            }
            catch (FieldAccessException ex)
            {
                Log.Warn(ex.Message);
            }
        }
    }

    public bool IsExcludedType(Type type)
    {
        return _excludedTypes.Contains(type) || (type.IsGenericType && _excludedTypes.Contains(type.GetGenericTypeDefinition()));
    }

    private bool IsExcludedByHierarchy(Type type)
    {
        return _excludedTypeHierarchies.Any((t) => t.IsAssignableFrom(type));
    }
}

public class ObjectStateSnapshot
{
    private readonly AutoSnapshotRegistry _dict;
    public object? StoredValue { get; private set; }
    protected Dictionary<FieldInfo, object> FieldSnapshots = new();
    public ArrayStateSnapshot collectionSnapshot = null;

    public int Depth;

    public ObjectStateSnapshot(AutoSnapshotRegistry dict, object? value, int depth)
    {
        _dict = dict;
        StoredValue = value;
        Depth = depth;
    }

    private List<FieldInfo> GetAllFields(Type type)
    {
        var fields = new List<FieldInfo>();
        var current = type;
        while (current != null && current != typeof(object) && !_dict.IsExcludedType(current))
        {
            fields.AddRange(current.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            current = current.BaseType;
        }
        return fields;
    }

    public void Init()
    {
        if (StoredValue == null)
            return;

        var type = StoredValue.GetType();

        
            var allFieldInfos = GetAllFields(type);

            FieldSnapshots = allFieldInfos
                .Select(fi =>
                {
                    object? fieldValue = fi.GetValue(StoredValue);
                    Log.Info($"Registering {fi.Name} of type {fi.FieldType} in {fi.DeclaringType}; Depth: {Depth}");
                    _dict.CreateSnapshot(fieldValue, Depth + 1);
                    return KeyValuePair.Create(fi, fieldValue);
                })
                .ToDictionary();
        
        // Snapshot array elements — arrays are restored in-place by index.
        // Non-array collections (List, Dictionary, etc.) are fully restored via their fields.
        if (StoredValue.GetType().IsArray)
        {
            collectionSnapshot = new ArrayStateSnapshot(_dict, (IEnumerable)StoredValue, Depth);
        }
    }

    public void Restore()
    {
        if (StoredValue == null)
            return;

        // Restore fields
        foreach ((FieldInfo field, object value) in FieldSnapshots)
        {
            field.SetValue(StoredValue, value);
        }

        // Restore array elements
        if (collectionSnapshot != null)
        {
            var restored = collectionSnapshot.RestoreAndReturn();

            if (!ReferenceEquals(restored, StoredValue))
            {
                throw new InvalidOperationException(
                    $"Array restoration did not return the original instance. " +
                    $"Expected: {StoredValue?.GetType()}, Got: {restored?.GetType()}");
            }
        }
    }
}

public class ArrayStateSnapshot
{
    public Array Array { get; private set; }
    public List<object> Elements { get; } = new();

    int Depth;

    public ArrayStateSnapshot(AutoSnapshotRegistry dict, IEnumerable collection, int depth)
    {
        Depth = depth;
        ArgumentNullException.ThrowIfNull(collection);

        Array = (Array)collection;

        foreach (var element in Array)
        {
            Log.Info($"Array element snapshot of type {element?.GetType()} in {Array.GetType()}; Depth: {Depth}");
            dict.CreateSnapshot(element, Depth + 1);
            Elements.Add(element);
        }
    }

    public object RestoreAndReturn()
    {
        Array.Clear(Array);
        for (int i = 0; i < Math.Min(Array.Length, Elements.Count); i++)
        {
            Array.SetValue(Elements[i], i);
        }
        return Array;
    }
}