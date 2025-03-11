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
        public float ButtonSize = Conf.ButtonSize != 0 ? Conf.ButtonSize : 70;
        public float ButtonScale = 1f;

        // ItemSpawner and NPCSpawner panels
        public ItemSpawner itemSpawnerPanel;
        public NPCSpawner npcSpawnerPanel;
        public PlayerPanel playerPanel;
        public DebugPanel debugPanel;
        public WorldPanel worldPanel;
        public ModsPanel modsPanel;

        // Buttons
        public Collapse collapse;

        // List of all buttons
        public List<BaseButton> AllButtons = [];

        // List of all panels
        public List<DraggablePanel> AllPanels = [];

        // MainState Constructor
        // This is where we create all the buttons and set up their positions.
        public MainState()
        {
            // Create collapse button
            collapse = new(Assets.CollapseDown, Assets.CollapseUp);
            Append(collapse);
            if (Conf.ShowConfigButton) AddButton<ConfigButton>(Assets.ButtonConfig, "AAA");
            if (Conf.ShowItemButton) AddButton<ItemButton>(Assets.ButtonItems, "Items");
            if (Conf.ShowNPCButton) AddButton<NPCButton>(Assets.ButtonNPC, "NPC");
            if (Conf.ShowPlayerButton) AddButton<PlayerButton>(Assets.ButtonPlayer, "Player");
            if (Conf.ShowDebugButton) AddButton<DebugButton>(Assets.ButtonDebug, "Debug");
            if (Conf.ShowWorldButton) AddButton<WorldButton>(Assets.ButtonWorld, "World");
            if (Conf.ShowReloadSPButton) AddButton<ReloadSPButton>(Assets.ButtonReloadSP, "Reload", "Reload mod in singleplayer\nRight click to open mods list");
            if (Conf.ShowReloadMPButton) AddButton<ReloadMPButton>(Assets.ButtonReloadMP, "Reload");

            // Adjust button positions (assumes toggleButton.anchorPos is set appropriately)
            Vector2 anchorPosition = new Vector2(20, 300);
            for (int i = 0; i < AllButtons.Count; i++)
            {
                BaseButton btn = AllButtons[i];
                btn.RelativeLeftOffset = ButtonSize * i;

                btn.Left.Set(anchorPosition.X + btn.RelativeLeftOffset, 0f);
                btn.Top.Set(anchorPosition.Y, 0f);
                btn.Recalculate();
            }

            // Add the panels (invisible by default)
            if (Conf.ShowItemButton) itemSpawnerPanel = AddPanel<ItemSpawner>("Item Spawner");
            if (Conf.ShowNPCButton) npcSpawnerPanel = AddPanel<NPCSpawner>("NPC Spawner");
            if (Conf.ShowPlayerButton) playerPanel = AddPanel<PlayerPanel>("Player Panel");
            if (Conf.ShowDebugButton) debugPanel = AddPanel<DebugPanel>("Debug Panel");
            if (Conf.ShowWorldButton) worldPanel = AddPanel<WorldPanel>("World Panel");
            modsPanel = AddPanel<ModsPanel>("Mods Panel");
        }

        private T AddPanel<T>(string title) where T : DraggablePanel, new()
        {
            T panel = new T(); // Instantiate using the parameterless constructor.
            panel.Header = title; // Assumes your DraggablePanel (or derived classes) has a SetTitle method.
            AllPanels.Add(panel);
            Append(panel);
            return panel;
        }

        private T AddButton<T>(Asset<Texture2D> spritesheet, string buttonText, string hoverText = null) where T : BaseButton
        {
            // Directly use the current config value.
            float size = Conf.ButtonSize != 0 ? Conf.ButtonSize : 70;
            T button = (T)Activator.CreateInstance(typeof(T), spritesheet, buttonText, hoverText);
            button.Width.Set(size, 0f);
            button.Height.Set(size, 0f);
            button.MaxWidth = new StyleDimension(size, 0);
            button.MaxHeight = new StyleDimension(size, 0);
            button.MinWidth = new StyleDimension(size, 0);
            button.MinHeight = new StyleDimension(size, 0);
            button.Recalculate();

            AllButtons.Add(button);
            Append(button);

            return button;
        }

        public void ToggleCollapse()
        {
            AreButtonsShowing = !AreButtonsShowing;

            foreach (BaseButton btn in AllButtons)
            {
                btn.Active = AreButtonsShowing;
            }
        }
    }
}
