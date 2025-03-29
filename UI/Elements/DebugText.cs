using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Common.Systems;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class DebugText : UIText
    {
        private bool Active = true;

        public DebugText(string text, float scale = 0.3f, bool large = true) : base(text, scale, large)
        {
            TextColor = Color.White;
            VAlign = 0.9f;
            HAlign = 0.02f;

            // Arbitrary size, should use ChatManager.GetStringSize() instead
            Width.Set(200, 0);
            Height.Set(20, 0);
        }

        // public override void MouseOver(UIMouseEvent evt)
        // {
        //     base.MouseOver(evt);

        //     TextColor = Color.Yellow;
        // }

        // public override void MouseOut(UIMouseEvent evt)
        // {
        //     base.MouseOut(evt);

        //     TextColor = Color.White;
        // }

        public override void RightClick(UIMouseEvent evt)
        {
            // if (!Active)
            // {
            //     return;
            // }
            // base.RightClick(evt);

            Active = !Active;

            ChatHelper.NewText("Hiding the 'Keep Game Running' text. Open config to toggle show again.", Color.Green);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // update the text to show playername, whoAmI, and FPS
            string playerName = Main.LocalPlayer.name;
            int whoAmI = Main.LocalPlayer.whoAmI;
            int fps = Main.frameRate;
            int ups = Main.updateRate;
            string text = $"Player: {playerName}" +
                $"\nWhoAmI: {whoAmI}" +
                $"\n{fps}fps {ups}ups ({Main.upTimerMax.ToString("0.0")}ms)";

            SetText(text);
            // Log.Info("Setting text: " + text);
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
                UICommon.TooltipMouseText("Right click to toggle");
            }
        }
    }

}