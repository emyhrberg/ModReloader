using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace ModReloader.UI.Elements.ConfigElements
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class ModSourcesPanelConfig : BasePanelConfig
    {
        public List<ModSourcesElementConfig> modSourcesElements = [];

        public ModSourcesConfig parentConfig;

        public ModSourcesPanelConfig(ModSourcesConfig parent) : base(scrollbarEnabled: true)
        {
            Width.Set(0, 1);
            Left.Set(-10, 0);
            parentConfig = parent;
            ConstructModSources();
        }

        private void ConstructModSources()
        {
            // Get all the mod sources paths and their last modified times
            List<(string fullModPath, DateTime lastModified)> modSourcesWithTimes = new();

            foreach (string fullModPath in GetModSourcesPaths())
            {
                DateTime lastModified = File.GetLastWriteTime(fullModPath);
                modSourcesWithTimes.Add((fullModPath, lastModified));
            }

            // Sort by last modified time in descending order (latest first)
            modSourcesWithTimes.Sort((a, b) => b.lastModified.CompareTo(a.lastModified));

            // Add to the UI list in sorted order

            for (int i = 0; i < modSourcesWithTimes.Count; i++)
            {
                var (fullModPath, _) = modSourcesWithTimes[i];

                // Add the element to the UI list  
                string cleanName = GetModSourcesCleanName(fullModPath);
                ModSourcesElementConfig modSourcesElement = new(parentConfig, fullModPath: fullModPath, cleanName: cleanName);
                modSourcesElements.Add(modSourcesElement);
                uiList.Add(modSourcesElement);

                // Add padding for all except the last element  
                if (i != modSourcesWithTimes.Count - 1)
                {
                    AddPadding(1);
                }
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            Width.Set(-5,1);
            Left.Set(-0, 0);
            Height.Set(-10, 1);
            MaxHeight.Set(-10, 1);
            base.Draw(spriteBatch);
        }
    }
}
