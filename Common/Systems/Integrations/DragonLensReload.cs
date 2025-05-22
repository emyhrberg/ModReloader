using DragonLens.Core.Systems.ToolSystem;
using ModReloader.Common.Configs;
using ModReloader.Helpers;

namespace ModReloader.Common.Systems.Integrations
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensReload : Tool
    {
        public override string IconKey => "Reload"; // icon for assets only

        public override string DisplayName => Helpers.Loc.Get("ReloadButton.Text");

        public override string Description => GetDescription();

        private string GetDescription()
        {
            if (!Conf.C.RightClickToolOptions)
            {
                return Helpers.Loc.Get("ReloadButton.HoverText", string.Join(", ", Conf.C.ModsToReload));
            }

            string result = $"{Helpers.Loc.Get("ReloadButton.HoverText", string.Join(", ", Conf.C.ModsToReload))}\n{Helpers.Loc.Get("ReloadButton.HoverDescRightClick")}";
            //result += $"\n{Helpers.LocalizationHelper.GetText("ReloadButton.HoverDescRightClick")}";
            return result;
        }

        public override bool HasRightClick => Conf.C.RightClickToolOptions;

        public override async void OnActivate()
        {
            await ReloadUtilities.SinglePlayerReload();
        }

        public override async void OnRightClick()
        {
            if (!Conf.C.RightClickToolOptions)
            {
                return;
            }

            ReloadUtilities.forceJustReload = true;
            await ReloadUtilities.SinglePlayerReload();
        }
    }
}
