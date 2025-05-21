using System;
using System.Collections.Generic;
using System.IO;
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

        public ModSourcesPanelConfig(ModSourcesConfig parent, bool needScrollbar = false) : base(needScrollbar)
        {
            Width.Set(-5, 1);
            Left.Set(-0, 0);
            Height.Set(-10, 1);
            MaxHeight.Set(-10, 1);
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
                string cleanName = Path.GetFileName(fullModPath); // Use the folder name as the clean name
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

        private List<string> GetModSourcesPaths()
        {
            // Directly use the ModCompile class to get mod sources paths
            return [.. Terraria.ModLoader.Core.ModCompile.FindModSources()];
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Width.Set(-5, 1);
            // Left.Set(-0, 0);
            // Height.Set(-10, 1);
            // MaxHeight.Set(-10, 1);
            base.Draw(spriteBatch);
        }
    }
}
