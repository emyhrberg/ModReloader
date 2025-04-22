using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace ModHelper.Common.Systems.Menus
{
    public class MainMenuHook : ModSystem
    {
        public override void Load()
        {
            if (Conf.C != null && !Conf.C.ImproveMainMenu)
            {
                Log.Info("MainMenuHook: ImproveMainMenu is set to false. Not hooking into Main Menu.");
                return;
            }
            On_Main.DrawVersionNumber += DrawMenuOptions;
        }

        public override void Unload()
        {
            // Unload the hook
            if (Conf.C != null && !Conf.C.ImproveMainMenu)
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
            if (Conf.C != null && !Conf.C.ImproveMainMenu)
            {
                // Log.Info("MainMenuHook: ImproveMainMenu is set to false. Not drawing menu options.");
                return;
            }

            // check if mod is loaded
            Mod mod = ModHelper.Instance;
            if (mod == null) return;

            // Start at top-left corner
            var drawPos = new Vector2(15, 15);

            // If other mods exist, move down a bit
            if (ModLoader.HasMod("TerrariaOverhaul") || ModLoader.HasMod("Terramon"))
            {
                drawPos.Y = Main.screenHeight / 2f - 74;
            }
            string fileName = Path.GetFileName(Logging.LogPath);

            List<string> modsToReloadFromJson = ModsToReloadJsonHelper.ReadModsToReload();

            string reloadHoverMods;
            if (modsToReloadFromJson == null || modsToReloadFromJson.Count == 0)
            {
                reloadHoverMods = "No mods selected";
            }
            else
            {
                reloadHoverMods = string.Join(", ", modsToReloadFromJson);
            }

            // Menu options with corresponding actions
            var menuOptions = new (string Text, Action Action, float scale, string tooltip)[]
            {
                ($"{mod.DisplayNameClean} v{mod.Version}", null, 1.15f, "Welcome to Mod Helpers main menu! Join worlds, change options, reload, etc..."),
                ("Open config", OpenConfig, 1.02f, "Click to open the Mod Helper config and change settings"),
                ("Reload", async () => await ReloadSelectedMod(), 1.02f, $"Reload {reloadHoverMods}"),
                (" ", null, 1.15f, ""), // empty line

                ($"Options", null, 1.15f, "General options for testing"),
                ("Start Server", StartServer, 1.02f, "Starts a server instance with cmd-line"),
                ("Start Client", StartClient, 1.02f, "Starts another tML instance with cmd-line in addition to this one"),
                ("Open Log", Log.OpenClientLog, 1.02f, $"Click to open the {fileName} of this client"),
                ("Clear Log", Log.ClearClientLog, 1.02f, $"Click to clear the {fileName} of this client"),
                (" ", null, 1.15f, ""), // empty line
                ($"Singleplayer", null, 1.15f, "Quickly join a world"),
                ("Join Singleplayer", JoinSingleplayer, 1.02f, "Enter a singleplayer world with last selected player and world"),
                (" ", null, 1.15f, ""), // empty line
                ($"Multiplayer", null, 1.15f, "Options for entering and testing multiple clients"),
                ("Host Multiplayer", HostMultiplayer, 1.02f, "Start a multiplayer world with last selected player and world"),
                ("Join Multiplayer", JoinMultiplayer, 1.02f, "Enter the multiplayer world with first available player (server required)"),

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
                    DrawHelper.DrawMainMenuTooltipPanel(tooltip);

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
                DrawOutlinedStringOnMenu(Main.spriteBatch, FontAssets.MouseText.Value, text, drawPos, textColor,
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

        private static void OpenConfig()
        {
            // this does the same as below code
            Conf.C.Open();

            return;
            // below code is not needed
            try
            {
                // Use reflection to get the private ConfigManager.Configs property.
                FieldInfo configsProp = typeof(ConfigManager).GetField("Configs", BindingFlags.Static | BindingFlags.NonPublic);
                var configs = configsProp.GetValue(null) as IDictionary<Mod, List<ModConfig>>;

                // Get the mod name from the modPath.
                // string modName = Path.GetFileName(modPath);
                string modName = ModHelper.Instance.Name;
                Mod modInstance = ModLoader.GetMod(modName);
                if (modInstance == null)
                {
                    Log.Info($"Mod '{modName}' not found.");
                    return;
                }

                // Check if there are any configs for this mod.
                if (!configs.TryGetValue(modInstance, out List<ModConfig> modConfigs) || modConfigs.Count == 0)
                {
                    Log.Info("No config available for mod: " + modName);
                    return;
                }

                // Use the first available config.
                ModConfig config = modConfigs[0];

                // Open the config UI.
                // Use reflection to set the mod and config for the modConfig UI.
                Assembly assembly = typeof(Main).Assembly;
                Type interfaceType = assembly.GetType("Terraria.ModLoader.UI.Interface");
                var modConfigField = interfaceType.GetField("modConfig", BindingFlags.Static | BindingFlags.NonPublic);
                var modConfigInstance = modConfigField.GetValue(null);
                var setModMethod = modConfigInstance.GetType().GetMethod("SetMod", BindingFlags.Instance | BindingFlags.NonPublic);

                // Invoke the SetMod method to set the mod and config for the modConfig UI.
                setModMethod.Invoke(modConfigInstance, [modInstance, config, false, null, null, true]);

                // Open the mod config UI.
                Main.InGameUI.SetState(modConfigInstance as UIState);
                Main.menuMode = 10024; // config UI (must set this!)
                Main.NewText("Opening config for " + modName, Color.Green);
            }
            catch (Exception ex)
            {
                Log.Info($"No config found for mod '{ModHelper.Instance.Name}'. : {ex.Message}");
                return;
            }
        }

        private static async Task ReloadSelectedMod()
        {
            // read the json and add the mods to the list
            List<string> modsToReloadFromJson = ModsToReloadJsonHelper.ReadModsToReload();
            ReloadUtilities.ModsToReload.Clear();
            foreach (var mod in modsToReloadFromJson)
            {
                ReloadUtilities.ModsToReload.Add(mod);
            }

            // Reload the selected mod
            await ReloadUtilities.SinglePlayerReload();
        }

        private static void HostMultiplayer()
        {
            // First, always load players and worlds
            // LoadPlayers() creates a crash with index out of range
            Main.LoadPlayers();
            if (Main.PlayerList == null || Main.PlayerList.Count == 0)
            {
                Log.Error("No players found after loading players.");
                return;
            }
            Main.LoadWorlds();

            // Select player and world based on json
            int playerID = ClientDataJsonHelper.PlayerID;
            int worldID = ClientDataJsonHelper.WorldID;

            var player = Main.PlayerList.FirstOrDefault();
            var world = Main.WorldList.FirstOrDefault();

            if (playerID == -1 || worldID == -1)
            {
                Log.Error("PlayerID or WorldID is -1. Cannot autoload.");
                // if we return here, we cause a "crash" or "stuck" in loading.
            }
            else
            {
                // all ok, continue.
                player = Main.PlayerList[ClientDataJsonHelper.PlayerID];
                world = Main.WorldList[ClientDataJsonHelper.WorldID];
            }

            Log.Info("HostMultiplayer. Found player: " + player.Name + ", world: " + world.Name);

            Main.SelectPlayer(player);
            Main.ActiveWorldFileData = world;

            // Main.menuMode = 889; // host & play menu
            Main.instance.OnSubmitServerPassword("");
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
            List<Player> activePlayers = Main.player.Where(p => p != null && p.active).ToList();

            // Get a list of all player file data
            // Select the first player that is not already in the active players list
            foreach (var p in Main.PlayerList)
            {
                Player player = p.Player;

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

        #region draw

        private static void DrawOutlinedStringOnMenu(SpriteBatch spriteBatch, DynamicSpriteFont font, string text,
            Vector2 position, Color drawColor, float rotation, Vector2 origin, float scale, SpriteEffects effects,
            float layerDepth, bool special = false, float alphaMult = 0.3f)
        {
            for (int i = 0; i < 5; i++)
            {
                Color color;
                if (i == 4) // draw the main text last
                {
                    color = drawColor;
                }
                else // Draw the outline first
                {
                    color = Color.Black;
                    if (special)
                    {
                        color.R = (byte)((255 + color.R) / 2);
                        color.G = (byte)((255 + color.G) / 2);
                        color.B = (byte)((255 + color.B) / 2);
                    }
                }
                // Adjust alpha
                color.A = (byte)(color.A * alphaMult);

                // Outline offsets
                int offX = 0;
                int offY = 0;
                switch (i)
                {
                    case 0: offX = -2; break;
                    case 1: offX = 2; break;
                    case 2: offY = -2; break;
                    case 3: offY = 2; break;
                }

                // Draw text
                spriteBatch.DrawString(font, text, position + new Vector2(offX, offY),
                color, rotation, origin, scale, effects, layerDepth);
            }
        }
        #endregion

    }
}