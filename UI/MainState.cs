using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class MainState : UIState
    {
        // State
        public bool AreButtonsVisible = true;
        public bool IsDrawingTextOnButtons = true;
        public float ButtonScale = 1.0f;

        // Buttons
        public ButtonItems itemButton;
        public ButtonRefresh refreshButton;
        public ButtonConfig configButton;
        public ToggleButton toggleButton;
        public ButtonNPCs npcButton;
        public ButtonGod godButton;

        // List of all buttons
        public BaseButton[] Buttons => [itemButton, refreshButton, configButton, npcButton, godButton];

        public override void OnInitialize()
        {
            // Create the toggle button
            toggleButton = CreateButton<ToggleButton>(Assets.ButtonOn, Assets.ButtonOnNoText, "Toggle buttons\nRight click to hide", 0f);

            // Create all the other buttons
            configButton = CreateButton<ButtonConfig>(Assets.ButtonConfig, Assets.ButtonConfigNoText, "Open config", 100f);
            itemButton = CreateButton<ButtonItems>(Assets.ButtonItems, Assets.ButtonItemsNoText, "Browse all items", 200f);
            npcButton = CreateButton<ButtonNPCs>(Assets.ButtonNPC, Assets.ButtonNPCNoText, "Browse all NPCs", 300f);
            godButton = CreateButton<ButtonGod>(Assets.ButtonGod, Assets.ButtonGodNoText, "God mode", 400f);
            refreshButton = CreateButton<ButtonRefresh>(Assets.ButtonReload, Assets.ButtonReloadNoText, "Left click to reload mod \nRight click to go to mods", 500f);

            // Add all the buttons to the state
            Append(toggleButton);
            Append(itemButton);
            Append(refreshButton);
            Append(configButton);
            Append(npcButton);
            Append(godButton);

            // Initialize the setting of whether to show text on the buttons or not
            ToggleButtonTextVisibility();
        }

        // Utility to create & position any T : BaseButton
        private static T CreateButton<T>(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText, float leftOffset)
            where T : BaseButton
        {
            // We create the button via reflection
            T button = (T)Activator.CreateInstance(typeof(T), buttonImgText, buttonImgNoText, hoverText);

            // Wire up all buttons OnLeftClick to call their own HandleClick
            button.OnLeftClick += (evt, element) => button.HandleClick();

            // Set up positions, alignment, etc.
            button.Width.Set(100f, 0f);
            button.Height.Set(100f, 0f);
            button.HAlign = 0.3f; // start 30% from the left
            button.VAlign = 0.9f; // buttons at bottom
            button.Left.Set(leftOffset, 0f);
            button.RelativeLeftOffset = leftOffset;

            return button;
        }

        public void ToggleButtonTextVisibility()
        {
            toggleButton.UpdateTexture();
            itemButton.UpdateTexture();
            refreshButton.UpdateTexture();
            configButton.UpdateTexture();
            npcButton.UpdateTexture();
            godButton.UpdateTexture();
        }

        public void ToggleAllButtonsVisibility()
        {
            AreButtonsVisible = !AreButtonsVisible;
            // Log.Info("Setting buttons visibility to: " + AreButtonsVisible);

            if (!AreButtonsVisible)
            {
                RemoveChild(itemButton);
                RemoveChild(refreshButton);
                RemoveChild(configButton);
                RemoveChild(npcButton);
                RemoveChild(godButton);
            }
            else
            {
                Append(itemButton);
                Append(refreshButton);
                Append(configButton);
                Append(npcButton);
                Append(godButton);
            }
        }

        public void UpdateButtonsPositions(Vector2 anchorPosition)
        {
            foreach (BaseButton btn in Buttons)
            {
                btn.Left.Set(anchorPosition.X + btn.RelativeLeftOffset, 0f);
                btn.Top.Set(anchorPosition.Y, 0f);
            }
            Recalculate(); // Refresh layout after moving buttons.
        }
    }
}
