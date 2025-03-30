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
    public class KeepGameRunningText : UIText
    {
        private bool Active = true;

        public KeepGameRunningText(string text, float textScale = 1.0f, bool large = false) : base(text, textScale, large)
        {
            TextColor = Color.White;
            VAlign = 0.85f;
            HAlign = 0.02f;
            Width.Set(200, 0);
            Height.Set(20, 0);
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

            if (KeepGameRunning.KeepRunning)
            {
                SetText("Keep Game Running: ON");
                Log.Info("Keep Game Running: ON");
            }
            else
            {
                SetText("Keep Game Running: OFF");
                Log.Info("Keep Game Running: OFF");
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            if (!Active)
            {
                return;
            }

            base.RightClick(evt);

            Active = !Active;

            // Conf.C.ShowGameKeepRunningText = !Conf.C.ShowGameKeepRunningText;
            // Conf.ForceSaveConfig(Conf.C);

            if (!Active)
            {
                ChatHelper.NewText("Hiding keep game running text");
            }
            else
            {
                ChatHelper.NewText("Showing keep game running text");
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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
                UICommon.TooltipMouseText("Left click to toggle option \nRight click to toggle visibility");
            }
        }
    }

}