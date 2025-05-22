using System;
using System.Reflection;
using DragonLens.Content.GUI;
using DragonLens.Content.Tools;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace ModReloader.Common.Systems.Integrations.DragonLens
{
    // Adds a Mod Helper filter to tool browser
    // References:
    // https://github.com/ScalarVector1/DragonLens/blob/master/Content/Tools/Spawners/BrowserTool.cs
    // https://github.com/ScalarVector1/DragonLens/blob/master/Content/Tools/Spawners/NPCSpawner.cs#L85
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensToolFilter : ModSystem
    {
        public override void PostSetupContent()
        {
            MethodInfo targetMethod = typeof(ToolBrowser).GetMethod("PopulateGrid");
            if (targetMethod == null)
            {
                Log.Error("SetupFilters is null!");
                return;
            }

            MonoModHooks.Modify(targetMethod, ILSetupFilters);
        }

        private void ILSetupFilters(ILContext il)
        {
            var c = new ILCursor(il);

            // Find where filters are initialized in ToolBrowser
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchNewobj<FilterPanel>()
            ))
            {
                // Add filter after FilterPanel creation
                c.Emit(OpCodes.Dup); // Duplicate FilterPanel reference
                c.Emit(OpCodes.Ldarg_0); // Load ToolBrowser instance
                c.EmitDelegate((FilterPanel filters, ToolBrowser self) =>
                {
                    filters.AddFilter(new ModReloaderToolFilter(Mod));
                });
            }
        }
    }
}