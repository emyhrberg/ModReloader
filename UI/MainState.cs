using System;
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

        // Buttons
        public ItemsButton itemButton;
        public RefreshButton refreshButton;
        public ConfigButton configButton;
        public ToggleButton toggleButton;

        // Panel
        private ItemsPanel itemsPanel;

        public override void OnInitialize()
        {
            // Create the toggle button
            toggleButton = CreateButton<ToggleButton>(Assets.ButtonOn, Assets.ButtonOnNoText, "Hide buttons", 0f);
            Append(toggleButton);

            // Add all others buttons
            configButton = CreateButton<ConfigButton>(Assets.ButtonConfig, Assets.ButtonConfigNoText, "Open config", 100f);
            itemButton = CreateButton<ItemsButton>(Assets.ButtonItems, Assets.ButtonItemsNoText, "Browse items", 200f);
            refreshButton = CreateButton<RefreshButton>(Assets.ButtonReload, Assets.ButtonReloadNoText, "Reload selected mod (see config)", 300f);
            Append(itemButton);
            Append(refreshButton);
            Append(configButton);

            // Pass reference of ItemsPanel to ItemsButton
            // itemButton.SetItemsPanel(itemsPanel);

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

            return button;
        }

        public void ToggleButtonTextVisibility()
        {
            toggleButton.UpdateTexture();
            itemButton.UpdateTexture();
            refreshButton.UpdateTexture();
            configButton.UpdateTexture();
        }

        public void ToggleAllButtonsVisibility()
        {
            AreButtonsVisible = !AreButtonsVisible;
            Log.Info("Setting buttons visibility to: " + AreButtonsVisible);

            if (!AreButtonsVisible)
            {
                RemoveChild(itemButton);
                RemoveChild(refreshButton);
                RemoveChild(configButton);
            }
            else
            {
                Append(itemButton);
                Append(refreshButton);
                Append(configButton);
            }
        }
    }
}
