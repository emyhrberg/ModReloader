using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    // Contains:
    // Icon image
    // Mod name
    // Enabled text
    public class Option : UIPanel
    {
        public string text;
        private OptionEnabledText enabledText;
        private OptionIcon optionIcon;
        private OptionTitleText optionTitleText;
        private Action leftClick;
        private Action rightClick;

        public enum State
        {
            Enabled,
            Disabled
        }
        private State state = State.Disabled;

        public Option(Action leftClick, string text, string hover = "", Action rightClick = null)
        {
            this.leftClick = leftClick;
            this.text = text;

            // size and position
            Width.Set(-35f, 1f);
            Height.Set(30, 0);
            Left.Set(5, 0);

            // mod icon
            // passing a temp icon because above doesnt work
            // maybe because path its not loaded yet.
            Texture2D temp = TextureAssets.MagicPixel.Value;
            Texture2D temp2 = Main.Assets.Request<Texture2D>("Images/UI/DefaultResourcePackIcon", AssetRequestMode.ImmediateLoad).Value;

            optionIcon = new(temp2);
            Append(optionIcon);

            // mod name
            optionTitleText = new(text, hover);
            Append(optionTitleText);

            // enabled text
            enabledText = new("Disabled");
            Append(enabledText);
        }

        public void SetState(State state)
        {
            this.state = state;
            enabledText.SetTextState(state);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);

            rightClick?.Invoke();
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            state = state == State.Disabled ? State.Enabled : State.Disabled;
            enabledText.SetTextState(state);
            leftClick.Invoke();
        }
    }
}