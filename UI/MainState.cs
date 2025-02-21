using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.ItemSpawner;
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

        // Buttons
        public ItemSpawnerButton itemButton;
        public RefreshButton refreshButton;
        public ConfigButton configButton;
        public ToggleButton toggleButton;
        public NPCSpawnerButton npcButton;
        public GodButton godButton;
        public FastButton fastButton;
        public LogButton logButton;
        public HitboxButton hitboxButton;
        public DebugUIButton uiDebugButton;
        public ReloadSingleplayerButton reloadSingleplayerButton;
        public ReloadMultiplayerButton reloadMultiplayerButton;

        // List of all buttons
        public HashSet<BaseButton> AllButtons = [];

        // MainState Constructor
        // This is where we create all the buttons and set up their positions.
        public MainState()
        {
            // Create all buttons with a single line per button:
            toggleButton = AddButton<ToggleButton>(Assets.ButtonOn, "Toggle all buttons");
            configButton = AddButton<ConfigButton>(Assets.ButtonConfig, "Open config");
            refreshButton = AddButton<RefreshButton>(Assets.ButtonReload, "Reload the selected mod");
            itemButton = AddButton<ItemSpawnerButton>(Assets.ButtonItems, "Open Item Spawner\nContains all items in the game");
            npcButton = AddButton<NPCSpawnerButton>(Assets.ButtonNPC, "Open NPC Spawner\nContains town NPCs, enemies, and bosses");
            godButton = AddButton<GodButton>(Assets.ButtonGodOn, "Toggle player god mode");
            fastButton = AddButton<FastButton>(Assets.ButtonFastOn, "Toggle player fast mode");
            hitboxButton = AddButton<HitboxButton>(Assets.ButtonHitboxOn, "Show player, enemy, and projectile hitboxes");
            uiDebugButton = AddButton<DebugUIButton>(Assets.ButtonUIDebug, "Show all UIElements from mods");
            logButton = AddButton<LogButton>(Assets.ButtonLog, "Open client.log");

            // Adjust button positions (assumes toggleButton.anchorPos is set appropriately)
            UpdateButtonsPositions(toggleButton.anchorPos);

            // Add the panels (invisible)
            itemSpawnerPanel = new ItemSpawnerPanel();
            npcSpawnerPanel = new NPCSpawnerPanel();
            Append(itemSpawnerPanel);
            Append(npcSpawnerPanel);
        }

        private T AddButton<T>(Asset<Texture2D> buttonImgText, string hoverText)
        where T : BaseButton
        {
            // Create and configure the button
            T button = (T)Activator.CreateInstance(typeof(T), buttonImgText, hoverText);
            button.Width.Set(ButtonSize, 0f);
            button.Height.Set(ButtonSize, 0f);
            button.VAlign = 0.9f;
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
