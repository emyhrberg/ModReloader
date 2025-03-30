using System;
using System.Collections.Generic;
using System.Linq;
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

        private Dictionary<float, float> buttonTextSizes = new()
        {
            { 50f, 0.8f },
            { 55f, 0.8f },
            { 60f, 0.9f },
            { 65f, 0.9f },
            { 70f, 1f },
            { 75f, 1.1f },
            { 80f, 1.2f }
        };

        // MainState Constructor. This is where we create all the buttons and set up their positions.
        public MainState() => AddEverything();

        public void AddEverything()
        {
            // Set buttonsize according to config
            Config c = ModContent.GetInstance<Config>();
            if (c != null)
                ButtonSize = Conf.C.ButtonSize;

            // Set text size
            if (buttonTextSizes.TryGetValue(ButtonSize, out float value))
                TextSize = value;

            // Set offset for first button position relative to center
            offset = -ButtonSize * 4;
            offset -= 20; // 20 is CUSTOM CUSTOM CUSTOM offset, see collapse also. this is to avoid the collapse button colliding with heros mod

            // Add buttons
            AddButton<LaunchButton>(Ass.ButtonSecond, "Launch", "Start additional tML client");
            AddButton<ItemButton>(Ass.ButtonItems, "Items", "Spawn all items in the game");
            AddButton<NPCButton>(Ass.ButtonNPC, "NPC", "Spawn all NPC in the game");
            AddButton<PlayerButton>(Ass.ButtonPlayer, "Player", "Edit player stats and abilities", hoverTextDescription: "Right click to toggle super mode");
            AddButton<WorldButton>(Ass.ButtonWorld2, "World", "Set world settings and debugging");
            AddButton<LogButton>(Ass.ButtonDebug, "Log", "Customize logging", hoverTextDescription: "Right click to open log");
            AddButton<UIElementButton>(Ass.ButtonUI, "UI", "View and edit UI elements", hoverTextDescription: "Right click to toggle all UI elements hitboxes");
            AddButton<ModsButton>(Ass.ButtonMods, "Mods", "View list of mods", hoverTextDescription: "Right click to go to mod sources");

            // offset += ButtonSize;

            // Reload buttons. If MultiplayerClient, show only multiplayer. Otherwise, show both with toggle.
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                reloadMPButton = AddButton<ReloadMPButton>(Ass.ButtonReloadMP, "Reload", $"Reload \n{Conf.C.LatestModToReload}");
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                reloadSPButton = AddButton<ReloadSPButton>(Ass.ButtonReloadSP, "Reload", $"Reload {Conf.C.LatestModToReload}", hoverTextDescription: $"Right click to toggle MP reload");
                offset -= ButtonSize;
                reloadMPButton = AddButton<ReloadMPButton>(Ass.ButtonReloadMP, "Reload", $"Reload {Conf.C.LatestModToReload}", hoverTextDescription: $"Right click to toggle SP reload");
            }

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

            if (Main.netMode == NetmodeID.SinglePlayer && Conf.C.ShowGameKeepRunningText)
            {
                string onOff = Conf.C.ShowGameKeepRunningText ? "ON" : "OFF";
                KeepGameRunningText topText = new($"Keep Game Running: {onOff})");
                Append(topText);
            }

            // Temporary debug text for player name, who am I, and frame rate
            DebugText debugText = new(text: "");
            Append(debugText);
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

        private T AddButton<T>(Asset<Texture2D> spritesheet = null, string buttonText = null, string hoverText = null, string hoverTextDescription = "") where T : BaseButton
        {
            // Create a new button using reflection
            T button = (T)Activator.CreateInstance(typeof(T), spritesheet, buttonText, hoverText, hoverTextDescription);

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
            if (Conf.C.ButtonPosition == "Left")
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw everything in the main state
            base.Draw(spriteBatch);

            // last, draw the tooltips
            // this is to avoid the tooltips being drawn over the buttons

            foreach (var button in AllButtons)
            {
                // bool isAnyPanelOpen = LeftSidePanels.Any(p => p.Active) || RightSidePanels.Any(p => p.Active);

                if (button.IsMouseHovering && button.HoverText != null)
                {
                    // Draw the tooltip
                    DrawHelper.DrawTooltipPanel(button, button.HoverText, button.HoverTextDescription); // Draw the tooltip panel
                }
            }
        }

        public void TogglePanel(DraggablePanel panel)
        {
            // Determine if the panel is in LeftSidePanels or RightSidePanels
            bool isLeft = LeftSidePanels.Contains(panel);

            // Deactivate other panels on the same side
            var panelGroup = isLeft ? LeftSidePanels : RightSidePanels;
            foreach (var p in panelGroup)
            {
                if (p != panel) p.SetActive(false);
            }

            // Toggle the target panel
            panel.SetActive(!panel.GetActive());
        }
    }
}