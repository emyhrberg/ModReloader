using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
                    try
                    {
                        Log.Info("RoslynCompileHook called");
                        //Diagnostic[] r = orig(name, references, files, preprocessorSymbols, allowUnsafe, out code, out pdb);

                        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default,
                optimizationLevel: preprocessorSymbols.Contains("DEBUG") ? OptimizationLevel.Debug : OptimizationLevel.Release,
                allowUnsafe: true);

                        var parseOptions = new CSharpParseOptions(LanguageVersion.Preview, preprocessorSymbols: preprocessorSymbols);

                        var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);

                        //Where magic happens

                        var tmlReference = references.FirstOrDefault(s => Path.GetFileNameWithoutExtension(s) == "tModLoader");

                        if (tmlReference != null)
                        {
                            references.Remove(tmlReference);
                        }

                        foreach (var reference in references)
                        {
                            Log.Info("Raw Reference: " + reference);
                        }

                        using ModuleDef module = ModuleDefMD.Load(tmlReference);
                        Log.Info(PublicizeAssemblies.PublicizeAssembly(module) ? "Module is changed!" : "Someething wrong with module");

                        var writerOptions = new ModuleWriterOptions(module)
                        {
                            // Writing the module sometime fails without this flag due to how it was originally compiled.
                            // https://github.com/krafs/Publicizer/issues/42
                            MetadataOptions = new MetadataOptions(MetadataFlags.KeepOldMaxStack),
                            Logger = DummyLogger.NoThrowInstance
                        };

                        MemoryStream modDefStream = new MemoryStream();
                        module.Write(modDefStream, writerOptions);

                        // 2. 
                        var modReference = MetadataReference.CreateFromImage(modDefStream.ToArray());

                        var refs = references.Select(s => MetadataReference.CreateFromFile(s));
                        refs = refs.Concat(Net80.References.All);

                        refs = refs.Append(modReference);

                        foreach (var reference in refs)
                        {
                            Log.Info("True Reference: " + reference.FilePath);
                        }


                        var src = files.Select(f => SyntaxFactory.ParseSyntaxTree(System.IO.File.ReadAllText(f), parseOptions, f, Encoding.UTF8));

                        // IACT 1
                        const string asmAttr = @"[assembly: System.Runtime.CompilerServices.IgnoresAccessChecksTo(""tModLoader"")]";

                        // IACT 2
                        const string attrDecl = @"
                        namespace System.Runtime.CompilerServices {
                            [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
                            internal sealed class IgnoresAccessChecksToAttribute : System.Attribute {
                                public IgnoresAccessChecksToAttribute(string assemblyName) {}
                            }
                        }
                        ";

                        // adding IACT to the top of the file
                        src = src
                            .Prepend(SyntaxFactory.ParseSyntaxTree(asmAttr, parseOptions)) // �� ���� ������!
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
                    catch (Exception)
                    {
                        Log.Error("RoslynCompileHook error!");
                        RoslynCompileHook?.Dispose();
                        throw;
                    }

                });
            GC.SuppressFinalize(RoslynCompileHook);

        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModNetHandler.HandlePacket(reader, whoAmI);
        }
    }
}