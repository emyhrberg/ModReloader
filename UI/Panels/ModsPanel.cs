using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class ModsPanel : RightParentPanel
    {
        public ModsPanel() : base(title: "Mods List", scrollbarEnabled: false)
        {
            var mods = ModLoader.Mods.Skip(1);//ignore the built in Modloader mod

            AddHeader("Enabled Mods");
            foreach (var mod in mods)
            {
                AddOnOffOption(null, title: mod.DisplayNameClean, hoverText: mod.Name + " (v" + mod.Version + ")");
            }
            AddPadding();

            // add header
            AddHeader("Your Mods");

            foreach (var modPath in GetModFiles())
            {
                string modFolderName = Path.GetFileName(modPath);
                AddOnOffOption(() => ExitWorldAndBuildReloadMod(modFolderName), title: modFolderName, hoverText: modPath + "\nCLICK AT YOUR OWN RISK");
            }

            AddPadding();

            // test mod items
            AddHeader("Test Mod Items");
            CustomModItem item1 = new();
            Append(item1);
        }

        private async void ExitWorldAndBuildReloadMod(string modFolderName)
        {
            // set mod to reload
            Config c = ModContent.GetInstance<Config>();
            c.ModToReload = modFolderName;

            await ReloadUtilities.ExitWorldOrServer();

            // wait 1 sec
            await Task.Delay(1000);

            ReloadUtilities.ReloadMod();

            // wait 3 sec
            await Task.Delay(3000);

            ReloadUtilities.BuildAndReloadMod();
        }

        private List<string> GetModFiles()
        {
            List<string> strings = new List<string>();

            // 1. Getting Assembly 
            Assembly tModLoaderAssembly = typeof(Main).Assembly;

            // 2. Gettig method for finding modSources paths
            Type modCompileType = tModLoaderAssembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo findModSourcesMethod = modCompileType.GetMethod("FindModSources", BindingFlags.NonPublic | BindingFlags.Static);
            string[] modSources = (string[])findModSourcesMethod.Invoke(null, null);

            for (int i = 0; i < modSources.Length; i++)
            {
                strings.Add(modSources[i]);
            }

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
            return strings;
        }
    }
}
