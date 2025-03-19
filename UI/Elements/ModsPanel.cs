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
        public List<String> enabledMods = [];

        public ModsPanel() : base(title: "Mods List", scrollbarEnabled: true)
        {
            AddHeader("Mod Sources");
            ConstructModSourcesList();
            AddPadding();

            AddHeader("Enabled Mods");
            ConstructEnabledModsList();
            AddPadding();

            AddHeader("Navigate");
            AddOnOffOption(GoToModSources, "To Mod Sources (my mods)", "Exit World And Go To Mod Sources");
            AddOnOffOption(GoToModsList, "To Mods List (enabled mods)", "Exit World And Go To Mods List");
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
