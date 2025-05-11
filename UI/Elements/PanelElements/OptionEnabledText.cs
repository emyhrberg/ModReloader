using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using static ModHelper.UI.Elements.PanelElements.OptionElement;

namespace ModHelper.UI.Elements.PanelElements
{
    public class OptionEnabledText : UIText
    {
        private EnabledState state;

        public OptionEnabledText(string text) : base(text)
        {
            TextColor = ColorHelper.CalamityRed;

            // Position: Centered vertically, 65 pixels from the right
            VAlign = 0.5f;
            float def = -65f;
            Left.Set(def, 1f);
        }

        public void SetTextState(EnabledState state)
        {
            this.state = state;
            TextColor = state == EnabledState.Enabled ? Color.Green : ColorHelper.CalamityRed;
            SetText(state == EnabledState.Enabled ? "Enabled" : "Disabled");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // TODO Draw a rectangle behind the text
            DrawHelper.DrawDebugHitbox(this, ColorHelper.SuperDarkBluePanel, scale: 1.08f);

            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                if (state == EnabledState.Enabled)
                {
                    UICommon.TooltipMouseText("Click to disable");
                }
                else
                {
                    UICommon.TooltipMouseText("Click to enable");
                }
            }
        }
    }
}