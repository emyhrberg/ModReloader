using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    /// <summary>
    /// A panel containing options to modify player behaviour like God,Fast,Build,etc.
    /// </summary>
    public class DebugPanel : OptionPanel
    {
        // Variables 
        SliderOption widthOption;
        SliderOption heightOption;
        // bool showPlayerInfo = false;
        // PlayerInfoPanel playerInfoPanel;

        public DebugPanel() : base(title: "Debug", scrollbarEnabled: true)
        {
            // Get instances
            HitboxSystem hitboxSystem = ModContent.GetInstance<HitboxSystem>();
            DebugSystem debugSystem = ModContent.GetInstance<DebugSystem>();


            // Add debug options
            AddHeader("Hitboxes");
            AddOnOffOption(hitboxSystem.TogglePlayerHitboxes, "Player Hitboxes Off", "Show player hitboxes");
            AddOnOffOption(hitboxSystem.ToggleNPCHitboxes, "NPC Hitboxes Off", "Show NPC hitboxes (town NPCs, enemies and bosses)");
            AddOnOffOption(hitboxSystem.ToggleProjAndMeleeHitboxes, "Projectile Hitboxes Off", "Show projectile and melee hitboxes");
            AddPadding();

            // AddHeader("Info");
            // AddOnOffOption(TogglePlayerInfo, "Player Info Off (todo)", "Show player info panel\nRight click to lock to top right corner");
            // AddPadding();

            AddHeader("UI");
            AddOnOffOption(debugSystem.ToggleUIDebugDrawing, "UIElements Hitboxes Off", "Show all UI elements from mods");
            AddOnOffOption(debugSystem.ToggleUIDebugSizeElementDrawing, "UIElements Size Text Off", "Show sizes of UI elements");
            AddOnOffOption(debugSystem.PrintAllUIElements, "Print UIElements", "Prints all UI elements and dimensions to chat");
            AddPadding();

            AddHeader("Create DebugPanel");
            widthOption = new("Width", 0, 800, 100, null, 5, hover: "Width of the panel to create");
            heightOption = new("Height", 0, 800, 100, null, 5, hover: "Height of the panel to create");
            uiList.Add(widthOption);
            uiList.Add(heightOption);
            AddOnOffOption(SpawnDebugPanel, "Create", "Create a draggable DebugPanel with the specified dimensions");
            AddOnOffOption(RemoveAllDebugPanels, "Remove All DebugPanel", hoverText: "Remove all DebugPanels from the screen");
            AddPadding();

            AddHeader("Logs");
            AddOnOffOption(Log.OpenClientLog, "Open client.log", "Left click to open client.log\nRight click to open folder location", Log.OpenLogFolder);
            AddOnOffOption(Log.OpenEnabledJson, "Open enabled.json", "Left click to open enabled.json\nRight click to open folder location", Log.OpenEnabledJsonFolder);
        }

        // private void TogglePlayerInfo()
        // {
        //     showPlayerInfo = !showPlayerInfo;

        //     MainSystem sys = ModContent.GetInstance<MainSystem>();

        //     if (showPlayerInfo)
        //         sys.mainState.Append(playerInfoPanel = new PlayerInfoPanel());
        //     else
        //         sys.mainState.RemoveChild(playerInfoPanel);
        // }

        private void SpawnDebugPanel()
        {
            // get width and height from slider options
            int w = (int)Math.Round(widthOption.normalizedValue * 800);
            int h = (int)Math.Round(heightOption.normalizedValue * 800);
            Log.Info("Creating DebugPanel with dimensions: " + w + ", " + h);

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys.mainState.Append(new CustomDebugPanel(w, h));
        }

        private void RemoveAllDebugPanels()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;

            if (mainState.Children == null) return;

            // Remove all panels of type CustomDebugPanel
            foreach (var child in mainState.Children.ToList())
            {
                if (child is CustomDebugPanel)
                {
                    mainState?.RemoveChild(child);
                }
            }
        }
    }
}