using DragonLens.Core.Systems.ToolSystem;
using ModReloader.Core.Features.Reload;

namespace ModReloader.Common.Integrations.DragonLens
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensReloadMP : Tool
    {
        public override string IconKey => "ReloadMP"; // no need for localization. this is just to find the asset for the icon

        public override string DisplayName => Loc.Get("ReloadMPButton.Text");

        public override string Description => GetDescription();

        private string GetDescription()
        {
            string result = $"{Loc.Get("ReloadMPButton.HoverText", string.Join(", ", Conf.C.ModsToReload))}\n{Loc.Get("ReloadMPButton.HoverDescRightClick")}";
            //result += $"\n{Helpers.LocalizationHelper.GetText("ReloadButton.HoverDescRightClick")}";
            return result;
        }

        public override async void OnActivate()
        {
            await ReloadUtilities.MultiPlayerMainReload();
        }
    }
}
