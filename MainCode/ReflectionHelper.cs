using System;
using System.Reflection;

public static class ReflectionHelper
{
    public static FieldInfo GetField(Type type, string fieldName, BindingFlags bindingFlags)
    {
        return type.GetField(fieldName, bindingFlags);
    }

    public static object GetFieldValue(FieldInfo fieldInfo, object instance = null)
    {
        return fieldInfo?.GetValue(instance);
    }

    public static void SetFieldValue(FieldInfo fieldInfo, object instance, object value)
    {
        fieldInfo?.SetValue(instance, value);
    }

    public static PropertyInfo GetProperty(Type type, string propertyName, BindingFlags bindingFlags)
    {
        return type.GetProperty(propertyName, bindingFlags);
    }

    public static object GetPropertyValue(PropertyInfo propertyInfo, object instance = null)
    {
        return propertyInfo?.GetValue(instance);
    }

    public static void SetPropertyValue(PropertyInfo propertyInfo, object instance, object value)
    {
        propertyInfo?.SetValue(instance, value);
    }
}
