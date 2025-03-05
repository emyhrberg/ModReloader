using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Panels;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel containing options to modify player behaviour like God,Fast,Build,etc.
    /// </summary>
    public class DebugPanel : RightParentPanel
    {
        // Variables 
        SliderOption widthOption;
        SliderOption heightOption;
        bool enableSpawning = true;
        bool showPlayerInfo = false;
        bool showConsolePanel = false;
        PlayerInfoPanel playerInfoPanel;
        ConsolePanel consolePanel;

        public DebugPanel() : base(title: "Debug", scrollbarEnabled: true)
        {
            // Get instances
            HitboxSystem hitboxSystem = ModContent.GetInstance<HitboxSystem>();
            DebugSystem debugSystem = ModContent.GetInstance<DebugSystem>();
            // DebugEnemyTrackingSystem enemyTracking = ModContent.GetInstance<DebugEnemyTrackingSystem>();


            // Add debug options
            AddHeader("Hitboxes");
            AddOnOffOption(hitboxSystem.TogglePlayerHitboxes, "Player Hitboxes Off", "Show player hitboxes");
            AddOnOffOption(hitboxSystem.ToggleNPCHitboxes, "NPC Hitboxes Off", "Show NPC hitboxes (town NPCs, enemies and bosses)");
            AddOnOffOption(hitboxSystem.ToggleProjAndMeleeHitboxes, "Projectile Hitboxes Off", "Show projectile and melee hitboxes");
            AddPadding();

            AddHeader("Info");
            AddOnOffOption(ToggleConsole, "Console Off (pending)", "Show console");
            AddOnOffOption(TogglePlayerInfo, "Player Info Off (pending)", "Show player info panel\nRight click to lock to top right corner");
            AddOnOffOption(DebugEnemyTrackingSystem.ToggleTracking, "Track Enemies Off (pending)", "Show all enemies position");
            AddOnOffOption(ToggleEnemySpawnRate, "Enemies Can Spawn: On (pending)");
            AddPadding();

            AddHeader("UI");
            AddOnOffOption(debugSystem.ToggleUIDebugDrawing, "UIElements Hitboxes Off", "Show all UI elements from mods");
            AddOnOffOption(debugSystem.ToggleUIDebugSizeElementDrawing, "UIElements Size Text Off", "Show sizes of UI elements");
            AddOnOffOption(debugSystem.PrintAllUIElements, "Print UIElements", "Prints all UI elements and dimensions to chat");
            AddPadding();

            AddHeader("Debug Panel");
            widthOption = AddSliderOption("Width", 0, 800, 100);
            heightOption = AddSliderOption("Height", 0, 800, 100);
            AddOnOffOption(SpawnDebugPanel, "Create DebugPanel", "Create a debug panel with the specified dimensions");
            AddOnOffOption(RemoveAllDebugPanels, "Remove All DebugPanel");
            AddPadding();

            AddHeader("Logs");
            AddOnOffOption(Log.OpenClientLog, "Open client.log", "Open log file \n Right click to open folder location", Log.OpenLogFolder);
            AddOnOffOption(Log.OpenEnabledJson, "Open enabled.json", "This file shows currently enabled mods\nRight click to open folder location", Log.OpenEnabledJsonFolder);
        }

        private void ToggleConsole()
        {
            showConsolePanel = !showConsolePanel;

            if (showPlayerInfo)
            {
                // Disable player info panel if console is enabled
                TogglePlayerInfo();
            }

            MainSystem sys = ModContent.GetInstance<MainSystem>();

            if (showConsolePanel)
                sys.mainState.Append(consolePanel = new ConsolePanel("Console"));
            else
                sys.mainState.RemoveChild(playerInfoPanel);
        }

        private void TogglePlayerInfo()
        {
            showPlayerInfo = !showPlayerInfo;

            if (showConsolePanel)
            {
                // Disable console panel if player info is enabled
                ToggleConsole();
            }

            MainSystem sys = ModContent.GetInstance<MainSystem>();

            if (showPlayerInfo)
                sys.mainState.Append(playerInfoPanel = new PlayerInfoPanel("Player Info"));
            else
                sys.mainState.RemoveChild(playerInfoPanel);
        }

        private void ToggleEnemySpawnRate()
        {
            enableSpawning = !enableSpawning;

            if (!enableSpawning)
            {
                // Butcher all hostile NPCs
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].CanBeChasedBy()) // Checks if it's a hostile NPC
                    {
                        Main.npc[i].life = 0;
                        Main.npc[i].HitEffect();
                        Main.npc[i].active = false;
                        // NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i); // Sync NPC despawn
                    }
                }

                // Set spawn rate to 0x (disable spawning)
                SpawnRateMultiplier.Multiplier = 0f;
                Main.NewText("Enemy spawn rate disabled. All hostiles removed.", 255, 0, 0);
            }
            else
            {
                // Restore normal spawn rate (1x)
                SpawnRateMultiplier.Multiplier = 1f;
                Main.NewText("Enemy spawn rate set to normal (1x).", 0, 255, 0);
            }
        }

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