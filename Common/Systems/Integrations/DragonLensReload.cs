using DragonLens.Core.Systems.ToolSystem;
using ModReloader.Common.Configs;
using ModReloader.Helpers;

namespace ModReloader.Common.Systems.Integrations
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensReload : Tool
    {
        public override string IconKey => "Reload";

        public override string DisplayName => "Reload";

        public override string Description => $"Reloads {string.Join(", ", Conf.C.ModsToReload)}\n" +
            $"Right click to reload mods without building any";

        public override bool HasRightClick => true;
        public override async void OnActivate()
        {
            await ReloadUtilities.SinglePlayerReload();
        }

        public override async void OnRightClick()
        {
            ReloadUtilities.forceJustReload = true;
            await ReloadUtilities.SinglePlayerReload();
        }
    }
}
