using System;
using AssGen;
using DragonLens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper.Common.Systems.Integrations
{
    /// Tries to add reload as a 'tool' to DragonLens
    [JITWhenModsEnabled("DragonLens")]
    public class DragonLensIntegration : ModSystem
    {
        public override void PostSetupContent()
        {
            // Check if the mod is loaded and perform actions accordingly
            if (ModLoader.TryGetMod("DragonLens", out Mod dl))
            {
                Action click = async () => await ReloadUtilities.SinglePlayerReload();

                dl.Call("AddAPITool",
                        "Reload",        // internal key, must be unique
                        Ass.ButtonReloadSP,
                        click);
                Log.Info("DLintegration was run!");
            }
            else
            {
                Log.Warn("DragonLensIntegration failed to load (Is DragonLens enabled?)");
            }
        }
    }
}

