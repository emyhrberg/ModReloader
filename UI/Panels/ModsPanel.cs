using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class ModsPanel : OptionPanel
    {
        public ModsPanel() : base(title: "Mods List", scrollbarEnabled: true)
        {
            Asset<Texture2D> defaultIconTemp = Main.Assets.Request<Texture2D>("Images/UI/DefaultResourcePackIcon", AssetRequestMode.ImmediateLoad);

            AddHeader("Your Mods");
            foreach (var modPath in GetModFiles())
            {
                string modFolderName = Path.GetFileName(modPath);
                AddModItem(
                    name: modFolderName,
                    icon: defaultIconTemp.Value,
                    leftClick: () => ExitWorldAndBuildReloadMod(modFolderName),
                    hover: "Exit world and reload mod",
                    rightClick: null
                );
            }
            AddPadding();

            // Add mod items
            AddHeader("Enabled Mods");
            var mods = ModLoader.Mods.Skip(1);//ignore the built in Modloader mod
            foreach (var mod in mods)
            {
                AddModItem(
                    name: mod.DisplayNameClean,
                    icon: defaultIconTemp.Value,
                    leftClick: null,
                    hover: $"{mod.Name} (v{mod.Version})"
                );

                //     try
                //     {
                //         // Reflect to get LocalMod (the mod's compiled file + properties).
                //         // 'mod' is a Mod instance, but we need the LocalMod for reading its .tmod contents.
                //         var localModField = typeof(Mod).GetField("localMod", BindingFlags.NonPublic | BindingFlags.Instance);
                //         if (localModField == null)
                //             continue;

                //         var localMod = localModField.GetValue(mod);
                //         Type localModType = localMod.GetType();
                //         var modFileProperty = localModType.GetProperty("ModFile", BindingFlags.NonPublic | BindingFlags.Instance);
                //         var tmodFile = modFileProperty.GetValue(localMod) as TmodFile;
                //         if (localMod == null)
                //             continue;

                //         // localMod.ModFile is a TmodFile, which stores all the .tmod contents.

                //         // "icon.png" is the standard name for a mod's icon inside its .tmod file.
                //         if (!tmodFile.HasFile("icon.png"))
                //         {
                //             // fallback if no icon
                //             continue;
                //         }

                //         using var stream = tmodFile.GetStream("icon.png");
                //         // Load into Texture2D
                //         Texture2D iconTexture = Texture2D.FromStream(Main.graphics.GraphicsDevice, stream);

                //         AddModItem(
                //             name: mod.Name,
                //             icon: iconTexture,
                //             leftClick: () => ExitWorldAndBuildReloadMod(mod.Name),
                //             hover: $"{mod.Name} (v{mod.Version})"
                //         );
                //     }
                //     catch
                //     {
                //         Log.Info($"Failed to load icon for {mod.Name}");
                //     }
                // }
            }
        }

        private async void ExitWorldAndBuildReloadMod(string modFolderName = "")
        {
            if (modFolderName == "")
            {
                return;
            }

            // set mod to reload (just a config change)
            Config c = ModContent.GetInstance<Config>();
            c.ModToReload = modFolderName;

            await ReloadUtilities.ExitWorldOrServer();

            ReloadUtilities.ReloadMod();
            ReloadUtilities.BuildAndReloadMod();
        }

        private List<string> GetModFiles()
        {
            List<string> strings = [];

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
