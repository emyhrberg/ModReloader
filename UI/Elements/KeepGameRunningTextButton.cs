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
    public class KeepGameRunningTextButton : UIText
    {
        private string text;
        private float scale;
        private bool Active = true;

        public KeepGameRunningTextButton(string text, float scale = 0.5f, bool large = true) : base(text, scale, large)
        {
            this.text = text;
            this.scale = 1f;
            TextColor = Color.White;
            VAlign = 0.01f;
            HAlign = 0.5f;
            Width.Set(200, 0);
            Height.Set(20, 0);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            TextColor = Color.Yellow;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);

            TextColor = Color.White;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            KeepGameRunning.KeepRunning = !KeepGameRunning.KeepRunning;

            if (KeepGameRunning.KeepRunning)
            {
                SetText("Keep Game Running: ON");
                ChatHelper.NewText("Keep Game Running: ON", Color.Green);
                Log.Info("Keep Game Running: ON");
            }
            else
            {
                SetText("Keep Game Running: OFF");
                ChatHelper.NewText("Keep Game Running: OFF", new Color(226, 57, 39));
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

            Conf.C.ShowGameKeepRunningText = !Conf.C.ShowGameKeepRunningText;
            Conf.ForceSaveConfig(Conf.C);

            if (Active)
            {
                ChatHelper.NewText("'Keep Game Running' text hidden.", new Color(226, 57, 39));
            }
            else
            {
                ChatHelper.NewText("'Keep Game Running' text shown.", Color.Green);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Keep updating the text no matter what so we can toggle even if its hidden
            // if (!Active)
            // {
            // return;
            // }

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true; // disable item use if the button is hovered
            }

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
                UICommon.TooltipMouseText("Left click to toggle option \nRight click to toggle visibility");
            }
        }
    }

}