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
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace SquidTestingMod.UI.Elements
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class ModsPanel : OptionPanel
    {
        private List<ModItem> myMods = [];
        private List<ModItem> enabledMods = [];

        public ModsPanel() : base(title: "Mods List", scrollbarEnabled: true)
        {
            Asset<Texture2D> defaultIconTemp = Main.Assets.Request<Texture2D>("Images/UI/DefaultResourcePackIcon", AssetRequestMode.ImmediateLoad);

            AddHeader("My Mods");
            AddPadding(5f);
            foreach (var modPath in GetModFiles())
            {
                // get folder path and icon
                string modFolderName = Path.GetFileName(modPath);
                // UIImage icon = GetIconImage(modPath);

                bool isSetToReload = modFolderName == Conf.ModToReload;
                Log.Info("Mod name: " + modFolderName + " isSetToReload: " + isSetToReload);

                // create mod item
                ModItem modItem = AddModItem(
                    isSetToReload: modFolderName == Conf.ModToReload,
                    name: modFolderName,
                    icon: defaultIconTemp.Value,
                    leftClick: () => SetModAsDefaultReloadMod(modFolderName),
                    hover: "Click to make this the default mod to reload"
                );
                myMods.Add(modItem);
                AddPadding(5f);
            }
            AddPadding();

            // Add mod items
            AddHeader("Enabled Mods");
            var mods = ModLoader.Mods.Skip(1);//ignore the built in Modloader mod
            foreach (var mod in mods)
            {
                AddModItem(
                    isSetToReload: false,
                    name: mod.DisplayNameClean,
                    icon: defaultIconTemp.Value,
                    leftClick: null, // this should disable mod in the future (if possible), so that we can select which mods to reload
                    hover: $"{mod.Name} (v{mod.Version})"
                );
                AddPadding(5f);
            }
            AddPadding();
        }

        private void SetModAsDefaultReloadMod(string modFolderName)
        {
            // set mod to reload (just a config change)
            Config c = ModContent.GetInstance<Config>();
            c.ModToReload = modFolderName;

            // update the UI opacity
            foreach (var item in myMods)
            {
                // Log.Info("Item.ModName: " + item.ModName);
                bool isSetToReload = item.ModName == modFolderName;
                if (isSetToReload)
                {
                    item.SetSelected(true);
                }
                else
                {
                    item.SetSelected(false);
                }
            }
        }

        private void DisableMod(string modName)
        {
            // update UI
            foreach (var item in enabledMods)
            {
                if (item.ModName == modName)
                {
                    item.SetEnabled(false);
                }
                else
                {
                    item.SetEnabled(true);
                }
            }

            // todo disable mod
        }

        public List<string> GetModFiles()
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
            return strings;
        }
    }
}
