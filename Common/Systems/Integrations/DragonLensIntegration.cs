using ModHelper.Helpers;
using Terraria.ModLoader;
using DragonLens;

namespace ModHelper.Common.Systems.Integrations
{
    /// Tries to add reload as a 'tool' to DragonLens
    public class DragonLensIntegration : ModSystem
    {
        public override void PostSetupContent()
        {
            // Check if the mod is loaded and perform actions accordingly
            if (ModLoader.TryGetMod("DragonLens", out Mod dl))
            {
                // Perform actions with the loaded mod
                Log.Info($"DragonLensIntegration is ready: {dl.Name}");

                //DragonLens.Core.Systems.ToolbarSystem.AutomaticHideOption

                //dl.Call("RegisterTool", ModContent.GetInstance<DragonLensExampleTool>());
            }
            else
            {
                Log.Warn("DragonLensIntegration failed to load (Is DragonLens enabled?)");
            }
        }
    }
}

