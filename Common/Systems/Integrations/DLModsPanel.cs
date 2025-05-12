using DragonLens.Core.Systems.ToolSystem;
using ModHelper.Helpers;
using ModHelper.UI.Elements.PanelElements;
using Terraria.UI;

namespace ModHelper.Common.Systems.Integrations
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DLModsPanel : Tool
    {
        public override string IconKey => "Mods";

        public override string DisplayName => "Mods List";

        public override string Description => $"Enable or disable mods";

        public override void OnActivate()
        {
            Log.Info("DLModsPanel activated");
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            BasePanel modsPanel = sys.mainState.modsPanel;

            if (modsPanel is null)
            {
                Log.Error("ModsPanel is null");
                return;
            }

            if (modsPanel.GetActive())
            {
                modsPanel.SetActive(false);
            }
            else
            {
                modsPanel.SetActive(true);

                // bring to front …
                if (modsPanel.Parent is not null)
                {
                    UIElement parent = modsPanel.Parent;
                    modsPanel.Remove();
                    parent.Append(modsPanel);
                }
            }
        }
    }
}
