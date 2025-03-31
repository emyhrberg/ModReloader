using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Systems.ILHooks;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class FocusText : UIText
    {
        private bool Active = true;

        public FocusText(string text, float textScale = 1.0f, bool large = false) : base(text, textScale, large)
        {
            TextColor = Color.White;
            VAlign = 0.01f;
            HAlign = 0.65f;

            // Arbitrary size, should use ChatManager.GetStringSize() instead
            Width.Set(200, 0);
            Height.Set(20, 0); // 20 * 3 for 3 lines of text
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            //always call base! otherwise IsMouseHovering wont work
            base.MouseOver(evt);
            TextColor = Color.Yellow;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            //always call base! otherwise IsMouseHovering wont work
            base.MouseOut(evt);
            TextColor = Color.White;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            FocusHook.KeepRunning = !FocusHook.KeepRunning;

            if (FocusHook.KeepRunning)
            {
                SetText("Keep Game Running: Enabled", 1.0f, false);
            }
            else
            {
                SetText("Keep Game Running: Disabled", 1.0f, false);
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            Active = !Active;

            if (Active)
            {
                ChatHelper.NewText("Showing keep game running text", Color.White);
            }
            else
            {
                ChatHelper.NewText("Hiding keep game running text", Color.White);
            }
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
                UICommon.TooltipMouseText("Right click to hide");
            }
        }
    }
}