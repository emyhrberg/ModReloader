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
using Terraria;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class MainState : UIState
    {
        // State
        public bool AreButtonsShowing = true; // flag to toggle all buttons on/off using the toggle button
        public float ButtonSize = 70f;

        // Panels
        public ItemSpawner itemSpawnerPanel;
        public NPCSpawner npcSpawnerPanel;
        public PlayerPanel playerPanel;
        public DebugPanel debugPanel;
        public WorldPanel worldPanel;
        public ModsPanel modsPanel;
        public List<DraggablePanel> LeftSidePanels = [];
        public List<DraggablePanel> RightSidePanels = [];

        // Buttons
        public Collapse collapse;
        public List<BaseButton> AllButtons = [];

        // MainState Constructor. This is where we create all the buttons and set up their positions.
        public MainState()
        {
            // Add buttons
            AddButton<ConfigButton>(-210, Assets.ButtonConfig, "Config");
            AddButton<ItemButton>(-140, Assets.ButtonItems, "Items");
            AddButton<NPCButton>(-70, Assets.ButtonNPC, "NPC");
            AddButton<PlayerButton>(0, Assets.ButtonPlayer, "Player");
            AddButton<DebugButton>(70, Assets.ButtonDebug, "Debug");
            AddButton<WorldButton>(140, Assets.ButtonWorld, "World");
            AddButton<ReloadSPButton>(210, Assets.ButtonReloadSP, "Reload", "Left click to reload\nRight click to open list of mods\nAlt + click to show multiplayer reload");
            AddButton<ReloadMPButton>(210, Assets.ButtonReloadMP, "Reload", "Left click to reload\nRight click to open list of mods\nAlt + click to show singleplayer reload");

            // Add collapse button on top
            collapse = new(Assets.CollapseDown, Assets.CollapseUp);
            Append(collapse);

            // Make the MP button overlap the SP button
            BaseButton reloadSPButton = AllButtons[6];
            BaseButton reloadMPButton = AllButtons[7];
            reloadMPButton.Left.Set(reloadSPButton.Left.Pixels, 0);
            reloadMPButton.Top.Set(reloadSPButton.Top.Pixels, 0);
            RemoveChild(reloadMPButton);
            Append(reloadMPButton);
            reloadMPButton.Recalculate();

            // Add the panels (invisible by default)
            itemSpawnerPanel = AddPanel<ItemSpawner>("left");
            npcSpawnerPanel = AddPanel<NPCSpawner>("left");
            playerPanel = AddPanel<PlayerPanel>("right");
            debugPanel = AddPanel<DebugPanel>("right");
            worldPanel = AddPanel<WorldPanel>("right");
            modsPanel = AddPanel<ModsPanel>("right");
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

        private void AddButton<T>(float pos, Asset<Texture2D> spritesheet, string buttonText, string hoverText = null) where T : BaseButton
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
            button.Left.Set(pixels: pos, precent: 0f);

            // Add the button to the list of all buttons and append it to the MainState
            AllButtons.Add(button);
            Append(button);
        }
    }
}