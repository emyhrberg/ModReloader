using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MonoMod.RuntimeDetour;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.PacketHandlers;
using Terraria;

namespace SquidTestingMod.Helpers
{
    //All functions, related to reload
    internal class ReloadUtilities
    {
        public const string pipeName = "SquidTestingModPipe";
        public const string pipeNameAfterRebuild = "SquidTestingModPipeAfterRebuild";

        //public static NamedPipeServerStream Pipe {  get; set; }\

        public static void PrepareClient(ClientModes clientMode)
        {
            ClientDataHandler.ClientMode = clientMode;
            ClientDataHandler.PlayerID = Utilities.FindPlayerId();
            ClientDataHandler.WorldID = Utilities.FindWorldId();
        }

        public static Task ExitWorldOrServer()
        {
            // TODO check if Conf is null
            if (Conf.C == null) // Assuming 'Instance' is a static property or field in 'Conf'
            {
                Log.Warn("Conf is null");
                return Task.CompletedTask;
            }

            if (Conf.SaveWorldOnReload)
            {
                Log.Warn("Saving and quitting...");

                // Creating task that will delay reloading a mod until world finish saving
                var tcs = new TaskCompletionSource();
                WorldGen.SaveAndQuit(tcs.SetResult);
                return tcs.Task;
            }
            else
            {
                Log.Warn("Just quitting...");
                WorldGen.JustQuit();
                return Task.CompletedTask;
            }

        }

        public static Task ExitAndKillServer()
        {
            // Sending packet to server to inform about reloading mod in multiplayer
            ModNetHandler.RefreshServer.SendKillingServer(255, Main.myPlayer, Conf.SaveWorldOnReload);

            // idk if that needed for exiting server, but maybe we need to save player data idk
            var tcs = new TaskCompletionSource();
            WorldGen.SaveAndQuit(tcs.SetResult);
            return tcs.Task;
        }

        public static void ReloadMod()
        {
            // Going to reload mod menu(that automaticly invokes reload)
            Main.menuMode = 10002;
        }

        public static void BuildAndReloadMods(Action actionAfterBuild = null)
        {
            // 1. Getting Assembly
            Assembly tModLoaderAssembly = typeof(Main).Assembly;

            // 2. Getting method for finding modSources paths
            Type modCompileType = tModLoaderAssembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo findModSourcesMethod = modCompileType.GetMethod("FindModSources", BindingFlags.NonPublic | BindingFlags.Static);
            string[] modSources = (string[])findModSourcesMethod.Invoke(null, null);

            // Check if modSources is null or empty.
            if (modSources == null || modSources.Length == 0)
            {
                Log.Warn("No mod sources were found via reflection.");
                return;
            }

            // 3. Get all modPaths for future
            var modPaths = ModsToReload.modsToReload.Select((modName) =>
                modSources.FirstOrDefault(p =>
                    !string.IsNullOrEmpty(p) &&
                    Directory.Exists(p) &&
                    Path.GetFileName(p)?.Equals(modName, StringComparison.InvariantCultureIgnoreCase) == true));

            // 4. Getting method for reloading a mod
            // 4.1 Getting UIBuildMod Instance
            Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");
            FieldInfo buildModField = interfaceType.GetField("buildMod", BindingFlags.NonPublic | BindingFlags.Static);
            object buildModInstance = buildModField?.GetValue(null);

            // 4.2 Getting correct BuildMod method of UIBuildMod
            Type uiBuildModType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.UIBuildMod");
            MethodInfo buildModMethod = uiBuildModType.GetMethod("BuildMod", BindingFlags.Instance | BindingFlags.NonPublic, [typeof(Action<>).MakeGenericType(modCompileType), typeof(bool)]);

            // Check if it exist
            if (buildModMethod == null)
            {
                Log.Warn("No buildMethod were found via reflection.");
                return;
            }

            // 4.3 Getting correct Build method from ModCompile
            MethodInfo mcBuildModFolder = modCompileType.GetMethod("Build", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(string)]);

            // 5. Setting a hook on BuildMod method of UIBuildMod
            Hook buildModMethodHook = null;
            buildModMethodHook = new Hook(buildModMethod, (Func<object, Action<object>, bool, Task> orig, object self, Action<object> buildAction, bool reload) =>
            {
                Task origTask = orig(self, buildAction, reload); // Call original method correctly

                return origTask.ContinueWith(t =>
                {
                    actionAfterBuild?.Invoke(); // Execute custom action after the method finishes
                    buildModMethodHook?.Dispose(); // Disable hook
                });
            });

            // 6. Creating a task
            Main.menuMode = 10003;
            Task.Run(() => (Task)buildModMethod.Invoke(buildModInstance, [(object mc) => {
                foreach (var modPath in modPaths) {
                    mcBuildModFolder.Invoke(mc, [modPath]);
                }
            }, true]));
        }

        public static void BuildAndReloadMod(Action actionAfterBuild = null)
        {
            // 1. Getting Assembly
            Assembly tModLoaderAssembly = typeof(Main).Assembly;

            // 2. Gettig method for finding modSources paths
            Type modCompileType = tModLoaderAssembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo findModSourcesMethod = modCompileType.GetMethod("FindModSources", BindingFlags.NonPublic | BindingFlags.Static);
            string[] modSources = (string[])findModSourcesMethod.Invoke(null, null);

            // Check if modSources is null or empty.
            if (modSources == null || modSources.Length == 0)
            {
                Log.Warn("No mod sources were found via reflection.");
                return;
            }

            // Optionally, log what modSources contains:
            foreach (var src in modSources)
            {
                if (src == null)
                {
                    Log.Warn("A mod source entry is null.");
                    continue;
                }
                Log.Info($"Found mod source: {src}");
            }

            // 3. Find the mod folder that matches the desired mod name.

            string modPath = modSources.FirstOrDefault(p =>
                !string.IsNullOrEmpty(p) &&
                Directory.Exists(p) &&
                Path.GetFileName(p)?.Equals(Conf.ModToReload, StringComparison.InvariantCultureIgnoreCase) == true);

            if (modPath != null)
            {
                Log.Info($"Path to {Conf.ModToReload}: {modPath}");
            }
            else
            {
                Log.Warn($"No mod path found matching {Conf.ModToReload}.");
                return;
            }

            // 4. Getting method for reloading a mod
            Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");
            FieldInfo buildModField = interfaceType.GetField("buildMod", BindingFlags.NonPublic | BindingFlags.Static);
            object buildModInstance = buildModField?.GetValue(null);
            Type uiBuildModType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.UIBuildMod");
            MethodInfo buildMethod = uiBuildModType.GetMethod("Build", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(string), typeof(bool)]);

            Hook buildHook = null;

            buildHook = new Hook(buildMethod, (Action<object, string, bool> orig, object self, string path, bool reload) =>
            {
                orig(self, path, reload); // Call original method correctly
                actionAfterBuild?.Invoke(); // Execute custom action after the method
                buildHook?.Dispose(); // Disable hook
            });

            // 5.Invoking a Build method
            buildMethod.Invoke(buildModInstance, [modPath, true]);
        }

        //string can be replaced with json if needed
        //for me the fact of sending messages would be enough
        public static async Task<string?> ReadPipeMessage(NamedPipeServerStream pipe)
        {
            using StreamReader reader = new StreamReader(pipe);
            return await reader.ReadLineAsync();
        }
    }
}