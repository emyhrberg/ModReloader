using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Configs;
using ModReloader.Helpers;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.IO;

namespace ModReloader.Common.Systems.Hooks
{
    public class MainMenuHook : ModSystem
    {
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
        private static void DrawMenuOptions(On_Main.orig_DrawVersionNumber orig, Color menucolor, float upbump)
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

            // If other mods exist, move down a bit
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

            // Menu options with corresponding actions
            var menuOptions = new (string Text, Action Action, float scale, string tooltip)[]
            {
                ($"{mod.DisplayNameClean} v{mod.Version}", null, 1.15f, "Welcome to Mod Reloaders main menu!"),
                ("Open config", () => Conf.C.Open(), 1.02f, "Click to open Mod Reloader config"),
                ("Reload", async () => await ReloadUtilities.SinglePlayerReload(), 1.02f, $"Reloads {reloadHoverMods}"),
                (" ", null, 1.15f, ""), // empty line

                ($"Options", null, 1.15f, "General options for testing"),
                ("Start Server", StartServer, 1.02f, "Starts a server instance with cmd-line"),
                ("Start Client", StartClient, 1.02f, "Starts another tML instance with cmd-line in addition to this one"),
                ("Open Log", Conf.C.OpenLogType == "File" ? Log.OpenClientLog : Log.OpenLogFolder, 1.02f, $"Click to open the {fileName} of this client"),
                ("Clear Log", Log.ClearClientLog, 1.02f, $"Click to clear the {fileName} of this client"),
                (" ", null, 1.15f, ""), // empty line
                ($"Singleplayer", null, 1.15f, "Quickly join singleplayer worlds"),
                ("Join Singleplayer", () => {
                    ClientDataJsonHelper.ClientMode = ClientMode.SinglePlayer;
                    ClientDataJsonHelper.PlayerID = -1;
                    ClientDataJsonHelper.WorldID = -1;
                    AutoloadPlayerInWorldSystem.EnterSingleplayerWorld();
                }, 1.02f, "Enter singleplayer world with the selected player and world from config"),
                (" ", null, 1.15f, ""), // empty line
                ($"Multiplayer", null, 1.15f, "Hosting and testing multiplayer"),
                ("Host Multiplayer", AutoloadPlayerInWorldSystem.HostMultiplayerWorld, 1.02f, "Start a local multiplayer world"),
                ("Join Multiplayer", JoinMultiplayerNew, 1.02f, "Enter local multiplayer world with first available player (server required)"),
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

                // create a process
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
                var world = Main.WorldList[ClientDataJsonHelper.WorldID];

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

                // create a process
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

        private static void JoinMultiplayerNew()
        {
            Log.Info("[MainMenuHook] Trying to join localhost server with a player not already on the server");

            // ── 1. Load local characters ────────────────────────────────────
            Main.LoadPlayers();
            if (Main.PlayerList is null || Main.PlayerList.Count == 0)
            {
                Log.Error("No local players found.");
                return;
            }

            // ── 2. Choose a player that is NOT the one in Conf.C.Player ─────
            int cfgIndex = Conf.C.Player;                    // the one we want to avoid
            if (cfgIndex < 0 || cfgIndex >= Main.PlayerList.Count)
                cfgIndex = 0;                               // keep it in range

            int chosenIndex = -1;
            for (int i = 0; i < Main.PlayerList.Count; i++)
            {
                if (i != cfgIndex) { chosenIndex = i; break; }   // first non-config slot
            }
            if (chosenIndex == -1) chosenIndex = cfgIndex;       // only one character

            PlayerFileData playerToJoinWith = Main.PlayerList[chosenIndex];
            Log.Info($"Selected player index {chosenIndex}: {playerToJoinWith.Name}");
            Main.SelectPlayer(playerToJoinWith);

            // ── 3. Connect to localhost ─────────────────────────────────────
            Netplay.SetRemoteIP("127.0.0.1");
            Main.autoPass = true;
            Main.statusText = Lang.menu[8].Value;           // “Connecting…”
            Netplay.StartTcpClient();
            Main.menuMode = 10;
        }

        [Obsolete("Just use EnterMultiplayerWorld")]
        // Comment by Erky:
        // no cotlim, using EnterMultiplayerWorld doesnt work, we want to join with a player that is not already in the server
        private static void JoinMultiplayer()
        {
            // Simply join localhost, easy.
            Log.Info("EnterMultiplayerWorld() called!!");
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0)
                throw new Exception("No players found.");

            // Get Player
            // TODO check if player is already in server and if so, join with a different player.
            // var player = Main.PlayerList.FirstOrDefault();

            // select random player for now from playerlist
            // var random = new Random();
            // var player = Main.PlayerList[random.Next(Main.PlayerList.Count)];

            // Get a list of active players in the server
            List<Terraria.Player> activePlayers = Main.player.Where(p => p != null && p.active).ToList();

            // Get a list of all player file data
            // Select the first player that is not already in the active players list
            foreach (var p in Main.PlayerList)
            {
                Terraria.Player player = p.Player;

                if (!activePlayers.Contains(player))
                {
                    // Found a player that is not already in the active players list
                    Log.Info($"Found player: {player.name}");
                    // Select Player
                    Main.SelectPlayer(p);
                }
            }

            Main.SelectPlayer(Main.PlayerList.FirstOrDefault(p => !Main.player.Contains(p.Player)));

            // Play the selected world in multiplayer mode
            // Connect to server IP
            Ping pingSender = new();
            PingOptions options = new();
            options.DontFragment = true; // prevent packet from splitting into smaller packets
            string data = "a"; // dummy data to send because the Send method requires it
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data); // convert string to byte array
            int timeout = 2000; // 2000 ms timeout before the ping request is considered failed

            // Ping the server IP using the server's IP address
            PingReply reply;
            try
            {
                Netplay.ServerIP = new System.Net.IPAddress([127, 0, 0, 1]); // localhost
                reply = pingSender.Send(Netplay.ServerIP, timeout, buffer, options);
            }
            catch (PingException ex)
            {
                Log.Error($"Ping failed to destination server: {ex}");
                return;
            }

            if (reply.Status == IPStatus.Success)
            {
                Log.Info($"Ping successful to destination server: {reply.Address}");

                // set the IP AND PORT (the two necessary fields) for the server
                Netplay.ServerIP = new System.Net.IPAddress([127, 0, 0, 1]); // localhost
                Netplay.ListenPort = 7777; // default port

                Netplay.StartTcpClient(); // start the TCP client which is later used to connect to the server
            }
            else
            {
                Log.Error($"Ping failed to destination server, possibly timed out: {reply.Status}");
            }
        }

        [Obsolete("Just use EnterSinglePlayer")]
        private static void JoinSingleplayer()
        {
            Log.Info("JoinSingleplayer() called!");

            // Loading lists of Players and Worlds
            Main.LoadWorlds();
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
                throw new Exception("No players or worlds found.");

            // Check if the first world has a valid path
            if (string.IsNullOrEmpty(Main.WorldList.FirstOrDefault().Path))
            {
                Log.Error($"World {Main.WorldList.FirstOrDefault().Name} has an invalid or null path.");
                var worldPath = Main.WorldList.FirstOrDefault()?.Path;
                throw new ArgumentNullException(nameof(worldPath), "World path cannot be null or empty.");
            }

            // Getting Player and World from ClientDataHandler
            var player = Main.PlayerList.FirstOrDefault();
            var world = Main.WorldList.FirstOrDefault();

            // Start game with pair
            Main.SelectPlayer(player);
            Main.ActiveWorldFileData = world;

            Log.Info($"Starting game with Player: {player.Name}, World: {Main.WorldList.FirstOrDefault().Name}");

            WorldGen.playWorld(); // Play the selected world in singleplayer

            // show loading screen
            LoadWorldState.Show(world.Name);
        }

        #endregion
    }
}