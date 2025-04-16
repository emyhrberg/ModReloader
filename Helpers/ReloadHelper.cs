using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ModHelper.Common.Configs;
using Terraria;
namespace ModHelper.Helpers
{
    public static class ReloadHelper
    {
        /// <summary>
        /// Main function to build and reload all the mods in the ModsToReload list.
        /// </summary>
        public static async Task SinglePlayerReload()
        {
            await ExitWorld();
            BuildAndReloadMods();
        }

        // private static void ReloadMod()
        // {
        //     // Going to reload mod menu(that automaticly invokes reload)
        //     Main.menuMode = 10002;
        // }

        private static Task ExitWorld()
        {
            if (Conf.C.SaveWorld)
            {
                Log.Info("Saving and quitting...");
                // Creating task that will delay reloading a mod until world finish saving
                var tcs = new TaskCompletionSource();
                WorldGen.SaveAndQuit(tcs.SetResult);
                return tcs.Task;
            }
            else
            {
                Log.Info("Just quitting...");
                WorldGen.JustQuit();
                return Task.CompletedTask;
            }
        }

        private static void BuildAndReloadMods(Action actionAfterBuild = null)
        {
            List<string> mods = ["ModHelper"];

            // 2. Getting method for finding modSources paths
            Type modCompileType = typeof(Main).Assembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo findModSourcesMethod = modCompileType.GetMethod("FindModSources", BindingFlags.NonPublic | BindingFlags.Static);
            string[] modSources = (string[])findModSourcesMethod.Invoke(null, null);

            var modPaths = mods.Select((modName) =>
                modSources.FirstOrDefault(p =>
                    !string.IsNullOrEmpty(p) &&
                    Directory.Exists(p) &&
                    Path.GetFileName(p)?.Equals(modName, StringComparison.InvariantCultureIgnoreCase) == true));

            // 4. Getting method for reloading a mod
            // 4.1 Getting UIBuildMod Instance
            Type interfaceType = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.Interface");
            FieldInfo buildModField = interfaceType.GetField("buildMod", BindingFlags.NonPublic | BindingFlags.Static);
            object buildModInstance = buildModField?.GetValue(null);

            // 4.2 Getting correct BuildMod method of UIBuildMod
            Type uiBuildModType = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIBuildMod");
            MethodInfo buildModMethod = uiBuildModType.GetMethod("BuildMod", BindingFlags.Instance | BindingFlags.NonPublic, [typeof(Action<>).MakeGenericType(modCompileType), typeof(bool)]);

            // Check if it exist
            if (buildModMethod == null)
            {
                Log.Warn("No buildMethod were found via reflection.");
                return;
            }

            // 4.3 Getting correct Build method from ModCompile
            MethodInfo mcBuildModFolder = modCompileType.GetMethod("Build", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(string)]);

            Log.Info("Starting to build mods..." + string.Join(", ", modPaths));

            // 6. Creating a task
            Main.menuMode = 10003; // build mod menu ? what is this
            Task.Run(() =>
            {
                try
                {
                    return ((Task)buildModMethod.Invoke(buildModInstance,
                    [
                        (Action<object>) (mc =>
                        {
                            foreach (var modPath in modPaths)
                            {
                                Log.Info("Building mod: " + modPath);
                                try
                                {
                                    mcBuildModFolder.Invoke(mc, [modPath]);
                                }
                                catch (TargetInvocationException ex)
                                {
                                    throw ex.InnerException!;
                                }
                            }
                        }),
                        true
                    ])).ContinueWith(t =>
                    {
                        actionAfterBuild?.Invoke(); // Execute custom action after the method finishes
                    });
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException!; // ! means it will never be null
                }

            });
        }
    }
}