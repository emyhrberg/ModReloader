using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ModHelper.UI.Buttons;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI
{
    public class MainState : UIState
    {
        // Panels
        public ItemSpawner itemSpawnerPanel;
        public NPCSpawner npcSpawnerPanel;
        public PlayerPanel playerPanel;
        public LogPanel logPanel;
        public ModsPanel modsPanel;
        public UIElementPanel uiPanel;
        public WorldPanel worldPanel;
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
        public ReloadMPButton reloadMPButton;

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

            // Set offset for first button position relative to center
            offset = -ButtonSize * 4;
            offset -= 20; // 20 is CUSTOM CUSTOM CUSTOM offset, see collapse also. this is to avoid the collapse button colliding with heros mod

            // Add buttons
            AddButton<LaunchButton>(Ass.ButtonSecond, "Launch", "Start additional tML client", textSize: TextSize);
            AddButton<ItemButton>(Ass.ButtonItems, "Items", "Spawn all items in the game", textSize: TextSize);
            AddButton<NPCButton>(Ass.ButtonNPC, "NPC", "Spawn all NPC in the game", textSize: TextSize);
            AddButton<PlayerButton>(Ass.ButtonPlayer, "Player", "Edit player stats and abilities", textSize: TextSize);
            AddButton<WorldButton>(Ass.ButtonWorld2, "World", "Oversee all things in the world: NPCs, time, spawn rate, etc", textSize: TextSize);
            AddButton<LogButton>(Ass.ButtonDebug, "Log", "Customize logging", textSize: TextSize);
            AddButton<UIElementButton>(Ass.ButtonUI, "UI", "View and edit UI elements", textSize: TextSize);

            // Reload buttons. If MultiplayerClient, show only multiplayer. Otherwise, show both with toggle.
            reloadSPButton = AddButton<ReloadSPButton>(Ass.ButtonReloadSP, "Reload", $"Reload \n{Conf.ModToReload}", textSize: TextSize);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                reloadMPButton = AddButton<ReloadMPButton>(Ass.ButtonReloadMP, "Reload", $"Reload \n{Conf.ModToReload}", textSize: TextSize);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                offset -= ButtonSize;
                reloadMPButton = AddButton<ReloadMPButton>(Ass.ButtonReloadMP, "Reload", $"Reload \n{Conf.ModToReload}", textSize: TextSize);
            }

            // Mods button
            AddButton<ModsButton>(Ass.ButtonMods, "Mods", "View list of mods", textSize: TextSize);

            // Add collapse button on top
            collapse = new(Ass.CollapseDown, Ass.CollapseUp, Ass.CollapseLeft, Ass.CollapseRight);
            Append(collapse);

            // Add the panels (invisible by default)
            itemSpawnerPanel = AddPanel<ItemSpawner>("left");
            npcSpawnerPanel = AddPanel<NPCSpawner>("left");
            playerPanel = AddPanel<PlayerPanel>("right");
            logPanel = AddPanel<LogPanel>("right");
            modsPanel = AddPanel<ModsPanel>("right");
            uiPanel = AddPanel<UIElementPanel>("right");
            worldPanel = AddPanel<WorldPanel>("right");

            if (Conf.AlwaysShowTextOnTop)
            {
                string onOff = Conf.KeepRunning ? "ON" : "OFF";
                KeepGameRunningTextButton topText = new("Keep Game Running: " + onOff);
                Append(topText);
            }
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