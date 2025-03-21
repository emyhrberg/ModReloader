using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Buttons;
using SquidTestingMod.UI.Elements;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class MainState : UIState
    {
        // Panels
        public ItemSpawner itemSpawnerPanel;
        public NPCSpawner npcSpawnerPanel;
        public PlayerPanel playerPanel;
        public DebugPanel debugPanel;
        public ModsPanel modsPanel;
        public UiPanel uiPanel;
        public List<DraggablePanel> LeftSidePanels = [];
        public List<DraggablePanel> RightSidePanels = [];

        // Buttons
        public Collapse collapse;
        public bool AreButtonsShowing = true; // flag to toggle all buttons on/off using the toggle button
        public float ButtonSize = 70f;
        public float TextSize = 0.9f;
        public float offset = 0; // START offset for first button position relative to center
        public List<BaseButton> AllButtons = [];
        public ReloadSPButton reloadSPButton;

        // MainState Constructor. This is where we create all the buttons and set up their positions.
        public MainState() => AddEverything();

        public void AddEverything()
        {
            // Set buttonsize according to config
            Config c = ModContent.GetInstance<Config>();
            if (c != null)
                ButtonSize = Conf.ButtonSize;

            // Set text size
            if (c != null)
                TextSize = Conf.TextSize;

            Log.Info("TextSize: " + TextSize);

            // Set offset for first button position relative to center
            offset = -ButtonSize * 5;
            offset -= 20; // 20 is CUSTOM CUSTOM CUSTOM offset, see collapse also. this is to avoid the collapse button colliding with heros mod

            // Add buttons
            AddButton<ConfigButton>(Ass.ButtonConfig, "Config", "Temporary config for easy access.", textSize: TextSize);
            AddButton<TestButton>(Ass.CollapseUp, "Testing", "Temporary button used for testing", textSize: TextSize);
            AddButton<StartGameButton>(Ass.ButtonSecond, "Launch tML", "Start additional tML client", textSize: TextSize - 0.2f);
            AddButton<ItemButton>(Ass.ButtonItems, "Items", "Spawn all items in the game", textSize: TextSize);
            AddButton<NPCButton>(Ass.ButtonNPC, "NPC", "Spawn all NPC in the game", textSize: TextSize);
            AddButton<PlayerButton>(Ass.ButtonPlayer, "Player", "Edit player stats and abilities", textSize: TextSize);
            AddButton<DebugButton>(Ass.ButtonDebug, "Debug", "View and edit hitboxes, world, logs", textSize: TextSize);
            AddButton<UIButton>(Ass.ButtonUI, "UI", "View and edit UI elements", textSize: TextSize);
            AddButton<ModsButton>(Ass.ButtonMods, "Mods", "View list of mods", textSize: TextSize);
            reloadSPButton = AddButton<ReloadSPButton>(Ass.ButtonReloadSP, "Reload", $"Reload {Conf.ModToReload} \nRight click to show multiplayer reload", textSize: TextSize);
            // offset -= ButtonSize; // move back to place MP above SP.
            AddButton<ReloadMPButton>(Ass.ButtonReloadMP, "Reload", $"Reload {Conf.ModToReload} \nRight click to show singleplayer reload", textSize: TextSize);

            // Add collapse button on top
            collapse = new(Ass.CollapseDown, Ass.CollapseUp, Ass.CollapseLeft, Ass.CollapseRight);
            Append(collapse);

            // Add the panels (invisible by default)
            itemSpawnerPanel = AddPanel<ItemSpawner>("left");
            npcSpawnerPanel = AddPanel<NPCSpawner>("left");
            playerPanel = AddPanel<PlayerPanel>("right");
            debugPanel = AddPanel<DebugPanel>("right");
            modsPanel = AddPanel<ModsPanel>("right");
            uiPanel = AddPanel<UiPanel>("right");
        }

        private T AddPanel<T>(string side) where T : DraggablePanel, new()
        {
            // Create a new panel using reflection
            T panel = new();

            // Add to appropriate list
            if (side == "left")
                LeftSidePanels.Add(panel);
            else if (side == "right")
                RightSidePanels.Add(panel);

            // Add to MainState
            Append(panel);
            return panel;
        }

        private T AddButton<T>(Asset<Texture2D> spritesheet = null, string buttonText = null, string hoverText = null, float textSize = 0.9f) where T : BaseButton
        {
            // Create a new button using reflection
            T button = (T)Activator.CreateInstance(typeof(T), spritesheet, buttonText, hoverText, textSize);

            // Button dimensions
            float size = ButtonSize;
            button.Width.Set(size, 0f);
            button.Height.Set(size, 0f);
            button.MaxWidth = new StyleDimension(size, 0);
            button.MaxHeight = new StyleDimension(size, 0);
            button.MinWidth = new StyleDimension(size, 0);
            button.MinHeight = new StyleDimension(size, 0);
            button.Recalculate();
            button.VAlign = 1.0f;
            button.HAlign = 0.5f;

            // set x pos with offset
            button.Left.Set(pixels: offset, precent: 0f);

            // custom left pos. override default
            if (Conf.ButtonsPosition == "left")
            {
                button.VAlign = 0.73f;
                button.HAlign = 0f;
                button.Left.Set(pixels: 0, precent: 0f);
                button.Top.Set(pixels: offset, precent: 0f);
            }

            // increase offset for next button, except MPbutton
            offset += ButtonSize;

            // Add the button to the list of all buttons and append it to the MainState
            AllButtons.Add(button);
            Append(button);

            return button;
        }
    }
}