using DragonLens.Core.Systems.ToolSystem;
using ModHelper.Helpers;
using ModHelper.UI.Elements.PanelElements;
using Terraria.UI;

namespace ModHelper.Common.Systems.Integrations
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DLLogPanel : Tool
    {
        public override string IconKey => "Log";

        public override string DisplayName => "Log Options";

        public override string Description => $"Change log options here";

        public override void OnActivate()
        {
            Log.Info("DLModsPanel activated");
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            BasePanel logPanel = sys.mainState.logPanel;

            if (logPanel is null)
            {
                Log.Error("LogPanel is null");
                return;
            }

            if (logPanel.GetActive())
            {
                logPanel.SetActive(false);
            }
            else
            {
                logPanel.SetActive(true);

                // bring to front …
                if (logPanel.Parent is not null)
                {
                    UIElement parent = logPanel.Parent;
                    logPanel.Remove();
                    parent.Append(logPanel);
                }
            }
        }
    }
}
