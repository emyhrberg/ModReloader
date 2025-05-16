using AssGen;
using DragonLens.Core.Systems.ThemeSystem;
using DragonLens.Core.Systems.ToolSystem;
using DragonLens.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using ModReloader.UI.Elements.PanelElements;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.Common.Systems.Integrations
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensUIPanel : Tool
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

        public override void DrawIcon(SpriteBatch spriteBatch, Rectangle position)
        {
            base.DrawIcon(spriteBatch, position);
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            BasePanel uiPanel = sys.mainState.uiElementPanel;

            if (uiPanel.GetActive())
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
