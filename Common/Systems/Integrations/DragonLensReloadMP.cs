using DragonLens.Core.Systems.ToolSystem;
using ModReloader.Common.Configs;
using ModReloader.Helpers;

namespace ModReloader.Common.Systems.Integrations
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensReloadMP : Tool
    {
        public override string IconKey => "ReloadMP"; // no need for localization. this is just to find the asset for the icon

        public override string DisplayName => Helpers.Loc.Get("ReloadMPButton.Text");

        public override string Description => GetDescription();

        private string GetDescription()
        {
            if (!Conf.C.RightClickToolOptions)
            {
                return Helpers.Loc.Get("ReloadMPButton.HoverText", string.Join(", ", Conf.C.ModsToReload));
            }

            string result = $"{Helpers.Loc.Get("ReloadMPButton.HoverText", string.Join(", ", Conf.C.ModsToReload))}\n{Helpers.Loc.Get("ReloadMPButton.HoverDescRightClick")}";
            //result += $"\n{Helpers.LocalizationHelper.GetText("ReloadButton.HoverDescRightClick")}";
            return result;
        }

        public override bool HasRightClick => Conf.C.RightClickToolOptions;

        public override async void OnActivate()
        {
            await ReloadUtilities.MultiPlayerMainReload();
        }
    }
}
