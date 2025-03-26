using EliteTestingMod.Helpers;
using System;
using System.Collections;
using System.Reflection;

namespace EliteTestingMod.CustomReload
{

    //Backup of an property, saving it initial state and allowing to restore initial state
    class PropertyBackup
    {
        //private Type _targetType;
        private object _target;
        private PropertyInfo _property;
        private object _clonedValue;
        public object FieldValue
        {
            get
            {
                return _property.GetValue(_target);
            }
            set
            {
                _property.SetValue(_target, value);
            }
        }

        public object ClonedValue
        {
            get
            {
                return _clonedValue;
            }
        }

        public PropertyBackup(Type targetType, PropertyInfo field, object target = null)
        {
            //_targetType = targetType;
            _property = field ?? throw new ArgumentException($"Field not found in {targetType.Name}.");
            _target = target;

            Backup();
        }

        private void Backup()
        {
            if (_property.PropertyType.IsValueType) // Check if it's a reference type
            {
                _clonedValue = FieldValue;
                Log.Info($"Field {_property.Name} of type {_property.PropertyType} is Value Type");
            }
            else
            {
                if (FieldValue is ICloneable cloneable) // If it implements ICloneable, use it
                {
                    _clonedValue = cloneable.Clone();
                    Log.Info($"Field {_property.Name} of type {_property.PropertyType} is cloned by ICloneable");
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
                    Log.Info($"Field {_property.Name} of type {_property.PropertyType} is cloned as Dictionary");
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
                    Log.Info($"Field {_property.Name} of type {_property.PropertyType} is cloned as List");
                }
                else
                {
                    Log.Error($"Field {_property.Name} of type {_property.PropertyType} cannot be clonned");
                    throw new Exception($"Field {_property.Name} of type {_property.PropertyType} cannot be clonned");
                }
            }
        }

        public PropertyBackup(Type targetType, string propertyName, BindingFlags flags, object target = null) :
            this(targetType, targetType.GetProperty(propertyName, flags))
        { }

        public void Restore()
        {
            if (_clonedValue == null)
            {
                Log.Error($"No backup exists for field {_property.Name}");
                throw new Exception($"No backup exists for field {_property.Name}");
            }

            if (_property.PropertyType.IsValueType) // If it's a value type, assign directly
            {
                _property.SetValue(_target, _clonedValue);
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

                    Log.Info($"Field {_property.Name} of type {_property.PropertyType} is restored as Dictionary");
                }


                else if (_clonedValue is IList list) // Restore List
                {
                    var targetList = (IList)_property.GetValue(_target);

                    targetList.Clear();
                    foreach (var item in list)
                    {
                        targetList.Add(item);
                    }
                    Log.Info($"Field {_property.Name} of type {_property.PropertyType} is restored as List");
                }
                else // Restore by assigning the cloned reference
                {
                    _property.SetValue(_target, _clonedValue);
                    Log.Info($"Field {_property.Name} of type {_property.PropertyType} is restored by direct assignment");
                }
            }
        }

    }
}
