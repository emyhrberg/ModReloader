using Basic.Reference.Assemblies;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using log4net;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using ModHelper.Helpers;
using ModHelper.PacketHandlers;
using ModHelper.Publicizier;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace ModHelper
{
    // If no Autoload(Side) is provided, it will default to Both (which is wanted in this case)
    // [Autoload(Side = ModSide.Client)]
    // [Autoload(Side = ModSide.Both)]
    public class ModHelper : Mod
    {
        public static ModHelper Instance { get; private set; }

        public delegate Diagnostic[] RoslynCompileDelegate(
            string name,
            List<string> references,
            string[] files,
            string[] preprocessorSymbols,
            bool allowUnsafe,
            out byte[] code,
            out byte[] pdb);

        public override void Load()
        {
            Instance = this;
            Hook RoslynCompileHook = null;
            Assembly tModLoaderAssembly = typeof(Main).Assembly;
            Type modCompileType = tModLoaderAssembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo roslynCompileMethod = modCompileType.GetMethod("RoslynCompile", BindingFlags.NonPublic | BindingFlags.Static);
            RoslynCompileHook = new Hook(roslynCompileMethod,
                (RoslynCompileDelegate orig, string name, List<string> references, string[] files, string[] preprocessorSymbols, bool allowUnsafe, out byte[] code, out byte[] pdb) =>
                {
                    Log.Info("RoslynCompileHook called");
                    try
                    {
                        Log.Info($"Trying to find csproj file");
                        string csprojFile = CompilerUtilities.FindProjectFile(name, files);

                        if (!System.IO.File.Exists(csprojFile))
                        {
                            throw new Exception($"No csproj found in {csprojFile}");
                        }

                        Log.Info($"File found {csprojFile}");

                        var doc = XDocument.Load(csprojFile);
                        XNamespace ns = doc.Root!.Name.Namespace;

                        var referencesToPublicize = doc.Descendants(ns + "Publicize")
                                                  .Select(e => e.Attribute("Include")?.Value).ToList();

                        if (referencesToPublicize.Count == 0)
                        {
                            throw new Exception($"No Publicize references found in {Path.GetFileName(csprojFile)}");
                        }

                        Log.Info($"Publicize references found in {Path.GetFileName(csprojFile)}: {string.Join(", ", referencesToPublicize)}");

                        Dictionary<string, string> dllsToPublicize = CompilerUtilities.FindReferencePaths(references, referencesToPublicize);

                        var publicizedModReferences = new List<PortableExecutableReference>();

                        foreach (var r in referencesToPublicize)
                        {
                            using ModuleDef module = ModuleDefMD.Load(dllsToPublicize[r]);
                            Log.Info(PublicizeAssemblies.PublicizeAssembly(module) ? $"Module {r} is changed!" : $"Something wrong with module {r}");

                            var writerOptions = new ModuleWriterOptions(module)
                            {
                                MetadataOptions = new MetadataOptions(MetadataFlags.KeepOldMaxStack),
                                Logger = DummyLogger.NoThrowInstance
                            };

                            MemoryStream modDefStream = new MemoryStream();
                            module.Write(modDefStream, writerOptions);

                            publicizedModReferences.Add(MetadataReference.CreateFromImage(modDefStream.ToArray()));

                            Log.Info($"Publicized references count: {publicizedModReferences.Count()}");
                        }

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


                        var src = files.Select(f => SyntaxFactory.ParseSyntaxTree(System.IO.File.ReadAllText(f), parseOptions, f, Encoding.UTF8));

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

                        RoslynCompileHook?.Dispose();
                        return results.Diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Warning).ToArray();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"RoslynCompileHook error: {ex.Message}");
                        var r = orig(name, references, files, preprocessorSymbols, allowUnsafe, out code, out pdb);
                        RoslynCompileHook?.Dispose();
                        return r;
                    }
                });
            GC.SuppressFinalize(RoslynCompileHook);

            //LocalMod[] l = [];
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModNetHandler.HandlePacket(reader, whoAmI);
        }
    }
}