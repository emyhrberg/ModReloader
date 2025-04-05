// using System;
// using System.Reflection;
// using Terraria;

// namespace ModHelper.Helpers
// {
//     /// <summary>
//     /// Helper class for reflection operations.
//     /// Provies error handling and type safety for setting and getting field values.
//     /// Reduces the amount of work of writing boilerplate code for reflection.
//     /// Boilerplate is code that is repeated in multiple places with little or no variation.
//     /// </summary>
//     public static class Refl
//     {
//         /// <summary>  Returns a type, for example "Terraria.ModLoader.Mod". </summary>
//         public static Type Type(string typeName)
//         {
//             Assembly assembly = typeof(Main).Assembly;
//             if (assembly == null)
//             {
//                 Log.Error("Assembly is null.");
//                 return null;
//             }

//             Type type = assembly.GetType(typeName);
//             if (type == null)
//             {
//                 Log.Error($"Type {typeName} not found in assembly {assembly.FullName}.");
//                 return null;
//             }

//             return type;
//         }

//         public static MethodInfo MethodInfo(Type type, string methodName, Type[] parameterTypes = null)
//         {
//             if (type == null)
//             {
//                 Log.Error("Type is null.");
//                 return null;
//             }

//             MethodInfo methodInfo = type.GetMethod(methodName, parameterTypes);
//             if (methodInfo == null)
//             {
//                 Log.Error($"Method {methodName} not found in type {type.FullName}.");
//                 return null;
//             }

//             return methodInfo;
//         }
//         public static FieldInfo FieldInfo(Type type, string fieldName)
//         {
//             if (type == null)
//             {
//                 Log.Error("Type is null.");
//                 return null;
//             }

//             FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
//             if (fieldInfo == null)
//             {
//                 Log.Error($"Field {fieldName} not found in type {type.FullName}.");
//                 return null;
//             }

//             return fieldInfo;
//         }
//     }
// }