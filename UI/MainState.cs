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
        public ItemSpawner itemSpawnerPanel;
        public NPCSpawner npcSpawnerPanel;
        public PlayerPanel playerPanel;
        public DebugPanel debugPanel;
        public WorldPanel worldPanel;

        // Buttons
        public ToggleButton toggleButton;

        // List of all buttons
        public List<BaseButton> AllButtons = [];

        // MainState Constructor
        // This is where we create all the buttons and set up their positions.
        public MainState()
        {
            // Create all buttons
            if (C.ShowToggleButton) toggleButton = AddButton<ToggleButton>(Assets.ButtonOnOff, "Toggle", "Toggle buttons on/off");
            if (C.ShowConfigButton) AddButton<ConfigButton>(Assets.ButtonConfig, "Config", "Open config menu");
            if (C.ShowItemButton) AddButton<ItemButton>(Assets.ButtonItems, "Items", "Open item browser");
            if (C.ShowNPCButton) AddButton<NPCButton>(Assets.ButtonNPC, "NPC", "Open NPC browser");
            if (C.ShowPlayerButton) AddButton<PlayerButton>(Assets.ButtonPlayer, "Player", "Open player options");
            if (C.ShowDebugButton) AddButton<DebugButton>(Assets.ButtonDebug, "Debug", "Open debug options");
            if (C.ShowWorldButton) AddButton<WorldButton>(Assets.ButtonWorld, "World", "Open world options");
            if (C.ShowReloadSPButton) AddButton<ReloadSPButton>(Assets.ButtonReloadSP, "Reload", "Reload mod in singleplayer");
            if (C.ShowReloadMPButton) AddButton<ReloadMPButton>(Assets.ButtonReloadMP, "Reload", "Reload mod in multiplayer");

            // Adjust button positions (assumes toggleButton.anchorPos is set appropriately)
            UpdateButtonsPositions(toggleButton.anchorPos);

            // Add the panels (invisible by default)
            if (C.ShowItemButton) Append(itemSpawnerPanel = new ItemSpawner());
            if (C.ShowNPCButton) Append(npcSpawnerPanel = new NPCSpawner());
            if (C.ShowPlayerButton) Append(playerPanel = new PlayerPanel());
            if (C.ShowDebugButton) Append(debugPanel = new DebugPanel());
            if (C.ShowWorldButton) Append(worldPanel = new WorldPanel());
        }

        private T AddButton<T>(Asset<Texture2D> spritesheet, string buttonText, string hoverText)
        where T : BaseButton
        {
            // Create and configure the button
            T button = (T)Activator.CreateInstance(typeof(T), spritesheet, buttonText, hoverText);
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

        public void UpdateButtonsAfterConfigChanged()
        {
            // Re-add in the correct order
            AllButtons.Clear();
            RemoveAllChildren();

            // Create all buttons
            if (C.ShowToggleButton) toggleButton = AddButton<ToggleButton>(Assets.ButtonOnOff, "Toggle", "Toggle buttons on/off");
            if (C.ShowConfigButton) AddButton<ConfigButton>(Assets.ButtonConfig, "Config", "Open config menu");
            if (C.ShowItemButton) AddButton<ItemButton>(Assets.ButtonItems, "Items", "Open item browser");
            if (C.ShowNPCButton) AddButton<NPCButton>(Assets.ButtonNPC, "NPC", "Open NPC browser");
            if (C.ShowPlayerButton) AddButton<PlayerButton>(Assets.ButtonPlayer, "Player", "Open player options");
            if (C.ShowDebugButton) AddButton<DebugButton>(Assets.ButtonDebug, "Debug", "Open debug options");
            if (C.ShowWorldButton) AddButton<WorldButton>(Assets.ButtonWorld, "World", "Open world options");
            if (C.ShowReloadSPButton) AddButton<ReloadSPButton>(Assets.ButtonReloadSP, "Reload", "Reload mod in singleplayer");
            if (C.ShowReloadMPButton) AddButton<ReloadMPButton>(Assets.ButtonReloadMP, "Reload", "Reload mod in multiplayer");

            // Adjust button positions (assumes toggleButton.anchorPos is set appropriately)
            UpdateButtonsPositions(toggleButton.anchorPos);

            // Add the panels (invisible by default)
            if (C.ShowItemButton) Append(itemSpawnerPanel = new ItemSpawner());
            if (C.ShowNPCButton) Append(npcSpawnerPanel = new NPCSpawner());
            if (C.ShowPlayerButton) Append(playerPanel = new PlayerPanel());
            if (C.ShowDebugButton) Append(debugPanel = new DebugPanel());
            if (C.ShowWorldButton) Append(worldPanel = new WorldPanel());
        }

        // updates position only
        public void UpdateButtonsPositions(Vector2 anchorPosition)
        {
            int index = 0;
            foreach (BaseButton btn in AllButtons)
            {
                // set relative left offset to toggle button
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
            // toggleButton.UpdateTexture(); // on/off texture

            List<BaseButton> buttonsExceptToggle = AllButtons.Except([toggleButton]).ToList();

            foreach (BaseButton btn in buttonsExceptToggle)
            {
                btn.Active = AreButtonsShowing;
            }
        }
    }
}
