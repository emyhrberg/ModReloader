using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using static ModHelper.UI.Elements.Option;

namespace ModHelper.UI.Elements
{
    public class ModEnabledText : UIText
    {
        private State state;
        private Color red = new(226, 57, 39);
        private string internalName;

        public ModEnabledText(string text, string internalModName) : base(text)
        {
            this.internalName = internalModName;
            // text and size and position
            // enabledText.ShadowColor = new Color(226, 57, 39); // TODO change background color to this, shadowcolor is not it.
            float def = -65f;
            TextColor = Color.Green;
            VAlign = 0.5f;
            Left.Set(def, 1f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Toggle state
            state = state == State.Enabled ? State.Disabled : State.Enabled;
            this.SetTextState(state);
            bool enabled = state == State.Enabled;

            Log.Info("Setting mod enabled: " + internalName + " to " + enabled);

            // Use reflection to call SetModEnabled on internalModName
            var setModEnabled = typeof(ModLoader).GetMethod("SetModEnabled", BindingFlags.NonPublic | BindingFlags.Static);
            setModEnabled?.Invoke(null, [internalName, enabled]);
        }

        public void SetTextState(State state)
        {
            this.state = state;
            TextColor = state == State.Enabled ? Color.Green : red;
            SetText(state == State.Enabled ? "Enabled" : "Disabled");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                if (state == State.Enabled)
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