using System;
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
using MonoMod.RuntimeDetour;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.Common.Systems
{
    public class MainMenuHook : ModSystem
    {
        public override void Load()
        {
            On_Main.DrawVersionNumber += DrawMenuOptions;
        }

        private static void DrawMenuOptions(On_Main.orig_DrawVersionNumber orig, Color menucolor, float upbump)
        {
            // Draw vanilla stuff first
            orig(menucolor, upbump);

            // Only draw all this stuff if in main menu mode 0 (default main menu screen)
            if (Main.menuMode != 0) return;

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

            // Menu options with corresponding actions
            var menuOptions = new (string Text, Action Action, float scale)[]
            {
                ($"{mod.DisplayNameClean} v{mod.Version}", null, 1.15f),
                ("Join Singleplayer", JoinSingleplayer, 1.02f),
                ("Join Multiplayer", JoinMultiplayer, 1.02f),
                ("Host Multiplayer (todo)", null, 1.02f),
                ("Host Server", HostServer, 1.02f),
                ("Kill Server (todo)", null, 1.02f),
                ("Start Client", StartClient, 1.02f),
                ("Open Log", Log.OpenClientLog, 1.02f),
                ("Clear Log", Log.ClearClientLog, 1.02f),
                ("Open config", () => Conf.C.Open(), 1.02f)
            };

            foreach (var (text, action, scale) in menuOptions)
            {
                // Measure text
                Vector2 size = FontAssets.MouseText.Value.MeasureString(text) * 0.9f;
                // Check if mouse is hovering it
                bool hovered = Main.MouseScreen.Between(drawPos, drawPos + size);

                if (hovered)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    // Click
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                        Main.mouseLeftRelease = false;
                        action.Invoke(); // Call the corresponding action
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

                // Move down for the next line
                drawPos.Y += size.Y + 8f;
            }
        }

        #region actions

        private static void HostMultiplayer()
        {

        }

        private static void KillServer()
        {

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

        private static void HostServer()
        {
            try
            {
                Main.LoadWorlds();

                if (Main.WorldList.Count == 0)
                    throw new Exception("No worlds found.");

                // Getting Player and World from ClientDataHandler
                var world = Main.WorldList.FirstOrDefault();

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
            Log.Info("EnterMultiplayerWorld() called!");
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0)
                throw new Exception("No players found.");

            // Getting Player and World from ClientDataHandler
            var player = Main.PlayerList.FirstOrDefault();

            // TODO check if player is already in server and if so, join with a different player.

            Main.SelectPlayer(player);

            // Play the selected world in multiplayer mode
            // Connect to server IP
            Ping pingSender = new();
            PingOptions options = new();
            options.DontFragment = true; // prevent packet from splitting into smaller packets
            string data = "a"; // dummy data to send because the Send method requires it
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data); // convert string to byte array
            int timeout = 2000; // 120 ms timeout before the ping request is considered failed

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
            Log.Info("EnterSingleplayerWorld() called!");

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
            CustomLoadWorld.Show(world.Name);
        }

        #endregion

        #region draw

        private static void DrawOutlinedStringOnMenu(SpriteBatch spriteBatch, DynamicSpriteFont font, string text,
            Vector2 position, Color drawColor, float rotation, Vector2 origin, float scale, SpriteEffects effects,
            float layerDepth, bool special = false, float alphaMult = 0.3f)
        {
            for (int i = 0; i < 5; i++)
            {
                Color color = Color.Black;
                if (i == 4)
                {
                    color = drawColor;
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