using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EliteTestingMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using static EliteTestingMod.UI.Elements.Option;

namespace EliteTestingMod.UI.Elements
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class ModsPanel : OptionPanel
    {
        public List<ModSourcesElement> modSourcesElements = [];

        // enabled mods
        private List<ModElement> modElements = [];
        private Option toggleAllEnabledMods;

        // disabled mods
        public List<ModElement> allMods = [];
        private Option toggleAllAllMods;

        public ModsPanel() : base(title: "Mods", scrollbarEnabled: true)
        {
            Active = true; // uncomment to show the panel by default
            AddPadding(5);
            AddHeader("Mod Sources", GoToModSources, "Click to exit world and go to mod sources");
            ConstructModSourcesList();
            AddPadding();

            AddHeader("Enabled Mods", onLeftClick: GoToModsList, "Click to exit world and go to mods list");
            ConstructEnabledModsList();
            toggleAllEnabledMods = AddOption("Toggle All", leftClick: ToggleAllEnabledMods, hover: "Toggle all enabled mods on or off");
            toggleAllEnabledMods.SetState(State.Enabled);
            AddPadding();

            AddHeader("All Mods", onLeftClick: GoToModsList, "Click to exit world and go to mods list");
            ConstructAllMods();
            toggleAllAllMods = AddOption("Toggle All", leftClick: ToggleAllAllMods, hover: "Toggle all disabled mods on or off");
            AddPadding();
            AddPadding(3f);


            // Reflection from
            // ModOrganizer::internal static LocalMod[] FindMods()
            Assembly assembly = typeof(ModLoader).Assembly;
            Type ModOrganizer = assembly.GetType("Terraria.ModLoader.Core.ModOrganizer");
            MethodInfo FindMods = ModOrganizer.GetMethod("FindMods", BindingFlags.NonPublic | BindingFlags.Static);
            var mods = (IReadOnlyList<object>)FindMods.Invoke(null, [false]);
        }

        private void ConstructAllMods()
        {
            // Get all mods the user has installed via reflection
            // ModOrganizer.FindAllMods
            try
            {
                Assembly assembly = typeof(ModLoader).Assembly;
                Type modOrganizerType = assembly.GetType("Terraria.ModLoader.Core.ModOrganizer");
                MethodInfo findWorkshopModsMethod = modOrganizerType.GetMethod("FindWorkshopMods", BindingFlags.NonPublic | BindingFlags.Static);

                var workshopMods = (IReadOnlyList<object>)findWorkshopModsMethod.Invoke(null, null);

                foreach (var mod in workshopMods)
                {
                    // Get the clean name using reflection for the LocalMod mod.
                    FieldInfo displayNameField = mod.GetType().GetField("DisplayNameClean", BindingFlags.Public | BindingFlags.Instance);
                    string cleanName = (string)displayNameField.GetValue(mod);

                    string internalName = mod.ToString();
                    // check if mod already exists in enabledmods.
                    // Log.Info("enabled mods: " + string.Join(", ", modElements.Select(modElement => modElement.internalName)));

                    if (modElements.Any(modElement => modElement.internalName == internalName))
                    {
                        Log.Info("Mod already exists in enabled mods: " + internalName);
                        continue;
                    }

                    // "mod" is of type LocalMod 
                    // We want to pass the LocalMod's TmodFile to ModElement.
                    Type localModType = assembly.GetType("Terraria.ModLoader.Core.LocalMod");

                    FieldInfo modFileField = localModType.GetField("modFile", BindingFlags.Public | BindingFlags.Instance);
                    object tmod = modFileField.GetValue(mod);

                    Texture2D modIcon = GetModIconFromAllMods(tmod);

                    ModElement modElement = new(
                        modName: cleanName,
                        internalModName: internalName,
                        icon: modIcon
                        );


                    modElement.SetState(State.Disabled);
                    uiList.Add(modElement);
                    allMods.Add(modElement);
                    AddPadding(3);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("An error occurred while retrieving workshop mods." + ex);
            }
            AddPadding(3);
        }

        private Texture2D GetModIconFromAllMods(object TmodFile)
        {
            try
            {
                // Check if the file exists
                MethodInfo hasFileMethod = TmodFile.GetType().GetMethod("HasFile", BindingFlags.Public | BindingFlags.Instance);
                bool hasIcon = (bool)hasFileMethod.Invoke(TmodFile, ["icon.png"]);
                if (!hasIcon)
                {
                    Log.SlowInfo("The TmodFile does not have an icon.");
                    return null;
                }

                // Retrieve the Open method (no parameters).
                MethodInfo openMethod = TmodFile.GetType().GetMethod("Open", BindingFlags.Public | BindingFlags.Instance);

                // Retrieve the GetStream method that takes a string and a bool.
                MethodInfo getStreamMethod = TmodFile.GetType().GetMethod(
                    "GetStream",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    [typeof(string), typeof(bool)],
                    null
                );

                // Use a nested using block so both the open result and the stream are disposed.
                using var openResult = openMethod.Invoke(TmodFile, []) as IDisposable;

                // Get the stream for "icon.png". Note we pass both parameters.
                using Stream s = (Stream)getStreamMethod.Invoke(TmodFile, ["icon.png", true]);

                Asset<Texture2D> iconTexture = Main.Assets.CreateUntracked<Texture2D>(s, ".png", AssetRequestMode.ImmediateLoad);
                Log.SlowInfo("Successfully loaded icon from TmodFile.");
                return iconTexture.Value;
            }
            catch (Exception ex)
            {
                Log.SlowInfo("Error while retrieving icon from TmodFile via reflection: " + ex);
            }
            return null;
        }

        private void ToggleAllAllMods()
        {
            // Determine the new state based on whether all mods are currently enabled
            bool anyDisabled = allMods.Any(modElement => modElement.GetState() == State.Disabled);
            State newState = anyDisabled ? State.Enabled : State.Disabled;

            // Set the state for all mod elements
            foreach (ModElement modElement in allMods)
            {
                modElement.SetState(newState);
                string internalName = modElement.internalName; // Assuming InternalName is a property of ModElement

                // Use reflection to call SetModEnabled on internalModName
                var setModEnabled = typeof(ModLoader).GetMethod("SetModEnabled", BindingFlags.NonPublic | BindingFlags.Static);
                setModEnabled?.Invoke(null, [internalName, newState == State.Enabled]);
            }

            // Update the "Toggle All" option's state
            toggleAllAllMods.SetState(newState);
        }

        private void ToggleAllEnabledMods()
        {
            // Determine the new state based on whether all mods are currently enabled
            bool anyDisabled = modElements.Any(modElement => modElement.GetState() == State.Disabled);
            State newState = anyDisabled ? State.Enabled : State.Disabled;

            // Set the state for all mod elements
            foreach (ModElement modElement in modElements)
            {
                modElement.SetState(newState);
                string internalName = modElement.internalName; // Assuming InternalName is a property of ModElement

                // Use reflection to call SetModEnabled on internalModName
                var setModEnabled = typeof(ModLoader).GetMethod("SetModEnabled", BindingFlags.NonPublic | BindingFlags.Static);
                setModEnabled?.Invoke(null, [internalName, newState == State.Enabled]);
            }

            // Update the "Toggle All" option's state
            toggleAllEnabledMods.SetState(newState);
        }

        private void ConstructEnabledModsList()
        {
            var mods = ModLoader.Mods.Skip(1);//ignore the built in Modloader mod
            foreach (Mod mod in mods)
            {
                // you could change this to send the "clean name"
                // this is where we set the text of the mod element
                ModElement modElement = new(mod.DisplayNameClean, mod.Name);
                uiList.Add(modElement);
                modElements.Add(modElement);
                AddPadding(3);
            }
        }

        private void ConstructModSourcesList()
        {
            // Create a new ModSourcesElement : PanelElement for each mod in modsources.
            foreach (string modPath in GetModSourcesPaths())
            {
                string cleanName = GetModSourcesCleanName(modPath);

                // cut to max 20 chars
                if (cleanName.Length > 20)
                    cleanName = string.Concat(cleanName.AsSpan(0, 20), "...");
                // string cleanName = Path.GetFileName(modPath);

                ModSourcesElement modSourcesElement = new(modPath: modPath, cleanName: cleanName);
                modSourcesElements.Add(modSourcesElement);
                uiList.Add(modSourcesElement);
                AddPadding(3);
            }
        }

        private string GetModSourcesCleanName(string modFolder)
        {
            // Get the assembly and the ModCompile type.
            Assembly assembly = typeof(ModLoader).Assembly;
            Type modCompileType = assembly.GetType("Terraria.ModLoader.Core.ModCompile");

            // Get the non-public nested type "ConsoleBuildStatus".
            Type consoleBuildStatusType = modCompileType.GetNestedType("ConsoleBuildStatus", BindingFlags.NonPublic);
            // Create an instance of ConsoleBuildStatus.
            object consoleBuildStatusInstance = Activator.CreateInstance(consoleBuildStatusType, nonPublic: true);

            // Create an instance of ModCompile using the constructor that takes an IBuildStatus.
            object modCompileInstance = Activator.CreateInstance(
                modCompileType,
                BindingFlags.Public | BindingFlags.Instance,
                null,
                [consoleBuildStatusInstance],
                null);

            // Retrieve the private instance method ReadBuildInfo.
            MethodInfo readBuildInfoMethod = modCompileType.GetMethod("ReadBuildInfo", BindingFlags.NonPublic | BindingFlags.Instance);
            // Invoke the method on the instance.
            object buildingMod = readBuildInfoMethod.Invoke(modCompileInstance, [modFolder]);

            // Since DisplayNameClean is a field, use GetField instead of GetProperty.
            FieldInfo displayNameField = buildingMod.GetType().GetField("DisplayNameClean", BindingFlags.Public | BindingFlags.Instance);
            return (string)displayNameField?.GetValue(buildingMod);
        }

        private List<string> GetModSourcesPaths()
        {
            List<string> strings = [];

            // 1. Getting Assembly 
            Assembly assembly = typeof(Main).Assembly;

            // 2. Gettig method for finding modSources paths
            Type modCompileType = assembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo findModSourcesMethod = modCompileType.GetMethod("FindModSources", BindingFlags.NonPublic | BindingFlags.Static);
            string[] modSources = (string[])findModSourcesMethod.Invoke(null, null);

            for (int i = 0; i < modSources.Length; i++)
            {
                strings.Add(modSources[i]);
            }
            return strings;
        }

        private void GoToModSources()
        {
            WorldGen.JustQuit();
            Main.menuMode = 10001;
        }

        private void GoToModsList()
        {
            WorldGen.JustQuit();
            Main.menuMode = 10000;
        }
    }
}
