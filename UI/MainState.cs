using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class MainState : UIState
    {
        // State
        public bool AreButtonsVisible = true;
        public bool IsDrawingTextOnButtons = true;
        public float ButtonScale = 1.0f;
        private Config c;

        // Buttons
        public ItemsButton itemButton;
        public RefreshButton refreshButton;
        public ConfigButton configButton;
        public ToggleButton toggleButton;
        public NPCsButton npcButton;
        public GodButton godButton;

        // List of all buttons
        public BaseButton[] Buttons => [toggleButton, itemButton, refreshButton, configButton, npcButton, godButton];

        public override void OnInitialize()
        {
            // Init some stuff
            c = ModContent.GetInstance<Config>();

            // Create the toggle button
            toggleButton = CreateButton<ToggleButton>(Assets.ButtonOn, Assets.ButtonOnNoText, "Toggle buttons\nRight click to hide", 0f);

            // Create all the other buttons
            configButton = CreateButton<ConfigButton>(Assets.ButtonConfig, Assets.ButtonConfigNoText, "Open config", 100f);
            itemButton = CreateButton<ItemsButton>(Assets.ButtonItems, Assets.ButtonItemsNoText, "Browse all items", 200f);
            npcButton = CreateButton<NPCsButton>(Assets.ButtonNPC, Assets.ButtonNPCNoText, "Browse all NPCs", 300f);
            godButton = CreateButton<GodButton>(Assets.ButtonGod, Assets.ButtonGodNoText, "God mode", 400f);
            refreshButton = CreateButton<RefreshButton>(Assets.ButtonReload, Assets.ButtonReloadNoText, "Reload mod (see Config) \nRight click to go to mods", 500f);

            // Add all the buttons to the state
            Append(toggleButton);
            Append(itemButton);
            Append(refreshButton);
            Append(configButton);
            Append(npcButton);
            Append(godButton);

            // Initialize the setting of whether to show text on the buttons or not
            UpdateAllButtonsTexture();
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

        public void UpdateAllButtonsTexture()
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
            // if (!AreButtonsVisible)
            // {
            //     RemoveChild(itemButton);
            //     RemoveChild(refreshButton);
            //     RemoveChild(configButton);
            //     RemoveChild(npcButton);
            //     RemoveChild(godButton);
            // }
            // else
            // {
            //     Append(itemButton);
            //     Append(refreshButton);
            //     Append(configButton);
            //     Append(npcButton);
            //     Append(godButton);
            // }
        }

        public void UpdateButtonsPositions(Vector2 anchorPosition)
        {
            foreach (BaseButton btn in Buttons)
            {
                btn.Left.Set(anchorPosition.X + btn.RelativeLeftOffset * ButtonScale, 0f);
                btn.Top.Set(anchorPosition.Y, 0f);
            }
            Recalculate(); // Refresh layout after moving buttons.
        }

        public override void Update(GameTime gameTime)
        {
            if (c != null && c.General.OnlyShowWhenInventoryOpen)
            {
                if (!Main.playerInventory)
                {
                    // Inventory is closed – remove all non-toggle buttons if they are present.
                    foreach (var btn in Buttons)
                    {
                        if (Children.Contains(btn))
                        {
                            RemoveChild(btn);
                        }
                    }
                }
                else
                {
                    // Inventory is open – make sure all non-toggle buttons are appended.
                    foreach (var btn in Buttons)
                    {
                        if (!Children.Contains(btn))
                        {
                            Append(btn);
                            Log.Info("Appending button: " + btn.GetType().Name); // is only called once, yay
                        }
                    }
                }
            }
            else
            {
                // Config setting is disabled – always show all buttons.
                foreach (var btn in Buttons)
                {
                    if (!Children.Contains(btn))
                    {
                        Append(btn);
                    }
                }
            }

            base.Update(gameTime);
        }
    }
}
