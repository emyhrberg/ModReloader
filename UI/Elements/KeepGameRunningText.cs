using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Systems;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class KeepGameRunningText : UIText
    {
        private bool Active = true;

        public KeepGameRunningText(string text, float textScale = 1.0f, bool large = false) : base(text, textScale, large)
        {
            TextColor = Color.White;
            VAlign = 0.78f;
            HAlign = 0.02f;

            // Arbitrary size, should use ChatManager.GetStringSize() instead
            Width.Set(200, 0);
            Height.Set(20 * 1, 0); // 20 * 3 for 3 lines of text
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            TextColor = Color.Yellow;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            TextColor = Color.White;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            KeepGameRunning.KeepRunning = !KeepGameRunning.KeepRunning;
        }

        public override void RightClick(UIMouseEvent evt)
        {
            Active = !Active;

            if (Active)
            {
                ChatHelper.NewText("Showing debug text", Color.White);
            }
            else
            {
                ChatHelper.NewText("Hiding debug text", Color.White);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            string onOff = KeepGameRunning.KeepRunning ? "ON" : "OFF";
            string text = $"Keep Game Running: {onOff}\n";
            SetText(text);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
            {
                return;
            }

            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true; // disable item use if the button is hovered
                UICommon.TooltipMouseText("Right click to toggle visibility");
            }
        }
    }
}