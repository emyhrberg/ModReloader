using Microsoft.CodeAnalysis;
using ModHelper.Helpers;
using ModHelper.PacketHandlers;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.IO;
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
            string path,
            List<string> references,
            string[] assemblyReferences,
            string[] sourceFiles,
            bool isDebug,
            out byte[] assemblyBytes,
            out byte[] pdbBytes);

        public override void Load()
        {
            Instance = this;
            Hook RoslynCompileHook = null;
            RoslynCompileHook = new Hook(((RoslynCompileDelegate)ModCompile.RoslynCompile).Method, 
                (RoslynCompileDelegate orig, string path, List<string> references, string[] assemblyReferences, string[] sourceFiles, bool isDebug, out byte[] assemblyBytes, out byte[] pdbBytes) =>
                {
                    Diagnostic[] r = orig(path, references, assemblyReferences, sourceFiles, isDebug, out assemblyBytes, out pdbBytes);
                    Log.Info("RoslynCompileHook called");
                    RoslynCompileHook?.Dispose();
                    return r;
                    
                });
            GC.SuppressFinalize(RoslynCompileHook);
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModNetHandler.HandlePacket(reader, whoAmI);
        }
    }
}