using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ButtonsState : UIState
    {
        private ItemsButton itemBrowserButton;
        private RefreshButton refreshButton;
        private ConfigButton configButton;

        public override void OnInitialize()
        {
            // Create each button with the shared helper
            itemBrowserButton = CreateButton<ItemsButton>(Assets.ButtonItems, "Browse items", 0f);
            refreshButton = CreateButton<RefreshButton>(Assets.ButtonRefresh, "Refresh the game", 100f);
            configButton = CreateButton<ConfigButton>(Assets.ButtonConfig, "Open config", 200f);

            // Append them to this UIState
            Append(itemBrowserButton);
            Append(refreshButton);
            Append(configButton);
        }

        // Utility to create & position any T : BaseButton
        private static T CreateButton<T>(Asset<Texture2D> texture, string hoverText, float leftOffset)
            where T : BaseButton
        {
            // We create the button via reflection
            T button = (T)Activator.CreateInstance(typeof(T), texture, hoverText);

            // Wire up all buttons onleftclick to call their own handleclick
            button.OnLeftClick += (evt, listeningElement) => button.HandleClick(evt, listeningElement);

            // Set up positions, alignment, etc.
            button.Width.Set(100f, 0f);
            button.Height.Set(100f, 0f);
            button.HAlign = 0.3f;
            button.VAlign = 0.98f;
            button.Left.Set(leftOffset, 0f);

            return button;
        }
    }
}
