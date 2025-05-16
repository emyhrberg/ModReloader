using AssGen;
using DragonLens.Core.Systems.ThemeSystem;
using DragonLens.Core.Systems.ToolSystem;
using DragonLens.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using ModReloader.UI.Elements.PanelElements;
using Terraria.UI;

namespace ModReloader.Common.Systems.Integrations
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensLogPanel : Tool
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

        public override void DrawIcon(SpriteBatch spriteBatch, Rectangle position)
        {
            base.DrawIcon(spriteBatch, position);
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            BasePanel logPanel = sys.mainState.logPanel;

            if (logPanel.GetActive())
            {
                GUIHelper.DrawOutline(spriteBatch, new Rectangle(position.X - 4, position.Y - 4, 46, 46), ThemeHandler.ButtonColor.InvertColor());

                Texture2D tex = DragonLensAssets.Misc.GlowAlpha.Value;
                Color color = new Color(255, 215, 150);
                color.A = 0;
                var target = new Rectangle(position.X, position.Y, 38, 38);

                spriteBatch.Draw(tex, target, color);
            }
        }
    }
}
