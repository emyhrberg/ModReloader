using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ModHelper.UI.Buttons;
using ReLogic.Content;
using Terraria.UI;

namespace ModHelper.UI
{
    public class MainState : UIState
    {
        // Buttons
        private List<BaseButton> AllButtons = [];
        public float ButtonSize;
        private float offset = 0; // offset for the button position

        #region Constructor
        // MainState Constructor. This is where we create all the buttons and set up their positions.
        public MainState() => AddEverything();

        public void AddEverything()
        {
            // Set the button size
            offset = 0;
            ButtonSize = (Conf.C == null) ? 70f : Conf.C.ButtonSize;

            // Add buttons
            AddButton<ReloadSPButton>(Ass.ButtonReloadSP, "Reload", "Singleplayer Reload", "Reloads the selected mod");
            AddButton<ReloadMPButton>(Ass.ButtonReloadMP, "Reload", "Multiplayer Reload", "Reloads the selected mod");
            AddButton<ConfigButton>(Ass.ButtonConfig, "Config", "Open config", "This button is only here for testing purposes");
            AddButton<TestButton>(Ass.CollapseUp, "Test", "Testing", "This button is only here for testing purposes");
        }
        #endregion

        private T AddButton<T>(Asset<Texture2D> spritesheet = null, string buttonText = null, string hoverText = null, string hoverTextDescription = "") where T : BaseButton
        {
            // Create a new button using reflection
            T button = (T)Activator.CreateInstance(typeof(T), spritesheet, buttonText, hoverText, hoverTextDescription);

            // Button dimensions
            float size = ButtonSize;
            button.Width.Set(size, 0f);
            button.Height.Set(size, 0f);
            button.MaxWidth = new StyleDimension(size, 0);
            button.MaxHeight = new StyleDimension(size, 0);
            button.MinWidth = new StyleDimension(size, 0);
            button.MinHeight = new StyleDimension(size, 0);

            // set x pos with offset
            button.Left.Set(pixels: offset, precent: 0f);
            offset += ButtonSize;

            // custom left pos. override default
            // convert vector2 to valign and halign
            // buttonposition is from 0 to 1
            button.VAlign = Conf.C.ButtonPosition.Y;
            button.HAlign = Conf.C.ButtonPosition.X;

            button.Recalculate();

            // Add the button to the list of all buttons and append it to the MainState
            AllButtons.Add(button);
            Append(button);
            return button;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw everything in the main state
            base.Draw(spriteBatch);

            // Draw tooltip
            foreach (var button in AllButtons)
            {
                if (button.IsMouseHovering && button.HoverText != null)
                {
                    // Draw the tooltip
                    DrawHelper.DrawTooltipPanel(button, button.HoverText, button.HoverTextDescription); // Draw the tooltip panel
                }
            }

            // if (reloadSPButton.IsMouseHovering && reloadSPButton.HoverText != null)
            // {
            //     // Draw the tooltip
            //     DrawHelper.DrawTooltipPanel(reloadSPButton, reloadSPButton.HoverText, reloadSPButton.HoverTextDescription); // Draw the tooltip panel
            // }
            // else if (reloadMPButton.IsMouseHovering && reloadMPButton.HoverText != null)
            // {
            //     // Draw the tooltip
            //     DrawHelper.DrawTooltipPanel(reloadMPButton, reloadMPButton.HoverText, reloadMPButton.HoverTextDescription); // Draw the tooltip panel
            // }
        }
    }
}