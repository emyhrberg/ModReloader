using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


namespace StateSnapshotTests
{
    // Copy all snapshot classes here (same code as in ModReloader)
    interface IStateSnapshot
    {
        public FieldInfo TargetField { get; }
        public object Parent { get; }
        object StoredValue { get; }
        void Capture();
        void Restore();
        object GetObject();
    }

    class FieldsToSnapshot
    {
        public string FieldName { get; }
        public List<FieldsToSnapshot> NestedFields { get; }
        public List<FieldsToSnapshot> NestedFieldsKey { get; }
        public List<FieldsToSnapshot> NestedFieldsValue { get; }

        public FieldsToSnapshot(string fieldName, List<FieldsToSnapshot> nestedFields = null)
        {
            FieldName = fieldName;
            NestedFields = nestedFields;
        }

        public FieldsToSnapshot(string fieldName, List<FieldsToSnapshot> nestedFieldsKey, List<FieldsToSnapshot> nestedFieldsValue)
        {
            FieldName = fieldName;
            NestedFieldsKey = nestedFieldsKey;
            NestedFieldsValue = nestedFieldsValue;
        }

        public IStateSnapshot CreateSnapshot(object parent)
        {
            var field = parent.GetType().GetField(FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                throw new ArgumentException($"Field '{FieldName}' not found in type {parent.GetType().Name}");

            var fieldType = field.FieldType;

            if (fieldType.IsArray)
                return new ArraySnapshot(field, parent, NestedFields);
            else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                return new ListSnapshot(field, parent, NestedFields);
            else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return new DictionarySnapshot(field, parent, NestedFieldsKey, NestedFieldsValue);
            else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(HashSet<>))
                return new HashSetSnapshot(field, parent, NestedFields);
            else if (fieldType.IsValueType || fieldType == typeof(string))
                return new ValueSnapshot(field, parent);
            else if (fieldType.IsClass)
                return new ClassSnapshot(field, parent, NestedFields);

            throw new NotSupportedException($"Field type {fieldType} is not supported for snapshotting.");
        }
    }

    class ValueSnapshot : IStateSnapshot
    {
        public FieldInfo TargetField { get; }
        public object Parent { get; }
        public object StoredValue { get; private set; }

        public ValueSnapshot(FieldInfo targetField, object instance)
        {
            TargetField = targetField;
            Parent = instance;
        }

        public void Capture()
        {
            StoredValue = TargetField.GetValue(Parent);
        }

        public void Restore()
        {
            TargetField.SetValue(Parent, StoredValue);
        }

        public object GetObject()
        {
            return StoredValue;
        }
    }

    class ClassSnapshot : IStateSnapshot
    {
        public FieldInfo TargetField { get; }
        public object Parent { get; }
        public List<IStateSnapshot> FieldSnapshots { get; } = new List<IStateSnapshot>();
        public object StoredValue { get; private set; }

        private List<FieldsToSnapshot> _nestedFields;

        public ClassSnapshot(FieldInfo targetField, object instance, List<FieldsToSnapshot> nestedFields)
        {
            TargetField = targetField;
            Parent = instance;
            _nestedFields = nestedFields;
        }

        public void Capture()
        {
            if (Parent == null)
                return;

            FieldSnapshots.Clear();

            if (_nestedFields == null || _nestedFields.Count <= 0)
                return;

            StoredValue = TargetField.GetValue(Parent);

            if (StoredValue == null)
                return;

            foreach (var nestedField in _nestedFields)
            {
                var snapshot = nestedField.CreateSnapshot(StoredValue);
                snapshot.Capture();
                FieldSnapshots.Add(snapshot);
            }
        }

        public void Restore()
        {
            if (Parent == null)
                return;

            var currentValue = TargetField.GetValue(Parent);

            foreach (var fieldSnapshot in FieldSnapshots)
            {
                fieldSnapshot.Restore();
            }

            TargetField.SetValue(Parent, StoredValue);
        }

        public object GetObject()
        {
            return Parent;
        }
    }

    class ElementSnapshot
    {
        public object StoredValue { get; }
        public List<IStateSnapshot> FieldSnapshots { get; } = new List<IStateSnapshot>();
        private List<FieldsToSnapshot> _nestedFields;
        public ElementSnapshot(object instance, List<FieldsToSnapshot> nestedFields)
        {
            StoredValue = instance;
            _nestedFields = nestedFields;
        }

        public void Capture()
        {
            if (StoredValue == null)
                return;

            FieldSnapshots.Clear();

            if (_nestedFields == null || _nestedFields.Count <= 0)
                return;

            foreach (var nestedField in _nestedFields)
            {
                var snapshot = nestedField.CreateSnapshot(StoredValue);
                snapshot.Capture();
                FieldSnapshots.Add(snapshot);
            }
        }

        public void Restore()
        {
            if (StoredValue == null)
                return;

            foreach (var fieldSnapshot in FieldSnapshots)
            {
                fieldSnapshot.Restore();
            }
        }

        public object GetObject()
        {
            return StoredValue;
        }
    }

    class ArraySnapshot : IStateSnapshot
    {
        public FieldInfo TargetField { get; }
        public object Parent { get; }
        public List<ElementSnapshot> ElementSnapshots { get; } = new List<ElementSnapshot>();
        public object StoredValue { get; private set; }

        private List<FieldsToSnapshot> _nestedFields;

        public ArraySnapshot(FieldInfo targetField, object instance, List<FieldsToSnapshot> nestedFields = null)
        {
            TargetField = targetField;
            Parent = instance;
            _nestedFields = nestedFields;
        }

        public void Capture()
        {
            var array = (Array)TargetField.GetValue(Parent);
            if (array == null)
                return;

            StoredValue = array;

            ElementSnapshots.Clear();

            foreach (var element in array)
            {
                if (element != null)
                {
                    var elementSnapshot = new ElementSnapshot(element, _nestedFields);
                    elementSnapshot.Capture();
                    ElementSnapshots.Add(elementSnapshot);
                }
            }
        }

        public void Restore()
        {
            var array = (Array)TargetField.GetValue(Parent);

            if (array == null && StoredValue != null)
            {
                TargetField.SetValue(Parent, StoredValue);
                array = (Array)StoredValue;
            }

            if (array == null)
            {
                return;
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (i < ElementSnapshots.Count)
                {
                    var element = ElementSnapshots[i];
                    element.Restore();
                    array.SetValue(element.GetObject(), i);
                }
                else
                {
                    array.SetValue(null, i);
                }
            }
        }

        public object GetObject()
        {
            return TargetField.GetValue(Parent);
        }
    }

    class ListSnapshot : IStateSnapshot
    {
        public FieldInfo TargetField { get; }
        public object Parent { get; }
        public List<ElementSnapshot> ElementSnapshots { get; } = new List<ElementSnapshot>();
        public object StoredValue { get; private set; }
        private List<FieldsToSnapshot> _nestedFields;

        public ListSnapshot(FieldInfo targetField, object instance, List<FieldsToSnapshot> nestedFields = null)
        {
            TargetField = targetField;
            Parent = instance;
            _nestedFields = nestedFields;
        }

        public void Capture()
        {
            var list = (IList)TargetField.GetValue(Parent);
            if (list == null)
                return;

            StoredValue = list;

            ElementSnapshots.Clear();

            foreach (var element in list)
            {
                if (element != null)
                {
                    var elementSnapshot = new ElementSnapshot(element, _nestedFields);
                    elementSnapshot.Capture();
                    ElementSnapshots.Add(elementSnapshot);
                }
            }
        }

        public void Restore()
        {
            var list = (IList)TargetField.GetValue(Parent);

            if (list == null && StoredValue != null)
            {
                TargetField.SetValue(Parent, StoredValue);
                list = (IList)StoredValue;
            }

            if (list == null)
            {
                return;
            }

            list.Clear();
            foreach (var elementSnapshot in ElementSnapshots)
            {
                elementSnapshot.Restore();
                list.Add(elementSnapshot.GetObject());
            }
        }

        public object GetObject() => TargetField.GetValue(Parent);
    }

    class DictionarySnapshot : IStateSnapshot
    {
        public FieldInfo TargetField { get; }
        public object Parent { get; }
        public object StoredValue { get; private set; }
        public List<(ElementSnapshot Key, ElementSnapshot Value)> EntrySnapshots { get; } = new List<(ElementSnapshot, ElementSnapshot)>();

        private List<FieldsToSnapshot> _nestedFieldsKey;
        private List<FieldsToSnapshot> _nestedFieldsValue;

        public DictionarySnapshot(FieldInfo targetField, object instance, List<FieldsToSnapshot> nestedFieldsKey = null, List<FieldsToSnapshot> nestedFieldsValue = null)
        {
            TargetField = targetField;
            Parent = instance;
            _nestedFieldsKey = nestedFieldsKey;
            _nestedFieldsValue = nestedFieldsValue;
        }

        public void Capture()
        {
            var dict = (IDictionary)TargetField.GetValue(Parent);
            if (dict == null)
                return;

            StoredValue = dict;

            EntrySnapshots.Clear();

            foreach (DictionaryEntry entry in dict)
            {
                ElementSnapshot keySnapshot = null;
                ElementSnapshot valueSnapshot = null;

                if (entry.Key != null)
                {
                    keySnapshot = new ElementSnapshot(entry.Key, _nestedFieldsKey);
                    keySnapshot.Capture();
                }

                if (entry.Value != null)
                {
                    valueSnapshot = new ElementSnapshot(entry.Value, _nestedFieldsValue);
                    valueSnapshot.Capture();
                }

                EntrySnapshots.Add((keySnapshot, valueSnapshot));
            }
        }

        public void Restore()
        {
            var dict = (IDictionary)TargetField.GetValue(Parent);

            if (dict == null && StoredValue != null)
            {
                TargetField.SetValue(Parent, StoredValue);
                dict = (IDictionary)StoredValue;
            }

            if (dict == null)
                return;

            dict.Clear();

            foreach (var (keySnapshot, valueSnapshot) in EntrySnapshots)
            {
                keySnapshot?.Restore();
                valueSnapshot?.Restore();

                var key = keySnapshot?.GetObject();
                var value = valueSnapshot?.GetObject();

                if (key != null)
                {
                    dict[key] = value;
                }
            }
        }

        public object GetObject() => TargetField.GetValue(Parent);
    }

    class HashSetSnapshot : IStateSnapshot
    {
        public FieldInfo TargetField { get; }
        public object Parent { get; }
        public object StoredValue { get; private set; }
        public List<ElementSnapshot> ElementSnapshots { get; } = new List<ElementSnapshot>();
        private List<FieldsToSnapshot> _nestedFields;

        public HashSetSnapshot(FieldInfo targetField, object instance, List<FieldsToSnapshot> nestedFields = null)
        {
            TargetField = targetField;
            Parent = instance;
            _nestedFields = nestedFields;
        }

        public void Capture()
        {
            var hashSet = (IEnumerable)TargetField.GetValue(Parent);
            if (hashSet == null)
                return;

            StoredValue = hashSet;

            ElementSnapshots.Clear();

            foreach (var element in hashSet)
            {
                if (element != null)
                {
                    var elementSnapshot = new ElementSnapshot(element, _nestedFields);
                    elementSnapshot.Capture();
                    ElementSnapshots.Add(elementSnapshot);
                }
            }
        }

        public void Restore()
        {
            var hashSet = TargetField.GetValue(Parent);

            if (hashSet == null && StoredValue != null)
            {
                TargetField.SetValue(Parent, StoredValue);
                hashSet = StoredValue;
            }

            if (hashSet == null)
                return;

            var clearMethod = hashSet.GetType().GetMethod("Clear");
            var addMethod = hashSet.GetType().GetMethod("Add");

            if (clearMethod != null && addMethod != null)
            {
                clearMethod.Invoke(hashSet, null);

                foreach (var elementSnapshot in ElementSnapshots)
                {
                    elementSnapshot.Restore();
                    addMethod.Invoke(hashSet, new[] { elementSnapshot.GetObject() });
                }
            }
        }

        public object GetObject() => TargetField.GetValue(Parent);
    }
}
