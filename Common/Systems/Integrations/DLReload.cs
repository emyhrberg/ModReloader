using DragonLens.Core.Systems.ToolSystem;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using Terraria.ModLoader;

namespace ModHelper.Common.Systems.Integrations
{
    [ExtendsFromMod("DragonLens")]
    public class DLReload : Tool
    {
        public override string IconKey => "Reload";

        public override string DisplayName => "Reload";

        public override string Description => $"Reloads {string.Join(", ", Conf.C.ModsToReload)}";

        public override async void OnActivate()
        {
            await ReloadUtilities.SinglePlayerReload();
        }
    }
}
