using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Basic.Reference.Assemblies;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using ModReloader.Helpers;
using MonoMod.RuntimeDetour;

namespace ModReloader.Publicizer
{
    public class CompileSystem : ModSystem
    {
        // This is the delegate that will be used to hook into the RoslynCompile method
        public delegate Diagnostic[] RoslynCompileDelegate(
            string name,
            List<string> references,
            string[] files,
            string[] preprocessorSymbols,
            bool allowUnsafe,
            out byte[] code,
            out byte[] pdb);

        private Hook RoslynCompileHook;

        // In load, we hook into the RoslynCompile method and replace it with our own implementation
        // that uses Publicizer to modify the assembly before compilation.
        public override void Load()
        {
            Assembly tModLoaderAssembly = typeof(Main).Assembly;
            Type modCompileType = tModLoaderAssembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo roslynCompileMethod = modCompileType.GetMethod("RoslynCompile", BindingFlags.NonPublic | BindingFlags.Static);
            RoslynCompileHook = new Hook(roslynCompileMethod,
                (RoslynCompileDelegate orig, string name, List<string> references, string[] files, string[] preprocessorSymbols, bool allowUnsafe, out byte[] code, out byte[] pdb) =>
                {
                    Log.Info("RoslynCompileHook called");
                    try
                    {
                        // Find the csproj file
                        string csprojFile = CompilerUtilities.FindCsprojFile(name, files);
                        if (!File.Exists(csprojFile))
                        {
                            throw new Exception($"No csproj found in {csprojFile}");
                        }
                        else
                        {
                            Log.Info($"Found csproj file: {csprojFile}");
                        }

                        // Load the csproj file and find the Publicize references and their context
                        var doc = XDocument.Load(csprojFile);
                        var assemblyContexts = CompilerUtilities.GetPublicizerAssemblyContexts(doc);
                        var referencesToPublicize = assemblyContexts.Keys.ToHashSet().ToList();

                        if (referencesToPublicize.Count == 0)
                        {
                            throw new Exception($"No Publicized mod references found in {Path.GetFileName(csprojFile)}");
                        }
                        else
                        {
                            Log.Info($"Publicize mod references found in {Path.GetFileName(csprojFile)}: {string.Join(", ", referencesToPublicize)}");
                        }

                        // Finding the dlls to publicize
                        Dictionary<string, string> dllPathsToPublicize = CompilerUtilities.FindReferencePaths(references, referencesToPublicize);

                        // Publicizing (or reading from files) the dlls
                        var publicizedModReferences = new List<PortableExecutableReference>();
                        foreach (var r in referencesToPublicize)
                        {
                            // Check if the dll path is valid
                            if (!dllPathsToPublicize.ContainsKey(r))
                            {
                                Log.Error($"Failed to find {r} in references!");
                                continue;
                            }

                            // Get the assembly context and dll path
                            var assemblyContext = assemblyContexts[r];
                            var dllPath = dllPathsToPublicize[r];

                            // Compute the hash and filename of the publicized dll path
                            var hash = CompilerUtilities.ComputeHash(dllPath, assemblyContext);
                            var filePath = CompilerUtilities.GetPRFolderPath($"{r}.{hash}.dll");

                            // Check if the publicized dll already exists
                            if (File.Exists(filePath))
                            {
                                Log.Info($"Publicized mod reference {r} already exists, loading from {Path.GetFileName(filePath)}");

                                // Loading the publicized dll
                                var publicizedModReference = MetadataReference.CreateFromFile(filePath);
                                publicizedModReferences.Add(publicizedModReference);
                            }
                            else
                            {
                                Log.Info($"Publicizing mod reference {r} to {Path.GetFileName(filePath)}");

                                // Creating a module
                                using ModuleDef module = ModuleDefMD.Load(dllPathsToPublicize[r]);

                                // Publicizing the module
                                bool moduleChanged = PublicizeAssemblies.PublicizeAssembly(module, assemblyContext);
                                if (moduleChanged)
                                {
                                    Log.Info($"Module {r} is changed!");
                                }
                                else
                                {
                                    Log.Info($"Module {r} isn't changed!");
                                    continue;
                                }

                                // Writing the publicized dll to a file
                                var writerOptions = new ModuleWriterOptions(module)
                                {
                                    MetadataOptions = new MetadataOptions(MetadataFlags.KeepOldMaxStack),
                                    Logger = DummyLogger.NoThrowInstance
                                };
                                Utilities.LockingFile(filePath, (reader, writer) =>
                                {
                                    module.Write(writer.BaseStream, writerOptions);
                                });

                                // Loading the publicized dll
                                var publicizedModReference = MetadataReference.CreateFromFile(filePath);
                                publicizedModReferences.Add(publicizedModReference);
                            }
                        }

                        // Normal RoslynCompiler method
                        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default,
                optimizationLevel: preprocessorSymbols.Contains("DEBUG") ? OptimizationLevel.Debug : OptimizationLevel.Release,
                allowUnsafe: true);

                        var parseOptions = new CSharpParseOptions(LanguageVersion.Preview, preprocessorSymbols: preprocessorSymbols);

                        var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);

                        var refs = references.Select(s => MetadataReference.CreateFromFile(s));
                        refs = refs.Concat(Net80.References.All);

                        // Adding references to the publicized mod
                        refs = refs.Concat(publicizedModReferences);

                        var src = files.Select(f => SyntaxFactory.ParseSyntaxTree(File.ReadAllText(f), parseOptions, f, Encoding.UTF8));

                        // IACT 1
                        var asmAttrs = string.Join(Environment.NewLine,
                        referencesToPublicize.Select(name =>
                            $@"[assembly: System.Runtime.CompilerServices.IgnoresAccessChecksTo(""{name}"")]"));

                        // IACT 2
                        const string attrDecl = @"
                        namespace System.Runtime.CompilerServices {
                            [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
                            internal sealed class IgnoresAccessChecksToAttribute : System.Attribute {
                                public IgnoresAccessChecksToAttribute(string assemblyName) {}
                            }
                        }";

                        // adding IACT to the top of the file
                        src = src
                            .Prepend(SyntaxFactory.ParseSyntaxTree(asmAttrs, parseOptions))
                            .Append(SyntaxFactory.ParseSyntaxTree(attrDecl, parseOptions));

                        var comp = CSharpCompilation.Create(name, src, refs, options);

                        using var peStream = new MemoryStream();
                        using var pdbStream = new MemoryStream();
                        var results = comp.Emit(peStream, pdbStream, options: emitOptions);

                        code = peStream.ToArray();
                        pdb = pdbStream.ToArray();

                        return results.Diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Warning).ToArray();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"RoslynCompileHook error: {ex.Message}");

                        // Return the original method
                        return orig(name, references, files, preprocessorSymbols, allowUnsafe, out code, out pdb);
                    }
                });
            GC.SuppressFinalize(RoslynCompileHook);
        }

        public override void Unload()
        {
            RoslynCompileHook?.Dispose();
        }
    }
}