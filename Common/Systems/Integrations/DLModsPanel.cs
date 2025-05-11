using DragonLens.Core.Systems.ToolSystem;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ModHelper.UI.Elements.PanelElements.ModElements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.Common.Systems.Integrations
{
    [ExtendsFromMod("DragonLens")]
    public class DLModsPanel : Tool
    {
        public override string IconKey => "ModsPanel";

        public override string DisplayName => "Mods Panel";

        public override string Description => $"Enable or disable mods";

        public override void OnActivate()
        {
            Log.Info("DLModsPanel activated");
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys == null || sys.mainState == null)
            {
                Log.Error("MainState is null, cannot open mods panel");
                return;
            }

            if (!sys.mainState.AreButtonsShowing) return;
            ModsPanel panel = sys.mainState.modsPanel;

            bool nowOpen = !panel.GetActive();
            panel.SetActive(nowOpen);

            if (nowOpen && panel.Parent is UIElement parent)
            {
                panel.Remove();
                parent.Append(panel);          // move to top layer
            }

            // (optional) highlight your in‑game button
            sys.mainState.modsButton.ParentActive = nowOpen;
        }
    }
}
