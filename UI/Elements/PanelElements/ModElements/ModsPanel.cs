using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Core;
using Terraria.UI;
using static ModReloader.UI.Elements.PanelElements.OptionElement;

namespace ModReloader.UI.Elements.PanelElements.ModElements
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class ModsPanel : BasePanel
    {
        // top container for enable all, disable all, and searchbox
        public Searchbox searchbox;
        public readonly UIElement topContainer;

        // filters
        public readonly ModFilterChangeView modChangeView; // filter mods view image button
        public readonly ModFilterEnabled modFilterEnabled; // filter enabled mods button
        public readonly ModFilterSideButton modFilterSide; // filter mod side button

        // enabled mods
        public readonly List<ModElement> enabledMods = [];

        // disabled mods
        public readonly List<ModElement> allMods = [];

        // filter state
        private string currentFilter = "";
        private bool updateNeeded = false;

        #region Constructor
        public ModsPanel() : base(header: "Manage Mods", scrollbarEnabled: true)
        {
            AddPadding(10f);

            // Active = true; // show by default for testing

            // we add this to UIlist, so it will be positioned by uiList
            topContainer = new() // top pos for enable all, disable all, and searchbox
            {
                Width = new StyleDimension(0f, 1f),
                Height = new StyleDimension(40, 0f),
            };
            // Add searchbox.
            searchbox = new Searchbox("Type to search");
            searchbox.Width.Set(200, 0f);
            searchbox.Height.Set(35, 0f);
            searchbox.Left.Set(10, 0f); // Place at the left edge
            searchbox.Top.Set(20, 0);

            // On text change event
            searchbox.OnTextChanged += () =>
            {
                currentFilter = searchbox.currentString;
                updateNeeded = true;
            };

            topContainer.Append(searchbox);

            // Add clear button
            UIImageButton clearSearchButton = new(Main.Assets.Request<Texture2D>("Images/UI/SearchCancel"))
            {
                HAlign = 1f,
                VAlign = 0.5f,
                Left = new StyleDimension(-2f, 0f)
            };
            searchbox.Append(clearSearchButton);
            clearSearchButton.OnLeftClick += (evt, element) =>
            {
                searchbox.SetText(string.Empty);
                currentFilter = string.Empty;
                updateNeeded = true;
            };

            // Add filter mods view image button
            modChangeView = new(Ass.FilterViewSize);
            modChangeView.Left.Set(10, 0);
            modChangeView.Top.Set(-6, 0);
            modChangeView.ForceLarge();
            topContainer.Append(modChangeView);

            // Add filter enabled mods button
            modFilterEnabled = new(Ass.FilterEnabled);
            modFilterEnabled.Left.Set(10 + 23 + 5, 0);
            modFilterEnabled.Top.Set(-6, 0);
            topContainer.Append(modFilterEnabled);

            // Add filter mod side button
            modFilterSide = new(Ass.FilterModSide);
            modFilterSide.Left.Set(10 + 23 * 2 + 5 * 2, 0);
            modFilterSide.Top.Set(-6, 0);
            topContainer.Append(modFilterSide);

            // Create and configure the "Enable All" panel.
            ModToggleAllPanel enableAllPanel = new(
                color: Color.Green,
                text: "Enable All",
                hover: "Enable all mods",
                onClick: EnableAllMods
            );
            enableAllPanel.Top.Set(-5, 0);
            topContainer.Append(enableAllPanel);

            // Create and configure the "Disable All" panel.
            ModToggleAllPanel disableAllPanel = new(
                color: ColorHelper.CalamityRed,
                text: "Disable All",
                hover: "Disable all mods",
                onClick: DisableAllMods
            );
            disableAllPanel.Top.Set(25, 0);
            topContainer.Append(disableAllPanel);

            // Now, add the horizontal container to the UIList.
            uiList.Add(topContainer);
            AddPadding(20f);

            // initial constructing of mod lists
            ConstructEnabledMods(large: true);
            ConstructAllMods(large: true);
            AddPadding(3f);
            FilterMods(); // needed to show the ModsFoundPanel at the start?
        }
        #endregion // end of constructor

        #region filter

        /// <summary>
        /// Clears the uiList, removes all elements, and adds the mods that match the current filter.
        /// </summary>
        public void FilterMods()
        {
            // Clear all elements from the UIList.
            uiList.Clear();

            // Re-add the top container and padding.
            AddPadding(10f);
            uiList.Add(topContainer);
            AddPadding(20f);

            // Prepare the list that will hold filtered mod elements.
            List<ModElement> filteredMods = [];

            // Combine enabled and disabled mods into one list.
            List<ModElement> allModsCombined = enabledMods.Concat(allMods).ToList();

            foreach (ModElement modElement in allModsCombined)
            {
                // 1. Check if the mod name contains the current filter string (ignoring case).
                //    (When currentFilter is empty, this always returns true.)
                bool matchCleanModName = modElement.cleanModName.Contains(currentFilter, StringComparison.OrdinalIgnoreCase);

                // 1b. Check internal mod name
                bool matchInternalModName = modElement.internalModName.Contains(currentFilter, StringComparison.OrdinalIgnoreCase);

                // 2. Check if the mod should be included based on the Enabled/Disabled filter.
                bool matchEnabledDisabled = true;
                if (modFilterEnabled.currentEnabledDisabledView == ModFilterEnabled.ModFilterEnabledDisabled.Enabled)
                {
                    matchEnabledDisabled = modElement.GetState() == EnabledState.Enabled;
                }
                else if (modFilterEnabled.currentEnabledDisabledView == ModFilterEnabled.ModFilterEnabledDisabled.Disabled)
                {
                    matchEnabledDisabled = modElement.GetState() == EnabledState.Disabled;
                }

                // 3. Check if the mod matches the current mod side filter.
                //    Compare the mod's side string to the filter's value,
                //    or always pass if the filter is set to "All".
                bool matchSide = modFilterSide.currentModSideFilter == ModFilterSideButton.ModFilterSide.All ||
                    string.Equals(modElement.side, modFilterSide.currentModSideFilter.ToString(), StringComparison.OrdinalIgnoreCase);

                if ((matchCleanModName || matchInternalModName) && matchEnabledDisabled && matchSide)
                {
                    filteredMods.Add(modElement);
                }
            }

            // Always add a ModsFoundPanel displaying the number of mods filtered.
            string numberOfModsFound = $"{filteredMods.Count} mods filtered";
            ModsFoundPanel modsFoundPanel = new ModsFoundPanel(numberOfModsFound);
            uiList.Add(modsFoundPanel);
            AddPadding(3f);

            // If no search text is provided, order the mods so that enabled ones come first.
            if (string.IsNullOrEmpty(currentFilter))
            {
                filteredMods = filteredMods
                    .OrderBy(mod => mod.GetState() == EnabledState.Enabled ? 0 : 1)
                    .ToList();
            }

            // Add all the filtered mod elements to the UIList.
            uiList.AddRange(filteredMods);
            AddPadding(3f); // a little extra padding at the end
        }

        public override void Update(GameTime gameTime)
        {
            if (!Active)
            {
                return;
            }

            base.Update(gameTime);

            if (updateNeeded)
            {
                FilterMods();
                updateNeeded = false;
            }
        }

        #endregion

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
            {
                return;
            }

            // use for hot reload UI changes
            base.Draw(spriteBatch);
        }

        #region Constructing mod lists

        public void ConstructEnabledMods(bool large = false)
        {
            // a list of all mods that are enabled
            var currEnabledMods = ModLoader.Mods.Skip(1); // (skip 1 to ignore tml mod)

            IReadOnlyList<LocalMod> sortedMods = ModOrganizer.FindAllMods();

            sortedMods = sortedMods.OrderBy(mod => mod.DisplayNameClean).ToList();

            // Get all mods the user has installed via reflection
            foreach (LocalMod localMod in sortedMods) // "localMod" is of type LocalMod
            {
                string internalName = localMod.ToString();

                // Only add mod if it's in the enabled mods list
                if (!currEnabledMods.Any(m => m.Name == internalName))
                {
                    // Log.Info("Skipping mod " + internalName + " as it is not enabled.");
                    continue;
                }

                // Check if the mod is already in the enabledMods list to avoid duplicates
                if (enabledMods.Any(modElement => modElement.internalModName == internalName))
                {
                    // Log.Info("Skipping mod " + internalName + " as it is already in the enabled mods list.");
                    continue;
                }

                // Get the mod of Mod instance
                Mod currMod = ModLoader.GetMod(internalName);

                // Log.Info("Adding mod " + internalName + " version " + currMod.Version + " to enabled mods.");

                // Get the clean name using reflection for the LocalMod mod.
                string cleanName = localMod.DisplayNameClean;
                string description = localMod.properties.description ?? string.Empty;

                // Create and add the mod element
                ModElement modElement = new(
                    cleanModName: currMod.DisplayNameClean,
                    internalModName: currMod.Name,
                    modDescription: description,
                    version: currMod.Version.ToString(),
                    side: currMod.Side.ToString(),
                    large: large
                );

                // Apply filtering: 1) mod side and 2) enabled

                uiList.Add(modElement);
                enabledMods.Add(modElement);
                // AddPadding(3);
            }
        }

        public void ConstructAllMods(bool large = false)
        {
            IReadOnlyList<LocalMod> sortedMods = ModOrganizer.FindWorkshopMods();

            // sort them by clean name
            sortedMods = sortedMods.OrderBy(mod => mod.DisplayNameClean).ToList();

            // Get all mods the user has installed via reflection
            foreach (LocalMod mod in sortedMods) // "mod" is of type LocalMod
            {
                // Get the clean name using reflection for the LocalMod mod.
                string cleanName = mod.DisplayNameClean;
                string internalName = mod.ToString();
                string description = mod.properties.description;
                string short_desc = description.Length > 50 ? description.Substring(0, 50) + "..." : description;
                // Log.Info("Mod name: " + cleanName + " InternalName: " + internalName + " Description: " + short_desc);

                // Skip mods that are already in the enabled mods list
                // to avoid duplicates.
                if (enabledMods.Any(modElement => modElement.internalModName == internalName))
                {
                    // Log.Info("Skipping mod " + cleanName + " as it is already enabled.");
                    continue;
                }

                // Log.Info("InternalName: " + internalName + " CleanName: " + cleanName);

                // We want to pass the LocalMod's TmodFile to GetModIconFromAllMods
                TmodFile tmod = mod.modFile;
                Texture2D modIcon = GetModIconFromAllMods(tmod);

                // try get mod version. if fails, just write empty string.
                string version = mod.properties.version.ToString() ?? string.Empty;
                // Log.Info("Mod version: " + version);
                string side = mod.properties.side.ToString() ?? string.Empty;

                ModElement modElement = new(
                    cleanModName: cleanName,
                    internalModName: internalName,
                    icon: modIcon,
                    modDescription: description,
                    version: version,
                    side: side,
                    large: large
                    );

                modElement.SetState(EnabledState.Disabled);
                uiList.Add(modElement);
                allMods.Add(modElement);
                // AddPadding(3);
            }
        }

        #endregion
        #region Helpers mod lists

        private static Texture2D GetModIconFromAllMods(TmodFile tmodFile)
        {
            try
            {
                // Check if the file contains "icon.png"
                if (!tmodFile.HasFile("icon.png"))
                {
                    Log.Warn("The TmodFile does not have an icon.");
                    return null;
                }

                // Open the file and retrieve the stream for "icon.png"
                using (tmodFile.Open())
                using (Stream stream = tmodFile.GetStream("icon.png", true))
                {
                    Asset<Texture2D> iconTexture = Main.Assets.CreateUntracked<Texture2D>(stream, ".png", AssetRequestMode.ImmediateLoad);

                    // Log.Info("Successfully loaded icon from TmodFile.");
                    return iconTexture.Value;
                }
            }
            catch (Exception ex)
            {
                Log.Info("Error while retrieving icon from TmodFile: " + ex);
            }
            return null;
        }

        #endregion

        #region Toggle all methods

        private void EnableAllMods()
        {
            // Combine allMods and enabledMods into one list
            var combinedMods = enabledMods.Concat(allMods);

            foreach (ModElement modElement in combinedMods)
            {
                modElement.SetState(EnabledState.Enabled);
                string internalName = modElement.internalModName; // Assuming InternalName is a property of ModElement

                ModLoader.SetModEnabled(internalName, true);
            }
        }

        private void DisableAllMods()
        {
            // Combine allMods and enabledMods into one list
            var combinedMods = enabledMods.Concat(allMods);

            foreach (ModElement modElement in combinedMods)
            {
                modElement.SetState(EnabledState.Disabled);
                string internalName = modElement.internalModName; // Assuming InternalName is a property of ModElement
                ModLoader.SetModEnabled(internalName, false);
            }
        }

        #endregion
    }
}
