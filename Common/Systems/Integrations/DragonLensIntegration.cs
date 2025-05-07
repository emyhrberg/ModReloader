using DragonLens.Core.Systems.ThemeSystem;
using ModHelper.Helpers;
using Terraria.ModLoader;

namespace ModHelper.Common.Systems.Integrations
{
    /// Tries to add reload as a 'tool' to DragonLens
    [JITWhenModsEnabled("DragonLens")]
    public class DragonLensIntegration : ModSystem
    {
        public override void PostSetupContent()
        {
            /*
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
            */
            foreach (var iconProvider in ThemeHandler.allIconProvidersByType)
            {
                iconProvider.Value.icons.Add("Reload", Ass.ButtonReloadSP.Value);
            }
            foreach (var iconProvider in ThemeHandler.allIconProviders)
            {
                iconProvider.Value.icons.Add("Reload", Ass.ButtonReloadSP.Value);
            }
        }
    }
}

