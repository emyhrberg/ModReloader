using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.Social;
using static ModReloader.Common.Configs.Config;

namespace ModReloader.Common.Systems.Hooks
{
    public class MainMenuHook : ModSystem
    {
        // Variables
        private static DateTime lastConfigCheckTime = DateTime.UtcNow;

        public override void Load()
        {
            if (Conf.C != null && !Conf.C.ShowMainMenuInfo)
            {
                Log.Info("MainMenuHook: ImproveMainMenu is set to false. Not hooking into Main Menu.");
                return;
            }
            On_Main.DrawVersionNumber += DrawMenuOptions;
        }

        public override void Unload()
        {
            // Unload the hook
            if (Conf.C != null && !Conf.C.ShowMainMenuInfo)
            {
                Log.Info("MainMenuHook: ImproveMainMenu is set to false. Not unloading the hook into Main Menu.");
                return;
            }
            On_Main.DrawVersionNumber -= DrawMenuOptions;
        }
        #region draw hook
        private void DrawMenuOptions(On_Main.orig_DrawVersionNumber orig, Color menucolor, float upbump)
        {
            // Draw vanilla stuff first
            orig(menucolor, upbump);

            // Debug menu modes.
            // Log.SlowInfo("UIElementSystem: Draw called. MenuMode: " + Main.menuMode + ", State: " + Main.MenuUI.CurrentState?.GetType().Name);

            // Only draw all this stuff if in main menu mode 0 (default main menu screen)
            if (Main.menuMode != 0) return;

            // Check the config
            if (Conf.C != null && !Conf.C.ShowMainMenuInfo)
            {
                // Log.Info("MainMenuHook: ImproveMainMenu is set to false. Not drawing menu options.");
                return;
            }

            // check if mod is loaded
            Mod mod = ModReloader.Instance;
            if (mod == null) return;

            // Start at top-left corner
            var drawPos = new Vector2(15, 15);
            int extraYOffset = 0;

            // If other mods exist, move down worldDifficultyId bit
            if (ModLoader.HasMod("TerrariaOverhaul") || ModLoader.HasMod("Terramon"))
            {
                drawPos.Y += 210;
            }

            if (ModLoader.HasMod("CompatChecker"))
            {
                drawPos.Y += 30;
                extraYOffset += 30;
            }

            // Get names and tooltips for menu options
            string fileName = Path.GetFileName(Logging.LogPath);
            string reloadHoverMods = ReloadUtilities.IsModsToReloadEmpty ? "No mods selected" : string.Join(",", Conf.C.ModsToReload);

            // Maybe use class variable. Solve later or just be find with loading players and worlds every tick
            //if (DateTime.UtcNow - lastConfigCheckTime >= TimeSpan.FromSeconds(1))
            // Load / pick player
            Main.LoadPlayers();
            int playerIdx = Conf.C.Player;
            if (playerIdx < 0 || playerIdx >= Main.PlayerList.Count) playerIdx = 0;
            string playerName = Main.PlayerList.Count > 0 ? Main.PlayerList[playerIdx].Name : "";

            // Load / pick world
            Main.LoadWorlds();
            int worldIdx = Conf.C.World;
            if (worldIdx < 0 || worldIdx >= Main.WorldList.Count) worldIdx = 0;
            string worldName = Main.WorldList.Count > 0 ? Main.WorldList[worldIdx].Name : "";

            var menuOptions = new (string Text, Action Action, float scale, Func<string> tooltip)[]
            {
               ($"{mod.DisplayNameClean} v{mod.Version}", null, 1.15f, () => Loc.Get("MainMenu.WelcomeTooltip")),
               (Loc.Get("MainMenu.OpenConfigText"), () => Conf.C.Open(), 1.02f, () => Loc.Get("MainMenu.OpenConfigTooltip")),
               (Loc.Get("MainMenu.ReloadText"), async () => await ReloadUtilities.SinglePlayerReload(), 1.02f, () => Loc.Get("MainMenu.ReloadTooltip", $"[c/FFFF00:{reloadHoverMods}] ")),
               (" ", null, 1.15f, () => string.Empty), // empty line
               (Loc.Get("MainMenu.OptionsHeader"), null, 1.15f, () => Loc.Get("MainMenu.OptionsTooltip")),
               (Loc.Get("MainMenu.StartServerText"), StartServer, 1.02f, () => Loc.Get("MainMenu.StartServerTooltip")),
               (Loc.Get("MainMenu.StartClientText"), StartClient, 1.02f, () => Loc.Get("MainMenu.StartClientTooltip")),
               (Loc.Get("MainMenu.OpenLogText"), Log.OpenClientLog, 1.02f, () => Loc.Get("MainMenu.OpenLogTooltip", $"[c/FFFF00:{fileName}]")),
               (Loc.Get("MainMenu.ClearLogText"), Log.ClearClientLog, 1.02f, () => Loc.Get("MainMenu.ClearLogTooltip", $"[c/FFFF00:{fileName}]")),
               (" ", null, 1.15f, () => string.Empty), // empty line
               (Loc.Get("MainMenu.SingleplayerHeader"), null, 1.15f, () => Loc.Get("MainMenu.SingleplayerTooltip")),
               (Loc.Get("MainMenu.JoinSingleplayerText"), () =>
               {
                   ClientDataJsonHelper.ClientMode = ClientMode.SinglePlayer;
                   ClientDataJsonHelper.PlayerPath = null;
                   ClientDataJsonHelper.WorldPath = null;
                   AutoloadPlayerInWorldSystem.EnterSingleplayerWorld();
               }, 1.02f, () => {
                    if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(worldName))
                        return Loc.Get("MainMenu.JoinSingleplayerTooltipNoData");

                   return Loc.Get("MainMenu.JoinSingleplayerTooltip", $"[c/FFFF00: {playerName}]", $"[c/FFFF00:{worldName}]");
               }),
               (" ", null, 1.15f, () => string.Empty),
               (Loc.Get("MainMenu.MultiplayerHeader"), null, 1.15f, () => Loc.Get("MainMenu.MultiplayerTooltip")),
               (Loc.Get("MainMenu.HostMultiplayerText"), AutoloadPlayerInWorldSystem.HostMultiplayerWorld, 1.02f, () => Loc.Get("MainMenu.HostMultiplayerTooltip")),
               (Loc.Get("MainMenu.JoinMultiplayerText"), AutoloadPlayerInWorldSystem.EnterMultiplayerWorld, 1.02f, () => Loc.Get("MainMenu.JoinMultiplayerTooltip")),
               (" ", null, 1.15f, () => string.Empty),
               (Loc.Get("MainMenu.WorldHeader"), null, 1.15f, () => Loc.Get("MainMenu.WorldTooltip")),
               (
                Loc.Get("MainMenu.CreateNewWorld"),
                () => CreateNewWorld(GetNextAvailableTestWorldName()),   // ← click action
                1.02f,
                () => Loc.Get(
                "MainMenu.CreateNewWorldTooltip",
                $"[c/FFFF00:{GetNextAvailableTestWorldName()}]",
                $"[c/FFFF00:{Conf.C.CreateTestWorldSize}]",
                $"[c/FFFF00:{Conf.C.CreateTestWorldDifficulty}]")
                ),
            };

            foreach (var (text, action, scale, tooltip) in menuOptions)
            {
                // Measure text
                Vector2 size = FontAssets.MouseText.Value.MeasureString(text) * 0.9f;
                size.Y *= 0.9f; // Increase the Y size by 50%
                Vector2 hoverSize = new Vector2(size.X, size.Y * 1.26f);
                // Check if mouse is hovering it
                bool hovered = Main.MouseScreen.Between(drawPos, drawPos + hoverSize);

                if (hovered)
                {
                    // Draw tooltip
                    DrawHelper.DrawMainMenuTooltipPanel(tooltip, extraYOffset: extraYOffset);

                    Main.LocalPlayer.mouseInterface = true;
                    // Click
                    if (Main.mouseLeft && Main.mouseLeftRelease && action != null)
                    {
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                        Main.mouseLeftRelease = false;
                        action?.Invoke(); // Call the corresponding action
                    }
                }

                // Choose color/alpha
                Color textColor = hovered ? new Color(237, 246, 255) : new Color(173, 173, 198);
                float alpha = hovered ? 1f : 0.76f;
                if (action == null)
                {
                    alpha = 1f;
                    textColor = new Color(237, 246, 255);
                }

                // Draw with an outline
                DrawHelper.DrawOutlinedStringOnMenu(Main.spriteBatch, FontAssets.MouseText.Value, text, drawPos, textColor,
                    rotation: 0f, origin: Vector2.Zero, scale: scale, effects: SpriteEffects.None, layerDepth: 0f,
                    alphaMult: alpha);

                // Draw debug
                //Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, 
                //    new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)hoverSize.X, (int)hoverSize.Y),
                //    Color.Red * 0.5f // Semi-transparent red.
                //);

                // Move down for the next line
                drawPos.Y += size.Y + 6f;
            }
        }
        #endregion

        #region actions

        private static string GetNextAvailableTestWorldName()
        {
            Main.LoadWorlds();
            var taken = new HashSet<string>(Main.WorldList.Select(w => w.Name),StringComparer.OrdinalIgnoreCase);

            const string baseName = "Test";
            int n = 1;
            while (taken.Contains($"{baseName}{n}"))
                n++;

            return $"{baseName}{n}";
        }

        private static void CreateNewWorld(string desiredName)
        {
            Main.LoadPlayers();
            if (Main.PlayerList.Count == 0) return;
            int pIdx = Conf.C.Player;
            if (pIdx < 0 || pIdx >= Main.PlayerList.Count) pIdx = 0;
            Main.SelectPlayer(Main.PlayerList[pIdx]);

            Main.LoadWorlds();
            var taken = new HashSet<string>(Main.WorldList.Select(w => w.Name),
                                            StringComparer.OrdinalIgnoreCase);
            string finalName = taken.Contains(desiredName)
                               ? GetNextAvailableTestWorldName()
                               : desiredName;

            // Set world size based on config
            switch (Conf.C.CreateTestWorldSize)
            {
                case WorldSize.ExtraSmall:
                    Main.maxTilesX = 4000;
                    Main.maxTilesY = 800;
                    break;
                case WorldSize.Small:
                    Main.maxTilesX = 4200;
                    Main.maxTilesY = 1200;
                    break;
                case WorldSize.Medium:
                    Main.maxTilesX = 6400;
                    Main.maxTilesY = 1800;
                    break;
                case WorldSize.Large:
                    Main.maxTilesX = 8400;
                    Main.maxTilesY = 2400;
                    break;
            }
            WorldGen.setWorldSize();

            // Set difficulty based on config
            int worldDifficultyId = Conf.C.CreateTestWorldDifficulty switch 
            {
                WorldDifficulty.Normal => 0,
                WorldDifficulty.Expert => 1,
                WorldDifficulty.Master => 2,
                WorldDifficulty.Journey => 3,
                _ => 0
            };
            Main.GameMode = worldDifficultyId;

            WorldGen.WorldGenParam_Evil = 0;   // Corruption

            string seed = WorldGen.genRand.Next().ToString();
            UIWorldCreation.ProcessSpecialWorldSeeds(seed);

            bool cloud = SocialAPI.Cloud != null && SocialAPI.Cloud.EnabledByDefault;
            Main.worldName = finalName;
            Main.ActiveWorldFileData = WorldFile.CreateMetadata(finalName, cloud, Main.GameMode);
            Main.ActiveWorldFileData.SetSeed(seed);

            Main.menuMode = 10;
            WorldGen.CreateNewWorld();
        }

        private static void StartClient()
        {
            try
            {
                string steamPath = Log.GetSteamPath();
                string startGameFileName = Path.Combine(steamPath, "start-tModLoader.bat");
                if (!File.Exists(startGameFileName))
                {
                    Log.Error("Failed to find start-tModLoader.bat file.");
                    return;
                }

                // create worldDifficultyId process
                ProcessStartInfo process = new(startGameFileName)
                {
                    UseShellExecute = true,
                };

                // start the process
                Process gameProcess = Process.Start(process);
                Log.Info("Game process started with ID: " + gameProcess.Id + " and name: " + gameProcess.ProcessName);
            }
            catch (Exception e)
            {
                Log.Error("Failed to start game process (start-tModLoader.bat failed to launch): " + e.Message);
                return;
            }
        }

        private static void StartServer()
        {
            try
            {
                Main.LoadWorlds();

                if (Main.WorldList.Count == 0)
                    throw new Exception("No worlds found.");

                // Getting Player and World from ClientDataHandler
                var world = Main.WorldList.FirstOrDefault(p => p.Path.Equals(ClientDataJsonHelper.WorldPath)) ?? throw new Exception("World not found: " + ClientDataJsonHelper.WorldPath);
                if (string.IsNullOrEmpty(world.Path))
                {
                    Log.Error($"World {world.Name} has an invalid or null path.");
                    var worldPath = world.Path;
                    throw new ArgumentNullException(nameof(worldPath), "World path cannot be null or empty.");
                }

                string steamPath = Log.GetSteamPath();
                string startServerFileName = Path.Combine(steamPath, "start-tModLoaderServer.bat");
                if (!File.Exists(startServerFileName))
                {
                    Log.Error("Failed to find start-tModLoaderServer.bat file.");
                    return;
                }

                // create worldDifficultyId process
                ProcessStartInfo process = new(startServerFileName)
                {
                    UseShellExecute = true,
                    Arguments = $"-nosteam -world {world.Path}"
                };

                // start the process
                Process serverProcess = Process.Start(process);
                Log.Info("Server process started with ID: " + serverProcess.Id + " and name: " + serverProcess.ProcessName);
            }
            catch (Exception e)
            {
                Log.Error("Failed to start server (start-tModLoaderServer.bat failed to launch): " + e.Message);
                return;
            }
        }

        #endregion
    }
}