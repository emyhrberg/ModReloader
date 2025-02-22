using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Buttons;
using SquidTestingMod.UI.Panels;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class MainState : UIState
    {
        // State
        public bool AreButtonsShowing = true; // flag to toggle all buttons on/off using the toggle button
        public float ButtonSize = 70f;

        // ItemSpawner and NPCSpawner panels
        public ItemSpawnerPanel itemSpawnerPanel;
        public NPCSpawnerPanel npcSpawnerPanel;
        public PlayerPanel playerPanel;
        public DebugPanel debugPanel;
        public WorldPanel worldPanel;

        // Buttons
        public ItemSpawnerButton itemButton;
        public ConfigButton configButton;
        public ToggleButton toggleButton;
        public NPCSpawnerButton npcButton;
        public PlayerButton playerButton;
        public DebugButton debugButton;
        public WorldButton worldButton;
        public ReloadSingleplayerButton reloadSingleplayerButton;
        public ReloadMultiplayerButton reloadMultiplayerButton;

        // List of all buttons
        public HashSet<BaseButton> AllButtons = [];

        // MainState Constructor
        // This is where we create all the buttons and set up their positions.
        public MainState()
        {
            // Check if reloadbuttons only
            if (Conf.ReloadButtonsOnly)
            {
                toggleButton = AddButton<ToggleButton>(Assets.ButtonOn, "Toggle all buttons");
                configButton = AddButton<ConfigButton>(Assets.ButtonConfig, "Open config");
                reloadSingleplayerButton = AddButton<ReloadSingleplayerButton>(Assets.ButtonReload, "Reload mod in singleplayer");
                reloadMultiplayerButton = AddButton<ReloadMultiplayerButton>(Assets.ButtonReload, "Reload mod in multiplayer");
                UpdateButtonsPositions(toggleButton.anchorPos);
                return;
            }

            // Create all buttons with a single line per button:
            toggleButton = AddButton<ToggleButton>(Assets.ButtonOn, "Toggle all buttons");
            configButton = AddButton<ConfigButton>(Assets.ButtonConfig, "Open config");
            itemButton = AddButton<ItemSpawnerButton>(Assets.ButtonItems, "Open Item Spawner\nContains all items in the game");
            npcButton = AddButton<NPCSpawnerButton>(Assets.ButtonNPC, "Open NPC Spawner\nContains town NPCs, enemies, and bosses");
            playerButton = AddButton<PlayerButton>(Assets.ButtonPlayer, "Toggle player panel");
            debugButton = AddButton<DebugButton>(Assets.ButtonDebug, "Toggle debug panel");
            worldButton = AddButton<WorldButton>(Assets.ButtonWorld, "Toggle world panel");
            reloadSingleplayerButton = AddButton<ReloadSingleplayerButton>(Assets.ButtonReload, "Reload mod in singleplayer");
            reloadMultiplayerButton = AddButton<ReloadMultiplayerButton>(Assets.ButtonReload, "Reload mod in multiplayer");

            // Adjust button positions (assumes toggleButton.anchorPos is set appropriately)
            UpdateButtonsPositions(toggleButton.anchorPos);

            // Add the panels (invisible by default)
            itemSpawnerPanel = new ItemSpawnerPanel();
            npcSpawnerPanel = new NPCSpawnerPanel();
            playerPanel = new PlayerPanel();
            debugPanel = new DebugPanel();
            worldPanel = new WorldPanel();
            Append(itemSpawnerPanel);
            Append(npcSpawnerPanel);
            Append(playerPanel);
            Append(debugPanel);
            Append(worldPanel);
        }

        private T AddButton<T>(Asset<Texture2D> buttonImgText, string hoverText)
        where T : BaseButton
        {
            // Create and configure the button
            T button = (T)Activator.CreateInstance(typeof(T), buttonImgText, hoverText);
            button.Width.Set(ButtonSize, 0f);
            button.Height.Set(ButtonSize, 0f);
            button.VAlign = 0.02f;
            button.HAlign = 0.35f;
            button.MaxWidth = new StyleDimension(ButtonSize, 0);
            button.MaxHeight = new StyleDimension(ButtonSize, 0);
            button.MinHeight = new StyleDimension(ButtonSize, 0);
            button.MinWidth = new StyleDimension(ButtonSize, 0);
            button.Recalculate();

            // Add the button to the list and UI
            AllButtons.Add(button);
            Append(button);

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

            HashSet<BaseButton> buttonsExceptToggle = AllButtons.Where(btn => btn != toggleButton).ToHashSet();

            foreach (BaseButton btn in buttonsExceptToggle)
            {
                btn.Active = AreButtonsShowing;
            }
        }
    }
}
