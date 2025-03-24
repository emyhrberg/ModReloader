using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private List<ModElement> disabledMods = [];
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

            AddHeader("Disabled Mods");
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

                    // if it doesnt exist in enabled mods, add it
                    if (modElements.Any(modElement => modElement.modName == modName))
                        continue;

                    ModElement modElement = new(modName: modName, internalModName: modName, hasIcon: false);
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

            // Add them to the UIList
            // foreach (string modPath in GetModSourcesPaths())
            // {
            //     ModElement modElement = new(modPath);
            //     modElement.SetState(State.Disabled);
            //     modElements.Add(modElement);
            //     uiList.Add(modElement);
            //     AddPadding(3);
            // }
            AddPadding(3);
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
