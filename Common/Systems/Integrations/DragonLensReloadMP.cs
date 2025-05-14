using DragonLens.Core.Systems.ToolSystem;
using ModHelper.Common.Configs;
using ModHelper.Helpers;

namespace ModHelper.Common.Systems.Integrations
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensReloadMP : Tool
    {
        public override string IconKey => "ReloadMP";

        public override string DisplayName => "Reload MP";

        public override string Description => $"Reloads {string.Join(", ", Conf.C.ModsToReload)}";

        public override async void OnActivate()
        {
            await ReloadUtilities.MultiPlayerMainReload();
        }
    }
}
