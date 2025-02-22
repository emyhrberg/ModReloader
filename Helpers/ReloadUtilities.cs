using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.PacketHandlers;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace SquidTestingMod.Helpers
{
    //All functions, related to reload
    internal class ReloadUtilities
    {
        public static void PrepareClient(ClientMode clientMode)
        {
            ClientDataHandler.Mode = clientMode;
            ClientDataHandler.PlayerId = Utilities.FindPlayerId();
            ClientDataHandler.WorldId = Utilities.FindWorldId();
        }

        public static Task ExitWorldOrServer()
        {
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

        public static void BuildAndReloadMod()
        {
            // 1. Getting Assembly
            Assembly tModLoaderAssembly = typeof(Main).Assembly;

            // 2. Gettig method for finding modSources paths
            Type modCompileType = tModLoaderAssembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo findModSourcesMethod = modCompileType.GetMethod("FindModSources", BindingFlags.NonPublic | BindingFlags.Static);
            string[] modSources = (string[])findModSourcesMethod.Invoke(null, null);

            // 3. Finding path by ModToReload name
            string modPath = modSources.FirstOrDefault(p => Path.GetFileName(p) == Conf.ModToReload);
            if (modPath != null)
            {
                Log.Info($"Path to {Conf.ModToReload}: {modPath}");
            }
            else
            {
                Console.WriteLine("No path found");
            }

            // 4. Getting method for reloading a mod
            Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");
            FieldInfo buildModField = interfaceType.GetField("buildMod", BindingFlags.NonPublic | BindingFlags.Static);
            object buildModInstance = buildModField?.GetValue(null);
            Type uiBuildModType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.UIBuildMod");
            MethodInfo buildMethod = uiBuildModType.GetMethod("Build", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(string), typeof(bool)]);

            // 5.Invoking a Build method
            buildMethod.Invoke(buildModInstance, [modPath, true]);
        }
    }
}