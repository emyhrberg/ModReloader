using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.UI.Elements
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class ModsPanel : OptionPanel
    {
        public List<ModSourcesElement> modSourcesElements = [];
        public List<string> enabledMods = [];

        public ModsPanel() : base(title: "Mods", scrollbarEnabled: true)
        {
            // Active = true; // uncomment to show the panel by default
            AddPadding(5);
            AddHeader("Mod Sources", GoToModSources, "Click to exit world and go to mod sources");
            ConstructModSourcesList();
            AddPadding();

            AddHeader("Mods List", onLeftClick: GoToModsList, "Click to exit world and go to mods list");
            ConstructEnabledModsList();
            AddPadding();
        }

        private void ConstructEnabledModsList()
        {
            var mods = ModLoader.Mods.Skip(1);//ignore the built in Modloader mod
            foreach (Mod mod in mods)
            {
                ModElement modElement = new(mod.DisplayNameClean, mod.Name);
                uiList.Add(modElement);
                enabledMods.Add(mod.Name);
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
