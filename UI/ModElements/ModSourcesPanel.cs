using System;
using System.Collections.Generic;
using System.Reflection;
using ModHelper.UI.AbstractElements;
using Terraria;
using Terraria.ModLoader;

namespace ModHelper.UI.ModElements
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class ModSourcesPanel : BasePanel
    {
        public List<ModSourcesElement> modSourcesElements = [];

        #region Constructor
        public ModSourcesPanel() : base(header: "Mod Sources", scrollbarEnabled: true)
        {
            // Active = true; // uncomment to show the panel by default
            AddPadding(20);
            ConstructModSources();
        }
        #endregion

        #region Constructing mod lists

        private void ConstructModSources()
        {
            // Get all the mod sources paths
            foreach (string fullModPath in GetModSourcesPaths())
            {
                // Get the clean name
                string cleanName = GetModSourcesCleanName(fullModPath);

                // Cut to max 20 chars
                if (cleanName.Length > 20)
                    cleanName = string.Concat(cleanName.AsSpan(0, 20), "...");

                ModSourcesElement modSourcesElement = new(fullModPath: fullModPath, cleanName: cleanName);
                modSourcesElements.Add(modSourcesElement);
                uiList.Add(modSourcesElement);
                AddPadding(3);
            }

            // if mwe have less than 10 mods, remove the scrollbar
            // if (modSourcesElements.Count < 10)
            // {
            // scrollbar.Remove();
            // }
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
        #endregion

        #region Navigation methods
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
        #endregion
    }
}
