using DragonLens.Core.Systems.ToolSystem;
using ModReloader.Core.Features.Reload;

namespace ModReloader.Common.Integrations.DragonLens
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensReload : Tool
    {
        public override string IconKey => "Reload"; // icon for assets only

        public override string DisplayName => Loc.Get("ReloadButton.Text");

        public override string Description => GetDescription();

        private string GetDescription()
        {
            string result = $"{Loc.Get("ReloadButton.HoverText", string.Join(", ", Conf.C.ModsToReload))}\n{Loc.Get("ReloadButton.HoverDescRightClick")}";
            //result += $"\n{Helpers.LocalizationHelper.GetText("ReloadButton.HoverDescRightClick")}";
            return result;
        }

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
