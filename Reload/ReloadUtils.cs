using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;

namespace SquidTestingMod.Reload
{

    /// <summary>
    /// This class provies a set of utilities for reloading the set mod.
    /// It does so by saving the player and world data in the window title.
    /// It then reads the data and reloads the mod.
    /// <see cref="ReloadSystem"/>
    /// Calls ReadData and WriteData to read and write data.
    /// <see cref="SquidTestingMod"
    /// </summary>
    public class ReloadUtils
    {
        public static int PlayerId = 0;
        public static int WorldId = 0;

        public static void WriteData()
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
                return;

            Log.Info("Writing Data");
            // Write the data in a predictable format.
            Main.instance.Window.Title = $"{PlayerId}, {WorldId}";
        }

        public static void ReadData()
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
                return;

            Log.Info("Reading Data");
            string[] titleParts = Main.instance.Window.Title.Split([',']);

            if (titleParts.Length != 2)
            {
                Log.Error("Failed to parse client data from window title.");
                return;
            }

            if (int.TryParse(titleParts[0], out int parsedPlayerId) &&
            int.TryParse(titleParts[1], out int parsedWorldId))
            {
                PlayerId = parsedPlayerId;
                WorldId = parsedWorldId;
            }
            else
            {
                Log.Error("Failed to parse client data from window title.");
            }
        }

        public static async Task ReloadEverything()
        {
            // 1: Clear logs if needed
            if (Conf.ClearClientLogOnReload)
                Log.ClearClientLog();

            // 2: Set client player and world
            Main.LoadPlayers();
            Main.LoadWorlds();
            PlayerId = Main.PlayerList.FindIndex(p => p.Path == Main.ActivePlayerFileData.Path);
            WorldId = Main.WorldList.FindIndex(w => w.Path == Main.ActiveWorldFileData.Path);
            Log.Info("Setting playerID: " + PlayerId + ", worldID: " + WorldId);

            // 3: Exit world
            if (Conf.SaveWorldOnReload)
            {
                Log.Info("Saving and quitting...");

                // Creating task that will delay reloading a mod until world finish saving
                var tcs = new TaskCompletionSource();
                WorldGen.SaveAndQuit(tcs.SetResult);
                await tcs.Task;
            }
            else
            {
                Log.Info("Just quitting...");
                WorldGen.JustQuit();
            }

            // 4: Build and reload
            BuildAndReload();
        }

        // Long method but basically just rebuilds the mod with a lot of reflection.
        // Uses BuildMod from Interface and Build from UIBuildMod
        private static void BuildAndReload()
        {
            Assembly assembly = typeof(Main).Assembly;

            // Get our mods with FindModSources via reflection
            Type modCompileType = assembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo findModSources = modCompileType.GetMethod("FindModSources", BindingFlags.Static | BindingFlags.NonPublic);

            // Invoke the FindModSources method to get the mod sources.
            string[] modSources = (string[])findModSources.Invoke(null, null);
            if (modSources == null || modSources.Length == 0)
                return;

            // Get the mod path that matches the configured mod name.
            string modPath = modSources.FirstOrDefault(path => Path.GetFileName(path) == Conf.ModToReload);
            if (string.IsNullOrEmpty(modPath))
                return;

            Log.Info($"Found mod path to reload: {modPath}");

            // Get the build mod instance.
            Type interfaceType = assembly.GetType("Terraria.ModLoader.UI.Interface");
            Type uiBuildModType = assembly.GetType("Terraria.ModLoader.UI.UIBuildMod");
            MethodInfo buildMethod = uiBuildModType.GetMethod("Build", BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(string), typeof(bool)], null);
            FieldInfo buildModField = interfaceType.GetField("buildMod", BindingFlags.NonPublic | BindingFlags.Static);
            object buildModInstance = buildModField.GetValue(null);

            // Invoke the Build method to rebuild the mod.
            buildMethod.Invoke(buildModInstance, [modPath, true]);
        }
    }
}

