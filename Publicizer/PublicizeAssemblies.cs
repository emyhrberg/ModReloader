using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using ModReloader.Helpers;

namespace ModReloader.Publicizer
{
    // Almost all of this code are taken from Krafs.Publicizer
    // https://github.com/krafs/Publicizer
    internal class PublicizeAssemblies
    {
        /// <summary>
        /// Publicizes the assembly members based on the provided context.
        /// </summary>
        /// <param name="module">Assembly module to be publicized.</param>
        /// <param name="assemblyContext">Context containing publicize settings.</param>
        /// <returns>True if any member was publicized; otherwise, false.</returns>
        public static bool PublicizeAssembly(ModuleDef module, PublicizerAssemblyContext assemblyContext)
        {
            bool publicizedAnyMemberInAssembly = false;
            var doNotPublicizePropertyMethods = new HashSet<MethodDef>();

            int publicizedTypesCount = 0;
            int publicizedPropertiesCount = 0;
            int publicizedMethodsCount = 0;
            int publicizedFieldsCount = 0;

            // TYPES
            foreach (TypeDef? typeDef in module.GetTypes())
            {
                doNotPublicizePropertyMethods.Clear();

                bool publicizedAnyMemberInType = false;
                string typeName = typeDef.ReflectionFullName;

                bool explicitlyDoNotPublicizeType = assemblyContext.DoNotPublicizeMemberPatterns.Contains(typeName);

                // PROPERTIES
                foreach (PropertyDef? propertyDef in typeDef.Properties)
                {
                    string propertyName = $"{typeName}.{propertyDef.Name}";

                    bool explicitlyDoNotPublicizeProperty = assemblyContext.DoNotPublicizeMemberPatterns.Contains(propertyName);
                    if (explicitlyDoNotPublicizeProperty)
                    {
                        if (propertyDef.GetMethod is MethodDef getter)
                        {
                            doNotPublicizePropertyMethods.Add(getter);
                        }
                        if (propertyDef.SetMethod is MethodDef setter)
                        {
                            doNotPublicizePropertyMethods.Add(setter);
                        }
                        Log.Info($"Explicitly ignoring property: {propertyName}");
                        continue;
                    }

                    bool explicitlyPublicizeProperty = assemblyContext.PublicizeMemberPatterns.Contains(propertyName);
                    if (explicitlyPublicizeProperty)
                    {
                        if (AssemblyEditor.PublicizeProperty(propertyDef))
                        {
                            publicizedAnyMemberInType = true;
                            publicizedAnyMemberInAssembly = true;
                            publicizedPropertiesCount++;
                            Log.Info($"Explicitly publicizing property: {propertyName}");
                        }
                        continue;
                    }

                    if (explicitlyDoNotPublicizeType)
                    {
                        continue;
                    }

                    if (assemblyContext.ExplicitlyDoNotPublicizeAssembly)
                    {
                        continue;
                    }

                    if (assemblyContext.ExplicitlyPublicizeAssembly)
                    {
                        bool isCompilerGeneratedProperty = IsCompilerGenerated(propertyDef);
                        if (isCompilerGeneratedProperty && !assemblyContext.IncludeCompilerGeneratedMembers)
                        {
                            continue;
                        }

                        bool isRegexPatternMatch = assemblyContext.PublicizeMemberRegexPattern?.IsMatch(propertyName) ?? true;
                        if (!isRegexPatternMatch)
                        {
                            continue;
                        }

                        if (AssemblyEditor.PublicizeProperty(propertyDef, assemblyContext.IncludeVirtualMembers))
                        {
                            publicizedAnyMemberInType = true;
                            publicizedAnyMemberInAssembly = true;
                            publicizedPropertiesCount++;
                        }
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

                    bool explicitlyDoNotPublicizeMethod = assemblyContext.DoNotPublicizeMemberPatterns.Contains(methodName);
                    if (explicitlyDoNotPublicizeMethod)
                    {
                        Log.Info($"Explicitly ignoring method: {methodName}");
                        continue;
                    }

                    bool explicitlyPublicizeMethod = assemblyContext.PublicizeMemberPatterns.Contains(methodName);
                    if (explicitlyPublicizeMethod)
                    {
                        if (AssemblyEditor.PublicizeMethod(methodDef))
                        {
                            publicizedAnyMemberInType = true;
                            publicizedAnyMemberInAssembly = true;
                            publicizedMethodsCount++;
                            Log.Info($"Explicitly publicizing method: {methodName}");
                        }
                        continue;
                    }

                    if (explicitlyDoNotPublicizeType)
                    {
                        continue;
                    }

                    if (assemblyContext.ExplicitlyDoNotPublicizeAssembly)
                    {
                        continue;
                    }

                    if (assemblyContext.ExplicitlyPublicizeAssembly)
                    {
                        bool isCompilerGeneratedMethod = IsCompilerGenerated(methodDef);
                        if (isCompilerGeneratedMethod && !assemblyContext.IncludeCompilerGeneratedMembers)
                        {
                            continue;
                        }

                        bool isRegexPatternMatch = assemblyContext.PublicizeMemberRegexPattern?.IsMatch(methodName) ?? true;
                        if (!isRegexPatternMatch)
                        {
                            continue;
                        }

                        if (AssemblyEditor.PublicizeMethod(methodDef, assemblyContext.IncludeVirtualMembers))
                        {
                            publicizedAnyMemberInType = true;
                            publicizedAnyMemberInAssembly = true;
                            publicizedMethodsCount++;
                        }
                    }
                }

                // FIELDS
                foreach (FieldDef? fieldDef in typeDef.Fields)
                {
                    string fieldName = $"{typeName}.{fieldDef.Name}";

                    bool explicitlyDoNotPublicizeField = assemblyContext.DoNotPublicizeMemberPatterns.Contains(fieldName);
                    if (explicitlyDoNotPublicizeField)
                    {
                        Log.Info($"Explicitly ignoring field: {fieldName}");
                        continue;
                    }

                    bool explicitlyPublicizeField = assemblyContext.PublicizeMemberPatterns.Contains(fieldName);
                    if (explicitlyPublicizeField)
                    {
                        if (AssemblyEditor.PublicizeField(fieldDef))
                        {
                            publicizedAnyMemberInType = true;
                            publicizedAnyMemberInAssembly = true;
                            publicizedFieldsCount++;
                            Log.Info($"Explicitly publicizing field: {fieldName}");
                        }
                        continue;
                    }

                    if (explicitlyDoNotPublicizeType)
                    {
                        continue;
                    }

                    if (assemblyContext.ExplicitlyDoNotPublicizeAssembly)
                    {
                        continue;
                    }

                    if (assemblyContext.ExplicitlyPublicizeAssembly)
                    {
                        bool isCompilerGeneratedField = IsCompilerGenerated(fieldDef);
                        if (isCompilerGeneratedField && !assemblyContext.IncludeCompilerGeneratedMembers)
                        {
                            continue;
                        }

                        bool isRegexPatternMatch = assemblyContext.PublicizeMemberRegexPattern?.IsMatch(fieldName) ?? true;
                        if (!isRegexPatternMatch)
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

                if (explicitlyDoNotPublicizeType)
                {
                    Log.Info($"Explicitly ignoring type: {typeName}");
                    continue;
                }

                bool explicitlyPublicizeType = assemblyContext.PublicizeMemberPatterns.Contains(typeName);
                if (explicitlyPublicizeType)
                {
                    if (AssemblyEditor.PublicizeType(typeDef))
                    {
                        publicizedAnyMemberInAssembly = true;
                        publicizedTypesCount++;
                        Log.Info($"Explicitly publicizing type: {typeName}");
                    }
                    continue;
                }

                if (assemblyContext.ExplicitlyDoNotPublicizeAssembly)
                {
                    continue;
                }

                if (assemblyContext.ExplicitlyPublicizeAssembly)
                {
                    bool isCompilerGeneratedType = IsCompilerGenerated(typeDef);
                    if (isCompilerGeneratedType && !assemblyContext.IncludeCompilerGeneratedMembers)
                    {
                        continue;
                    }

                    bool isRegexPatternMatch = assemblyContext.PublicizeMemberRegexPattern?.IsMatch(typeName) ?? true;
                    if (!isRegexPatternMatch)
                    {
                        continue;
                    }

                    if (AssemblyEditor.PublicizeType(typeDef))
                    {
                        publicizedAnyMemberInAssembly = true;
                        publicizedTypesCount++;
                    }
                }
            }

            Log.Info("Publicized types: " + publicizedTypesCount);
            Log.Info("Publicized properties: " + publicizedPropertiesCount);
            Log.Info("Publicized methods: " + publicizedMethodsCount);
            Log.Info("Publicized fields: " + publicizedFieldsCount);

            return publicizedAnyMemberInAssembly;
        }

        /// <summary>
        /// Checks if the member is compiler generated.
        /// </summary>
        /// <param name="memberDef">Member definition to check.</param>
        /// <returns>True if the member is compiler generated; otherwise, false.</returns>
        private static bool IsCompilerGenerated(IHasCustomAttribute memberDef)
        {
            return memberDef.CustomAttributes.Any(x => x.TypeFullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
        }

    }
}
