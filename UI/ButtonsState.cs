using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ButtonsState : UIState
    {
        // State
        // set to true by default
        public bool AreButtonsVisible { get; private set; } = true;

        // Buttons
        public ItemsButton itemBrowserButton;
        public RefreshButton refreshButton;
        public ConfigButton configButton;
        public ToggleButton toggleButton;

        public override void OnInitialize()
        {
            // Create each button with the shared helper
            toggleButton = CreateButton<ToggleButton>(Assets.ToggleButtonOn, "Toggle visibility of all buttons", 0f);
            itemBrowserButton = CreateButton<ItemsButton>(Assets.ButtonItems, "Browse items", 100f);
            refreshButton = CreateButton<RefreshButton>(Assets.ButtonRefresh, "Refresh selected mod (see config)", 200f);
            configButton = CreateButton<ConfigButton>(Assets.ButtonConfig, "Open config", 300f);

            Append(toggleButton);

            if (AreButtonsVisible)
            {
                Append(itemBrowserButton);
                Append(refreshButton);
                Append(configButton);
            }
        }

        // Utility to create & position any T : BaseButton
        private static T CreateButton<T>(Asset<Texture2D> texture, string hoverText, float leftOffset)
            where T : BaseButton
        {
            // We create the button via reflection
            T button = (T)Activator.CreateInstance(typeof(T), texture, hoverText);

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

        public void ToggleAllButtonsVisibility()
        {
            AreButtonsVisible = !AreButtonsVisible;
            Log.Info("Setting buttons visibility to: " + AreButtonsVisible);

            if (!AreButtonsVisible)
            {
                RemoveChild(itemBrowserButton);
                RemoveChild(refreshButton);
                RemoveChild(configButton);
            }
            else
            {
                Append(itemBrowserButton);
                Append(refreshButton);
                Append(configButton);
            }
        }
    }
}
