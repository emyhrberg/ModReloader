using DragonLens.Core.Systems.ToolSystem;
using ModHelper.Helpers;
using ModHelper.UI.Elements.PanelElements;
using Terraria.UI;

namespace ModHelper.Common.Systems.Integrations
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DLUIPanel : Tool
    {
        public override string IconKey => "UI";

        public override string DisplayName => "UIElement Hitboxes";

        public override string Description => $"Toggle UIElement hitboxes";

        public override void OnActivate()
        {
            Log.Info("DLModsPanel activated");
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            BasePanel uiPanel = sys.mainState.uiElementPanel;

            if (uiPanel is null)
            {
                Log.Error("UIPanel is null");
                return;
            }

            if (uiPanel.GetActive())
            {
                uiPanel.SetActive(false);
            }
            else
            {
                uiPanel.SetActive(true);

                // bring to front …
                if (uiPanel.Parent is not null)
                {
                    UIElement parent = uiPanel.Parent;
                    uiPanel.Remove();
                    parent.Append(uiPanel);
                }
            }
        }
    }
}
