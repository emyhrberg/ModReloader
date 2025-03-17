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
        private List<ModItem> modSources = [];
        private List<ModItem> enabledMods = [];

        public ModsPanel() : base(title: "Mods List", scrollbarEnabled: true)
        {
            Asset<Texture2D> defaultIconTemp = Main.Assets.Request<Texture2D>("Images/UI/DefaultResourcePackIcon", AssetRequestMode.ImmediateLoad);

            AddHeader("Mod Sources");
            AddPadding(3f);

            // Get the currently selected mod from config
            string selectedMod = Conf.ModToReload;
            bool foundSelectedMod = false;

            foreach (var modPath in GetModFiles())
            {
                // Get folder path and icon
                string modFolderName = Path.GetFileName(modPath);
                bool isSet = modFolderName == selectedMod;

                if (isSet)
                {
                    foundSelectedMod = true;
                    Log.Info($"Found selected mod: {modFolderName}");
                }

                // Get icon
                // Try to load the mod's icon from the file system
                Texture2D modIcon = defaultIconTemp.Value;
                string iconPath = Path.Combine(modPath, "icon.png");

                if (File.Exists(iconPath))
                {
                    try
                    {
                        // Load the icon texture from file
                        using FileStream stream = new FileStream(iconPath, FileMode.Open);
                        modIcon = Texture2D.FromStream(Main.graphics.GraphicsDevice, stream);
                        Log.Info($"Loaded custom icon for mod {modFolderName}");
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Failed to load icon for mod {modFolderName}: {ex.Message}");
                    }
                }

                // Create mod item
                ModItem modItem = AddModItem(
                    isSetToReload: isSet,
                    name: modFolderName,
                    icon: modIcon,
                    leftClick: () => OnClickMyMod(modFolderName),
                    hover: "Click to make this the mod to reload"
                );
                modSources.Add(modItem);

                // Explicitly set the state based on whether it's the selected mod
                modItem.SetState(isSet ? ModItem.ModItemState.Selected : ModItem.ModItemState.Unselected);
            }

            // If no selected mod was found in the config, select the first one
            if (!foundSelectedMod && modSources.Count > 0)
            {
                string firstModName = modSources[0].ModName;
                Log.Info($"No mod matched '{selectedMod}', defaulting to first mod: {firstModName}");

                // Update config
                Config c = ModContent.GetInstance<Config>();
                c.ModToReload = firstModName;
                ConfigUtilities.SaveConfig(c);

                // Update UI
                modSources[0].SetState(ModItem.ModItemState.Selected);
            }

            AddPadding();

            // Add mod items
            AddHeader("Enabled Mods");
            AddPadding(3f);
            var mods = ModLoader.Mods.Skip(1);//ignore the built in Modloader mod
            foreach (var mod in mods)
            {
                var modItem = AddModItem(
                    isSetToReload: false,
                    name: mod.DisplayNameClean,
                    icon: defaultIconTemp.Value,
                    leftClick: () => OnClickEnabledMod(mod.DisplayNameClean),
                    hover: $"{mod.Name} (v{mod.Version})"
                );
                enabledMods.Add(modItem);

                // Make sure all enabled mods start in the Default state
                modItem.SetState(ModItem.ModItemState.Default);
            }
        }

        private void OnClickMyMod(string modFolderName)
        {
            // set mod to reload (just a config change)
            Config c = ModContent.GetInstance<Config>();
            c.ModToReload = modFolderName;
            ConfigUtilities.SaveConfig(c);

            // set color of this mod to green
            foreach (var modItem in modSources)
            {
                if (modItem.ModName == modFolderName)
                {
                    modItem.SetState(ModItem.ModItemState.Selected);
                }
                else
                {
                    modItem.SetState(ModItem.ModItemState.Unselected);
                }
            }
        }

        private void OnClickEnabledMod(string modName)
        {
            foreach (var modItem in enabledMods)
            {
                if (modItem.ModName == modName)
                {
                    if (modItem.state == ModItem.ModItemState.Default)
                    {
                        // enable mod
                        modItem.SetState(ModItem.ModItemState.Disabled);
                    }
                    else
                    {
                        // disable mod
                        modItem.SetState(ModItem.ModItemState.Default);
                    }
                }
            }
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
