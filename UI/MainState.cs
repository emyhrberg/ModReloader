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
        public float offset = 0; // START offset for first button position relative to center
        public List<BaseButton> AllButtons = [];
        public ReloadSPButton reloadSPButton;

        // MainState Constructor. This is where we create all the buttons and set up their positions.
        public MainState()
        {
            AddEverything();
        }

        public void AddEverything()
        {
            // 20 is CUSTOM CUSTOM CUSTOM offset, see collapse also. this is to avoid the collapse button colliding with heros mod
            offset = -ButtonSize * 5 - 20;
            // offset = -ButtonSize * 3 - 20;

            // 10% chance to make NPC asset xmas themed
            Asset<Texture2D> npcAss = Ass.ButtonNPC;
            // if (Main.rand.NextBool(10))
            // npcAss = Ass.ButtonNPC_XMAS;

            // Add buttons
            AddButton<ConfigButton>(Ass.ButtonConfig, "Config", "Temporary config for easy access. To be removed later");
            AddButton<TestButton>(Ass.CollapseUp, "Test", "TestButton");
            AddButton<StartGameButton>(Ass.CollapseDown, "Start", "Start additional tML client");
            AddButton<ItemButton>(Ass.ButtonItems, "Items", "Spawn all items in the game");
            AddButton<NPCButton>(npcAss, "NPC", "Spawn all NPC in the game");
            AddButton<PlayerButton>(Ass.ButtonPlayer, "Player", "Edit player stats and abilities");
            AddButton<DebugButton>(Ass.ButtonDebug, "Debug", "View and edit hitboxes, world, logs");
            AddButton<UIButton>(Ass.ButtonUI, "UI", "View and edit UI elements");
            AddButton<ModsButton>(Ass.ButtonMods, "Mods", "View list of mods");
            reloadSPButton = AddButton<ReloadSPButton>(Ass.ButtonReloadSP, "Reload", $"Reload {Conf.ModToReload} \nRight click to show multiplayer reload");
            offset -= ButtonSize; // move back to place MP above SP.
            AddButton<ReloadMPButton>(Ass.ButtonReloadMP, "Reload", $"Reload {Conf.ModToReload} \nRight click to show singleplayer reload");

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

        private T AddButton<T>(Asset<Texture2D> spritesheet = null, string buttonText = null, string hoverText = null) where T : BaseButton
        {
            // Create a new button using reflection
            T button = (T)Activator.CreateInstance(typeof(T), spritesheet, buttonText, hoverText);

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