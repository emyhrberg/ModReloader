using AssGen;
using DragonLens.Core.Systems.ThemeSystem;
using DragonLens.Core.Systems.ToolSystem;
using DragonLens.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.BuilderToggles;
using ModReloader.Common.Configs;
using ModReloader.UI.Elements.PanelElements;
using Terraria.UI;

namespace ModReloader.Common.Systems.Integrations.DragonLens
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensUIPanel : Tool
    {
        public override string IconKey => Helpers.Loc.Get("UIElementButton.Text");

        public override string DisplayName => Helpers.Loc.Get("UIElementButton.HoverText");

        public override string Description => GetDescription();

        private string GetDescription()
        {
            if (!Conf.C.RightClickToolOptions)
            {
                return Helpers.Loc.Get("UIElementButton.HoverDescBase");
            }

            string result = Helpers.Loc.Get("UIElementButton.HoverDescBase");
            result += $"\n{Helpers.Loc.Get("UIElementButton.HoverDescRightClick")}";
            return result;
        }

        public override void OnActivate()
        {
            if (!BuilderToggleHelper.GetActive())
            {
                LeftClickHelper.Notify();
                return;
            }

            Log.Info("DLUIPanel activated");
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

        public override bool HasRightClick => Conf.C.RightClickToolOptions;

        public override void OnRightClick()
        {
            if (!Conf.C.RightClickToolOptions)
            {
                return;
            }

            UIElementSystem elementSystem = ModContent.GetInstance<UIElementSystem>();
            if (elementSystem == null) return;
            UIElementState elementState = elementSystem.debugState;
            if (elementState == null) return;
            elementState.ToggleShowAll();
        }

        public override void DrawIcon(SpriteBatch spriteBatch, Rectangle position)
        {
            base.DrawIcon(spriteBatch, position);

            if (!BuilderToggleHelper.GetActive())
            {
                return;
            }

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
