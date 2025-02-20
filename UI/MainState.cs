using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public bool AreButtonsShowing = true;
        public float ButtonSize = 70f;

        // Buttons
        public ItemBrowserButton itemButton;
        public RefreshButton refreshButton;
        public ConfigButton configButton;
        public ToggleButton toggleButton;
        public NPCBrowserButton npcButton;
        public GodButton godButton;
        public FastButton fastButton;
        public LogButton logButton;
        public HitboxButton hitboxButton;
        public DebugUIButton uiDebugButton;
        public ReloadSingleplayerButton reloadSingleplayerButton;
        public ReloadMultiplayerButton reloadMultiplayerButton;
        public MinimalItemBrowserButton minimalItemButton;

        // List of all buttons
        public HashSet<BaseButton> AllButtons;

        // MainState Constructor
        // This is where we create all the buttons and set up their positions.
        public MainState()
        {
            // Initialize config reference
            Config c = ModContent.GetInstance<Config>();

            // Create all the buttons first.
            toggleButton = CreateButton<ToggleButton>(Assets.ButtonOn, "Toggle all buttons");
            configButton = CreateButton<ConfigButton>(Assets.ButtonConfig, "Open config");
            refreshButton = CreateButton<RefreshButton>(Assets.ButtonReload, "Reload the selected mod");
            itemButton = CreateButton<ItemBrowserButton>(Assets.ButtonItems, "Open item spawner\nContains all items in the game");
            npcButton = CreateButton<NPCBrowserButton>(Assets.ButtonNPC, "Open NPC spawner\nContains town NPCs, enemies, and bosses");
            godButton = CreateButton<GodButton>(Assets.ButtonGodOn, "Toggle player god mode");
            fastButton = CreateButton<FastButton>(Assets.ButtonFastOn, "Toggle player fast mode");
            hitboxButton = CreateButton<HitboxButton>(Assets.ButtonHitboxOn, "Show player, enemy, and projectile hitboxes");
            uiDebugButton = CreateButton<DebugUIButton>(Assets.ButtonUIDebug, "Show all UIElements from mods");
            logButton = CreateButton<LogButton>(Assets.ButtonLog, "Open client.log");
            reloadSingleplayerButton = CreateButton<ReloadSingleplayerButton>(Assets.ButtonReloadSingleplayer, "Reload singleplayer");
            reloadMultiplayerButton = CreateButton<ReloadMultiplayerButton>(Assets.ButtonReloadMultiplayer, "Reload multiplayer");
            minimalItemButton = CreateButton<MinimalItemBrowserButton>(Assets.ButtonItems, "Open minimal item spawner\nContains all items in the game");

            // Append buttons conditionally based on disable flags.
            // Build list of buttons to append.
            var buttons = new List<BaseButton> { toggleButton }; // always include toggleButton

            if (!c.DisableConfig) buttons.Add(configButton);
            if (!c.DisableReload) buttons.Add(refreshButton);
            if (!c.DisableItemBrowser) buttons.Add(itemButton);
            if (!c.DisableNPCBrowser) buttons.Add(npcButton);
            if (!c.DisableGod) buttons.Add(godButton);
            if (!c.DisableFast) buttons.Add(fastButton);
            if (!c.DisableHitboxes) buttons.Add(hitboxButton);
            if (!c.DisableUIElements) buttons.Add(uiDebugButton);
            if (!c.DisableLog) buttons.Add(logButton);

            // Append reload button to on multiplayer/singleplayer
            buttons.Add(reloadSingleplayerButton);
            buttons.Add(reloadMultiplayerButton); //TODO maybe only in multiplayer client

            // Add minimal item browser
            buttons.Add(minimalItemButton);
            // Add the minimalitembrowserpanel to the state
            // Append(new MinimalItemBrowserPanel());

            // Append all buttons and set AllButtons.
            buttons.ForEach(Append);
            AllButtons = new HashSet<BaseButton>(buttons);

            // Append all enabled buttons to the UI
            foreach (var button in AllButtons)
            {
                Append(button);
            }

            // Adjust left offsets for the appended buttons (skip the toggle button).
            int index = 0;
            foreach (BaseButton btn in AllButtons)
            {
                if (btn == toggleButton)
                    btn.RelativeLeftOffset = 0;
                else
                    btn.RelativeLeftOffset = ButtonSize * (++index);
            }

            // Initialize textures and positions.
            UpdateButtonsPositions(toggleButton.anchorPos);
        }

        // Utility to create & position any T : BaseButton
        private T CreateButton<T>(Asset<Texture2D> buttonImgText, string hoverText)
            where T : BaseButton
        {
            // We create the button via reflection
            T button = (T)Activator.CreateInstance(typeof(T), buttonImgText, hoverText);

            // setup center position
            // Log.Info($"Screen width: {Main.screenWidth}, Button count: 10, Button size: {ButtonSize}");
            // 1600 - 70 * 10 = 900 (center)
            // 900 / 2 = 450
            // convert to percent 450 / 1600 = 0.28125

            // Set up positions, alignment, etc.
            button.Width.Set(ButtonSize, 0f);
            button.Height.Set(ButtonSize, 0f);
            button.VAlign = 0.9f; // 90% from top
            button.HAlign = 0.28f;


            // set min and max, width and height
            button.MinWidth = new StyleDimension(ButtonSize, 0f);
            button.MaxWidth = new StyleDimension(ButtonSize, 0f);
            button.MinHeight = new StyleDimension(ButtonSize, 0f);
            button.MaxHeight = new StyleDimension(ButtonSize, 0f);

            button.Recalculate();
            return button;
        }

        // updates position only
        public void UpdateButtonsPositions(Vector2 anchorPosition)
        {
            int index = 0;
            foreach (BaseButton btn in AllButtons)
            {
                if (btn == toggleButton)
                    btn.RelativeLeftOffset = 0;
                else
                    btn.RelativeLeftOffset = ButtonSize * (++index);

                btn.Left.Set(anchorPosition.X + btn.RelativeLeftOffset, 0f);
                btn.Top.Set(anchorPosition.Y, 0f);
            }
            Recalculate(); // Refresh layout after moving buttons.
        }

        public void ToggleOnOff()
        {
            AreButtonsShowing = !AreButtonsShowing;
            toggleButton.UpdateTexture(); // on/off texture

            if (!AreButtonsShowing)
            {
                // Deactivate and hide all buttons
                foreach (BaseButton btn in AllButtons.Where(b => b != toggleButton))
                {
                    btn.Active = false;
                }
            }
            else
            {
                // Activate and show all buttons
                foreach (BaseButton btn in AllButtons.Where(b => b != toggleButton))
                {
                    btn.Active = true;
                }
            }
        }

        #region Update Buttons
        // Method to remove buttons based on config
        public void UpdateButtons()
        {
            Config c = ModContent.GetInstance<Config>();

            // Remove buttons based on config
            if (c.DisableConfig) RemoveButton(configButton);
            if (c.DisableReload) RemoveButton(refreshButton);
            if (c.DisableItemBrowser) RemoveButton(itemButton);
            if (c.DisableNPCBrowser) RemoveButton(npcButton);
            if (c.DisableGod) RemoveButton(godButton);
            if (c.DisableFast) RemoveButton(fastButton);
            if (c.DisableHitboxes) RemoveButton(hitboxButton);
            if (c.DisableUIElements) RemoveButton(uiDebugButton);
            if (c.DisableLog) RemoveButton(logButton);

            // Add buttons based on config
            if (!c.DisableConfig) AppendButton(configButton);
            if (!c.DisableReload) AppendButton(refreshButton);
            if (!c.DisableItemBrowser) AppendButton(itemButton);
            if (!c.DisableNPCBrowser) AppendButton(npcButton);
            if (!c.DisableGod) AppendButton(godButton);
            if (!c.DisableFast) AppendButton(fastButton);
            if (!c.DisableHitboxes) AppendButton(hitboxButton);
            if (!c.DisableUIElements) AppendButton(uiDebugButton);
            if (!c.DisableLog) AppendButton(logButton);

            // Update positions of remaining buttons
            UpdateButtonsPositions(toggleButton.anchorPos);
        }

        private void AppendButton(BaseButton button)
        {
            if (!AllButtons.Contains(button))
            {
                AllButtons.Add(button);
                Append(button);
            }
        }

        private void RemoveButton(BaseButton button)
        {
            if (button != null)
            {
                AllButtons.Remove(button);
                RemoveChild(button);
            }
        }
        #endregion
    }
}
