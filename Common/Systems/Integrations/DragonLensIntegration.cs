using DragonLens.Core.Systems.ThemeSystem;
using DragonLens.Core.Systems.ToolbarSystem;
using ModHelper.Helpers;
using Terraria.ModLoader;

namespace ModHelper.Common.Systems.Integrations
{
    /// Tries to add reload as a 'tool' to DragonLens
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensIntegration : ModSystem
    {
        public override void PostSetupContent()
        {
            AddIcons();
        }

        private static void AddIcons()
        {
            foreach (var provider in ThemeHandler.allIconProviders.Values)
            {
                // assign (overwrites if the key exists already) – never throws
                provider.icons["Reload"] = Ass.ButtonReloadSP.Value;
                provider.icons["Mods"] = Ass.ButtonModsHeros.Value;
                provider.icons["UI"] = Ass.ButtonUIHeros.Value;
                provider.icons["Log"] = Ass.ButtonLogHeros.Value;
                provider.icons["ReloadMP"] = Ass.ButtonReloadMP.Value;
            }

            // rebuild toolbars *after* icons have been injected
            ModContent.GetInstance<ToolbarHandler>().OnModLoad();
        }
    }
}

