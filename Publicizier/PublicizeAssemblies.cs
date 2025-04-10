using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using ModHelper.Helpers;
using Mono.CompilerServices.SymbolWriter;

namespace ModHelper.Publicizier
{
    internal class PublicizeAssemblies
    {
        public static bool PublicizeAssembly(ModuleDef module)
        {
            bool publicizedAnyMemberInAssembly = false;

            bool includeVirtual = false; // Include virtual members in the publicization process

            var doNotPublicizePropertyMethods = new HashSet<MethodDef>();

            int publicizedTypesCount = 0;
            int publicizedPropertiesCount = 0;
            int publicizedMethodsCount = 0;
            int publicizedFieldsCount = 0;

            foreach (TypeDef? typeDef in module.GetTypes())
            {
                doNotPublicizePropertyMethods.Clear();

                bool publicizedAnyMemberInType = false;
                string typeName = typeDef.ReflectionFullName;

                bool explicitlyDoNotPublicizeType = false; //assemblyContext.DoNotPublicizeMemberPatterns.Contains(typeName);

                // PROPERTIES
                foreach (PropertyDef? propertyDef in typeDef.Properties)
                {
                    string propertyName = $"{typeName}.{propertyDef.Name}";

                    bool isCompilerGeneratedProperty = IsCompilerGenerated(propertyDef);
                    if (isCompilerGeneratedProperty)
                    {
                        continue;
                    }

                    if (AssemblyEditor.PublicizeProperty(propertyDef, includeVirtual))
                    {
                        publicizedAnyMemberInType = true;
                        publicizedAnyMemberInAssembly = true;
                        publicizedPropertiesCount++;
                    }
                }

                // METHODS
                foreach (MethodDef? methodDef in typeDef.Methods)
                {
                    string methodName = $"{typeName}.{methodDef.Name}";

                    bool isMethodOfNonPublicizedProperty = doNotPublicizePropertyMethods.Contains(methodDef);
                    if (isMethodOfNonPublicizedProperty)
                    {
                        continue;
                    }

                    bool isCompilerGeneratedMethod = IsCompilerGenerated(methodDef);
                    if (isCompilerGeneratedMethod)
                    {
                        continue;
                    }

                    if (AssemblyEditor.PublicizeMethod(methodDef, includeVirtual))
                    {
                        publicizedAnyMemberInType = true;
                        publicizedAnyMemberInAssembly = true;
                        publicizedMethodsCount++;
                    }
                }

                // FIELDS
                foreach (FieldDef? fieldDef in typeDef.Fields)
                {
                    string fieldName = $"{typeName}.{fieldDef.Name}";

                    bool isCompilerGeneratedField = IsCompilerGenerated(fieldDef);
                    if (isCompilerGeneratedField)
                    {
                        continue;
                    }

                    if (AssemblyEditor.PublicizeField(fieldDef))
                    {
                        publicizedAnyMemberInType = true;
                        publicizedAnyMemberInAssembly = true;
                        publicizedFieldsCount++;
                    }

                }

                if (publicizedAnyMemberInType)
                {
                    if (AssemblyEditor.PublicizeType(typeDef))
                    {
                        publicizedAnyMemberInAssembly = true;
                        publicizedTypesCount++;
                    }
                    continue;
                }


                bool isCompilerGeneratedType = IsCompilerGenerated(typeDef);
                if (isCompilerGeneratedType)
                {
                    continue;
                }

                if (AssemblyEditor.PublicizeType(typeDef))
                {
                    publicizedAnyMemberInAssembly = true;
                    publicizedTypesCount++;
                }

            }

            Log.Info("Publicized types: " + publicizedTypesCount);
            Log.Info("Publicized properties: " + publicizedPropertiesCount);
            Log.Info("Publicized methods: " + publicizedMethodsCount);
            Log.Info("Publicized fields: " + publicizedFieldsCount);

            return publicizedAnyMemberInAssembly;
        }
        private static bool IsCompilerGenerated(IHasCustomAttribute memberDef)
        {
            return memberDef.CustomAttributes.Any(x => x.TypeFullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
        }

    }
}
