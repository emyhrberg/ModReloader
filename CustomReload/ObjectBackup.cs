using SquidTestingMod.Helpers;
using System;
using System.Collections;
using System.Reflection;

namespace SquidTestingMod.CustomReload
{

    //Backup of an object, saving it initial state and allowing to restore initial state
    class ObjectBackup
    {
        private Type _targetType;
        private object _target;
        private FieldInfo _field;
        private object _clonedValue;
        public object FieldValue
        {
            get
            {
                return _field.GetValue(_target);
            }
            set
            {
                _field.SetValue(_target, value);
            }
        }

        public ObjectBackup(Type targetType, FieldInfo field, object target = null)
        {
            _targetType = targetType;
            _field = field ?? throw new ArgumentException($"Field not found in {targetType.Name}.");
            _target = target;

            Backup();
        }

        private void Backup()
        {
            if (_field.FieldType.IsValueType) // Check if it's a reference type
            {
                _clonedValue = FieldValue;
                Log.Info($"Field {_field.Name} of type {_field.FieldType} is Value Type");
            }
            else
            {
                if (FieldValue is ICloneable cloneable) // If it implements ICloneable, use it
                {
                    _clonedValue = cloneable.Clone();
                    Log.Info($"Field {_field.Name} of type {_field.FieldType} is cloned by ICloneable");
                }
                else if (FieldValue is IDictionary dictionary) // Handle Dictionary cloning
                {
                    Type dictType = FieldValue.GetType();
                    var newDict = (IDictionary)Activator.CreateInstance(dictType);

                    foreach (DictionaryEntry entry in dictionary)
                    {
                        newDict.Add(entry.Key, entry.Value); // Shallow copy
                    }
                    _clonedValue = newDict;
                    Log.Info($"Field {_field.Name} of type {_field.FieldType} is cloned as Dictionary");
                }
                else if (FieldValue is IList list) // Handle List cloning
                {
                    Type listType = FieldValue.GetType();
                    var newList = (IList)Activator.CreateInstance(listType);

                    foreach (var item in list)
                    {
                        newList.Add(item); // Shallow copy
                    }
                    _clonedValue = newList;
                    Log.Info($"Field {_field.Name} of type {_field.FieldType} is cloned as List");
                }
                else
                {
                    Log.Error($"Field {_field.Name} of type {_field.FieldType} cannot be clonned");
                    throw new Exception($"Field {_field.Name} of type {_field.FieldType} cannot be clonned");
                }
            }
        }

        public ObjectBackup(Type targetType, string fieldName, BindingFlags flags, object target = null) :
            this(targetType, targetType.GetField(fieldName, flags))
        { }

        public void Restore()
        {
            if (_clonedValue == null)
            {
                Log.Error($"No backup exists for field {_field.Name}");
                throw new Exception($"No backup exists for field {_field.Name}");
            }

            if (_field.FieldType.IsValueType) // If it's a value type, assign directly
            {
                _field.SetValue(_target, _clonedValue);
            }
            else
            {
                if (_clonedValue is IDictionary dictionary) // Restore Dictionary
                {
                    var targetDict = (IDictionary)FieldValue;

                    targetDict.Clear();
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        targetDict.Add(entry.Key, entry.Value);
                    }

                    Log.Info($"Field {_field.Name} of type {_field.FieldType} is restored as Dictionary");
                }


                else if (_clonedValue is IList list) // Restore List
                {
                    var targetList = (IList)_field.GetValue(_target);

                    targetList.Clear();
                    foreach (var item in list)
                    {
                        targetList.Add(item);
                    }
                    Log.Info($"Field {_field.Name} of type {_field.FieldType} is restored as List");
                }
                else // Restore by assigning the cloned reference
                {
                    _field.SetValue(_target, _clonedValue);
                    Log.Info($"Field {_field.Name} of type {_field.FieldType} is restored by direct assignment");
                }
            }
        }

    }
}
