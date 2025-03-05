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

        // Buttons
        public ToggleButton toggleButton;

        // List of all buttons
        public List<BaseButton> AllButtons = [];

        // MainState Constructor
        // This is where we create all the buttons and set up their positions.
        public MainState()
        {
            // Create all buttons
            if (Conf.ShowToggleButton) toggleButton = AddButton<ToggleButton>(Assets.ButtonOnOff, "Toggle", "Toggle buttons on/off");
            if (Conf.ShowConfigButton) AddButton<ConfigButton>(Assets.ButtonConfig, "Config", "Open config menu");
            if (Conf.ShowItemButton) AddButton<ItemButton>(Assets.ButtonItems, "Items", "Open item browser");
            if (Conf.ShowNPCButton) AddButton<NPCButton>(Assets.ButtonNPC, "NPC", "Open NPC browser");
            if (Conf.ShowPlayerButton) AddButton<PlayerButton>(Assets.ButtonPlayer, "Player", "Open player options");
            if (Conf.ShowDebugButton) AddButton<DebugButton>(Assets.ButtonDebug, "Debug", "Open debug options");
            if (Conf.ShowWorldButton) AddButton<WorldButton>(Assets.ButtonWorld, "World", "Open world options");
            if (Conf.ShowReloadSPButton) AddButton<ReloadSPButton>(Assets.ButtonReloadSP, "Reload", "Reload mod in singleplayer\nRight click to go to mods list");
            if (Conf.ShowReloadMPButton) AddButton<ReloadMPButton>(Assets.ButtonReloadMP, "Reload", "Reload mod in multiplayer");

            // Adjust button positions (assumes toggleButton.anchorPos is set appropriately)
            UpdateButtonsPositions(toggleButton.anchorPos);

            // Add the panels (invisible by default)
            if (Conf.ShowItemButton) Append(itemSpawnerPanel = new ItemSpawner());
            if (Conf.ShowNPCButton) Append(npcSpawnerPanel = new NPCSpawner());
            if (Conf.ShowPlayerButton) Append(playerPanel = new PlayerPanel());
            if (Conf.ShowDebugButton) Append(debugPanel = new DebugPanel());
            if (Conf.ShowWorldButton) Append(worldPanel = new WorldPanel());
        }

        private T AddButton<T>(Asset<Texture2D> spritesheet, string buttonText, string hoverText)
where T : BaseButton
        {
            // Directly use the current config value.
            float size = Conf.ButtonSize != 0 ? Conf.ButtonSize : 70;
            T button = (T)Activator.CreateInstance(typeof(T), spritesheet, buttonText, hoverText);
            button.Width.Set(size, 0f);
            button.Height.Set(size, 0f);
            button.VAlign = 0.02f;
            button.HAlign = 0.35f;
            button.MaxWidth = new StyleDimension(size, 0);
            button.MaxHeight = new StyleDimension(size, 0);
            button.MinWidth = new StyleDimension(size, 0);
            button.MinHeight = new StyleDimension(size, 0);
            button.Recalculate();

            AllButtons.Add(button);
            Append(button);

            return button;
        }

        public void UpdateButtonsAfterConfigChanged()
        {
            // Set button size
            ButtonSize = Conf.ButtonSize != 0 ? Conf.ButtonSize : 70;

            // Re-add in the correct order
            AllButtons.Clear();
            RemoveAllChildren();

            // Create all buttons
            if (Conf.ShowToggleButton) toggleButton = AddButton<ToggleButton>(Assets.ButtonOnOff, "Toggle", "Toggle buttons on/off");
            if (Conf.ShowConfigButton) AddButton<ConfigButton>(Assets.ButtonConfig, "Config", "Open config menu");
            if (Conf.ShowItemButton) AddButton<ItemButton>(Assets.ButtonItems, "Items", "Open item browser");
            if (Conf.ShowNPCButton) AddButton<NPCButton>(Assets.ButtonNPC, "NPC", "Open NPC browser");
            if (Conf.ShowPlayerButton) AddButton<PlayerButton>(Assets.ButtonPlayer, "Player", "Open player options");
            if (Conf.ShowDebugButton) AddButton<DebugButton>(Assets.ButtonDebug, "Debug", "Open debug options");
            if (Conf.ShowWorldButton) AddButton<WorldButton>(Assets.ButtonWorld, "World", "Open world options");
            if (Conf.ShowReloadSPButton) AddButton<ReloadSPButton>(Assets.ButtonReloadSP, "Reload", "Reload mod in singleplayer");
            if (Conf.ShowReloadMPButton) AddButton<ReloadMPButton>(Assets.ButtonReloadMP, "Reload", "Reload mod in multiplayer");

            // Adjust button positions (assumes toggleButton.anchorPos is set appropriately)
            UpdateButtonsPositions(toggleButton.anchorPos);

            // Add the panels (invisible by default)
            if (Conf.ShowItemButton) Append(itemSpawnerPanel = new ItemSpawner());
            if (Conf.ShowNPCButton) Append(npcSpawnerPanel = new NPCSpawner());
            if (Conf.ShowPlayerButton) Append(playerPanel = new PlayerPanel());
            if (Conf.ShowDebugButton) Append(debugPanel = new DebugPanel());
            if (Conf.ShowWorldButton) Append(worldPanel = new WorldPanel());
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
