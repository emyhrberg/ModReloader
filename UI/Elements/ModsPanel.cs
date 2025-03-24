using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using static SquidTestingMod.UI.Elements.Option;

namespace SquidTestingMod.UI.Elements
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
        public List<ModElement> disabledMods = [];
        private Option toggleAllDisabledMods;

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
            ConstructDisabledMods();
            toggleAllDisabledMods = AddOption("Toggle All", leftClick: ToggleAllDisabledMods, hover: "Toggle all disabled mods on or off");
            AddPadding();
            AddPadding(3f);
        }

        private void ConstructDisabledMods()
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
                    string modName = mod.ToString();
                    // check if mod already exists in enabledmods.
                    // Log.Info("enabled mods: " + string.Join(", ", modElements.Select(modElement => modElement.internalName)));

                    if (modElements.Any(modElement => modElement.internalName == modName))
                    {
                        Log.Info("Mod already exists in enabled mods: " + modName);
                        continue;
                    }

                    // "mod" is of type LocalMod 
                    // We want to pass the LocalMod's TmodFile to ModElement.
                    Type localModType = assembly.GetType("Terraria.ModLoader.Core.LocalMod");

                    FieldInfo modFileField = localModType.GetField("modFile", BindingFlags.Public | BindingFlags.Instance);
                    object tmod = modFileField.GetValue(mod);


                    Texture2D disabledIcon = GetDisabledIcon(tmod);

                    ModElement modElement = new(
                        modName: modName,
                        internalModName: modName,
                        icon: disabledIcon);


                    modElement.SetState(State.Disabled);
                    uiList.Add(modElement);
                    disabledMods.Add(modElement);
                    AddPadding(3);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("An error occurred while retrieving workshop mods." + ex);
            }
            AddPadding(3);
        }

        private Texture2D GetDisabledIcon(object TmodFile)
        {
            try
            {
                // Check if the file exists
                MethodInfo hasFileMethod = TmodFile.GetType().GetMethod("HasFile", BindingFlags.Public | BindingFlags.Instance);
                bool hasIcon = (bool)hasFileMethod.Invoke(TmodFile, new object[] { "icon.png" });
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

        private void ToggleAllDisabledMods()
        {
            // Determine the new state based on whether all mods are currently enabled
            bool anyDisabled = disabledMods.Any(modElement => modElement.GetState() == State.Disabled);
            State newState = anyDisabled ? State.Enabled : State.Disabled;

            // Set the state for all mod elements
            foreach (ModElement modElement in disabledMods)
            {
                modElement.SetState(newState);
                string internalName = modElement.internalName; // Assuming InternalName is a property of ModElement

                // Use reflection to call SetModEnabled on internalModName
                var setModEnabled = typeof(ModLoader).GetMethod("SetModEnabled", BindingFlags.NonPublic | BindingFlags.Static);
                setModEnabled?.Invoke(null, [internalName, newState == State.Enabled]);
            }

            // Update the "Toggle All" option's state
            toggleAllDisabledMods.SetState(newState);
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
                ModElement modElement = new(mod.Name, mod.Name);
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
                ModSourcesElement modSourcesElement = new(modPath);
                modSourcesElements.Add(modSourcesElement);
                uiList.Add(modSourcesElement);
                AddPadding(3);
            }
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
