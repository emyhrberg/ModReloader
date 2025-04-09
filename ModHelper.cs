using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using ModHelper.Helpers;
using ModHelper.PacketHandlers;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria;
using Terraria.ModLoader;

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
                    Diagnostic[] r = orig(name, references, files, preprocessorSymbols, allowUnsafe, out code, out pdb);

                    var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
            assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default,
            optimizationLevel: preprocessorSymbols.Contains("DEBUG") ? OptimizationLevel.Debug : OptimizationLevel.Release,
            allowUnsafe: allowUnsafe);

                    var parseOptions = new CSharpParseOptions(LanguageVersion.Preview, preprocessorSymbols: preprocessorSymbols);

                    var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);

                    //Where magic happens

                    foreach (var reference in references)
                    {
                        Log.Info($"Reference: {reference}");
                    }

                    var refs = references.Select(s => MetadataReference.CreateFromFile(s));
                    refs = refs.Concat(Net80.References.All);

                    var src = files.Select(f => SyntaxFactory.ParseSyntaxTree(System.IO.File.ReadAllText(f), parseOptions, f, Encoding.UTF8));

                    var comp = CSharpCompilation.Create(name, src, refs, options);

                    using var peStream = new MemoryStream();
                    using var pdbStream = new MemoryStream();
                    var results = comp.Emit(peStream, pdbStream, options: emitOptions);

                    code = peStream.ToArray();
                    pdb = pdbStream.ToArray();
                    Log.Info("RoslynCompileHook called");
                    RoslynCompileHook?.Dispose();
                    Log.Info("RoslynCompileHook disposed");
                    return results.Diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Warning).ToArray();

                });
            GC.SuppressFinalize(RoslynCompileHook);
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModNetHandler.HandlePacket(reader, whoAmI);
        }
    }
}