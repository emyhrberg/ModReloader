// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Reflection;
// using SquidTestingMod.Common.Configs;
// using SquidTestingMod.Helpers;
// using Terraria;
// using Terraria.ModLoader;

// namespace SquidTestingMod.UI.Elements
// {
//     /// <summary>
//     /// A panel to display the contents of client.log.
//     /// </summary>
//     public class ModsPanel : OptionPanel
//     {
//         public List<ModItemPanel> modSources = [];
//         public List<ModItemPanel> enabledMods = [];

//         public ModsPanel() : base(title: "Mods List", scrollbarEnabled: true)
//         {
//             AddHeader("Mod Sources");
//             AddPadding(3f);

//             // Get the currently selected mod from config
//             string selectedMod = Conf.ModToReload;
//             bool foundSelectedMod = false;

//             foreach (var modPath in GetModFiles())
//             {
//                 // Get folder path and icon
//                 string modFolderName = Path.GetFileName(modPath);
//                 bool isSet = modFolderName == selectedMod;

//                 if (isSet)
//                 {
//                     foundSelectedMod = true;
//                     //Log.Info($"Found selected mod: {modFolderName}");
//                 }

//                 // Get modPath
//                 string modPath3 = "";
//                 string modPath2 = Path.Combine(modPath, "icon.png");
//                 if (modPath2 != "")
//                 {
//                     modPath3 = modPath2;
//                     Log.Info("found icon");
//                 }

//                 // Create mod item
//                 ModItemPanel modItem = AddModItem(
//                     isSetToReload: isSet,
//                     modName: modFolderName,
//                     modPath: modPath3,
//                     leftClick: () => OnClickMyMod(modFolderName),
//                     hover: "Click to make this the mod to reload"
//                 );
//                 modSources.Add(modItem);

//                 // Explicitly set the state based on whether it's the selected mod
//                 modItem.SetState(isSet ? ModItemPanel.ModItemState.Selected : ModItemPanel.ModItemState.Unselected);
//             }

//             // If no selected mod was found in the config, select the first one
//             if (!foundSelectedMod && modSources.Count > 0)
//             {
//                 string firstModName = modSources[0].ModName;
//                 Log.Info($"No mod matched '{selectedMod}', defaulting to first mod: {firstModName}");

//                 // Update config
//                 Config c = ModContent.GetInstance<Config>();
//                 c.ModToReload = firstModName;
//                 ConfigUtilities.SaveConfig(c);

//                 // Update UI
//                 modSources[0].SetState(ModItemPanel.ModItemState.Selected);
//             }

//             AddPadding();

//             // Add mod items
//             AddHeader("Enabled Mods");
//             AddPadding(3f);
//             var mods = ModLoader.Mods.Skip(1);//ignore the built in Modloader mod
//             foreach (Mod mod in mods)
//             {
//                 // get icon texture
//                 //Texture2D modTex = ModContent.GetTexture(mod.File.GetIconPath());

//                 string path = $"{mod.Name}/icon";

//                 var modItem = AddModItem(
//                     isSetToReload: false,
//                     modName: mod.DisplayNameClean,
//                     modPath: path,
//                     leftClick: () => OnClickEnabledMod(mod.DisplayNameClean),
//                     hover: "Click to disabled this mod"
//                 );
//                 enabledMods.Add(modItem);

//                 // Make sure all enabled mods start in the Default state
//                 modItem.SetState(ModItemPanel.ModItemState.Default);
//             }
//         }

//         private void OnClickMyMod(string modFolderName)
//         {
//             // set mod to reload (just a config change)
//             Config c = ModContent.GetInstance<Config>();
//             c.ModToReload = modFolderName;
//             ConfigUtilities.SaveConfig(c);

//             // set color of this mod to green
//             foreach (var modItem in modSources)
//             {
//                 if (modItem.ModName == modFolderName)
//                 {
//                     modItem.SetState(ModItemPanel.ModItemState.Selected);
//                 }
//                 else
//                 {
//                     modItem.SetState(ModItemPanel.ModItemState.Unselected);
//                 }
//             }
//         }

//         private void OnClickEnabledMod(string modName)
//         {
//             foreach (var modItem in enabledMods)
//             {
//                 if (modItem.ModName == modName)
//                 {
//                     if (modItem.state == ModItemPanel.ModItemState.Default)
//                     {
//                         // enable mod
//                         modItem.SetState(ModItemPanel.ModItemState.Disabled);
//                     }
//                     else
//                     {
//                         // disable mod
//                         modItem.SetState(ModItemPanel.ModItemState.Default);
//                     }
//                 }
//             }
//         }

//         public List<string> GetModFiles()
//         {
//             List<string> strings = [];

//             // 1. Getting Assembly 
//             Assembly tModLoaderAssembly = typeof(Main).Assembly;

//             // 2. Gettig method for finding modSources paths
//             Type modCompileType = tModLoaderAssembly.GetType("Terraria.ModLoader.Core.ModCompile");
//             MethodInfo findModSourcesMethod = modCompileType.GetMethod("FindModSources", BindingFlags.NonPublic | BindingFlags.Static);
//             string[] modSources = (string[])findModSourcesMethod.Invoke(null, null);

//             for (int i = 0; i < modSources.Length; i++)
//             {
//                 strings.Add(modSources[i]);
//             }
//             return strings;
//         }
//     }
// }
