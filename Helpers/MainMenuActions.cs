using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.Social;
using static ModReloader.Common.Configs.Config;

namespace ModReloader.Helpers
{
    public static class MainMenuActions
    {
        public static string GetNextAvailableTestWorldName()
        {
            Main.LoadWorlds();
            var taken = new HashSet<string>(Main.WorldList.Select(w => w.Name), StringComparer.OrdinalIgnoreCase);

            const string baseName = "Test";
            int n = 1;
            while (taken.Contains($"{baseName}{n}"))
                n++;

            return $"{baseName}{n}";
        }

        public static void CreateNewWorld(string desiredName)
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
                    Main.maxTilesX = 2100;
                    Main.maxTilesY = 600;
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

        public static void StartClient()
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

        public static void StartServer()
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
    }
}