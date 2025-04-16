using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using static ModHelper.UI.Elements.OptionElement;

namespace ModHelper.UI.Elements
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class ModsPanel : OptionPanel
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
        public ModsPanel() : base(title: "Manage Mods", scrollbarEnabled: true)
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
            EnableDisableAllPanel enableAllPanel = new(
                color: Color.Green,
                text: "Enable All",
                hover: "Enable all mods",
                onClick: EnableAllMods
            );
            enableAllPanel.Top.Set(-5, 0);
            topContainer.Append(enableAllPanel);

            // Create and configure the "Disable All" panel.
            EnableDisableAllPanel disableAllPanel = new(
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
            ConstructEnabledMods();
            ConstructAllMods();
            AddPadding(3f);
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
            List<ModElement> filteredMods = new List<ModElement>();

            // Combine enabled and disabled mods into one list.
            List<ModElement> allModsCombined = enabledMods.Concat(allMods).ToList();

            foreach (ModElement modElement in allModsCombined)
            {
                // 1. Check if the mod name contains the current filter string (ignoring case).
                //    (When currentFilter is empty, this always returns true.)
                bool matchSearch = modElement.cleanModName.Contains(currentFilter, StringComparison.OrdinalIgnoreCase);

                // 2. Check if the mod should be included based on the Enabled/Disabled filter.
                bool matchEnabledDisabled = true;
                if (modFilterEnabled.currentEnabledDisabledView == ModFilterEnabled.ModFilterEnabledDisabled.Enabled)
                {
                    matchEnabledDisabled = modElement.GetState() == State.Enabled;
                }
                else if (modFilterEnabled.currentEnabledDisabledView == ModFilterEnabled.ModFilterEnabledDisabled.Disabled)
                {
                    matchEnabledDisabled = modElement.GetState() == State.Disabled;
                }

                // 3. Check if the mod matches the current mod side filter.
                //    Compare the mod's side string to the filter's value,
                //    or always pass if the filter is set to "All".
                bool matchSide = modFilterSide.currentModSideFilter == ModFilterSideButton.ModFilterSide.All ||
                    string.Equals(modElement.side, modFilterSide.currentModSideFilter.ToString(), StringComparison.OrdinalIgnoreCase);

                if (matchSearch && matchEnabledDisabled && matchSide)
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
                    .OrderBy(mod => mod.GetState() == State.Enabled ? 0 : 1)
                    .ToList();
            }

            // Add all the filtered mod elements to the UIList.
            uiList.AddRange(filteredMods);
            AddPadding(3f); // a little extra padding at the end
        }

        public override void Update(GameTime gameTime)
        {
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
            // use for hot reload UI changes
            base.Draw(spriteBatch);
        }

        #region Constructing mod lists

        public void ConstructEnabledMods()
        {
            // a list of all mods that are enabled
            var currEnabledMods = ModLoader.Mods.Skip(1); // (skip 1 to ignore tml mod)

            List<object> sortedMods = FindAllMods();

            // Get all mods the user has installed via reflection
            foreach (var localMod in sortedMods) // "localMod" is of type LocalMod
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
                string cleanName = GetCleanName(localMod);
                string description = GetLocalModDescription(localMod);

                // Create and add the mod element
                ModElement modElement = new(
                    cleanModName: currMod.DisplayNameClean,
                    internalModName: currMod.Name,
                    modDescription: description,
                    version: currMod.Version.ToString(),
                    side: currMod.Side.ToString()
                );

                // Apply filtering: 1) mod side and 2) enabled

                uiList.Add(modElement);
                enabledMods.Add(modElement);
                // AddPadding(3);
            }
        }

        public void ConstructAllMods()
        {
            List<object> sortedMods = FindAllMods();

            // Get all mods the user has installed via reflection
            foreach (var mod in sortedMods) // "mod" is of type LocalMod
            {
                // Get the clean name using reflection for the LocalMod mod.
                string cleanName = GetCleanName(mod);
                string internalName = mod.ToString();
                string description = GetLocalModDescription(mod);
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
                object tmod = getTmodFile(mod);

                Texture2D modIcon = GetModIconFromAllMods(tmod);

                // try get mod version. if fails, just write empty string.
                string version = GetLocalModVersion(mod);
                // Log.Info("Mod version: " + version);
                string side = GetLocalModSide(mod);

                ModElement modElement = new(
                    cleanModName: cleanName,
                    internalModName: internalName,
                    icon: modIcon,
                    modDescription: description,
                    version: version,
                    side: side
                    );

                modElement.SetState(State.Disabled);
                uiList.Add(modElement);
                allMods.Add(modElement);
                // AddPadding(3);
            }
        }

        #endregion

        #region Helpers mod lists
        // Note: Lots of reflection is used here, so be careful with error handling.

        // Helper method to get all workshop mods
        private static List<object> FindAllMods()
        {
            // Get all mods the user has installed via reflection
            try
            {
                Type modOrganizerType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.ModOrganizer");
                MethodInfo findWorkshopModsMethod = modOrganizerType.GetMethod("FindAllMods", BindingFlags.NonPublic | BindingFlags.Static);

                var workshopModsArray = findWorkshopModsMethod.Invoke(null, null) as Array;
                if (workshopModsArray == null)
                {
                    Log.Warn("FindAllMods returned null.");
                    return [];
                }

                List<object> sortedMods = [.. workshopModsArray];

                // Remove duplicates by clean name
                var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                sortedMods.RemoveAll(mod => !unique.Add(GetCleanName(mod)));

                // Sort mods by clean name
                sortedMods.Sort((a, b) =>
                {
                    string nameA = GetCleanName(a);
                    string nameB = GetCleanName(b);
                    return string.Compare(nameA, nameB, StringComparison.OrdinalIgnoreCase);
                });

                Log.Info("Found " + sortedMods.Count + " mods.");
                return sortedMods;
            }
            catch (Exception ex)
            {
                Log.Warn($"An error occurred while retrieving workshop mods: {ex.Message}");
            }
            return [];
        }

        // Helper method to get clean name from a mod object
        private static string GetCleanName(object mod)
        {
            FieldInfo displayNameField = mod.GetType().GetField("DisplayNameClean", BindingFlags.Public | BindingFlags.Instance);
            return (string)displayNameField.GetValue(mod);
        }

        private static Texture2D GetModIconFromAllMods(object TmodFile)
        {
            try
            {
                // Check if the file exists
                MethodInfo hasFileMethod = TmodFile.GetType().GetMethod("HasFile", BindingFlags.Public | BindingFlags.Instance);
                bool hasIcon = (bool)hasFileMethod.Invoke(TmodFile, ["icon.png"]);
                if (!hasIcon)
                {
                    Log.Warn("The TmodFile does not have an icon.");
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

                // note: Probably MUST be immediateLoad, otherwise it doesnt show up.

                // Log.Info("Successfully loaded icon from TmodFile.");
                return iconTexture.Value;
            }
            catch (Exception ex)
            {
                Log.Info("Error while retrieving icon from TmodFile via reflection: " + ex);
            }
            return null;
        }

        private object getTmodFile(object mod)
        {
            Assembly assembly = typeof(ModLoader).Assembly;
            Type localModType = assembly.GetType("Terraria.ModLoader.Core.LocalMod");

            FieldInfo modFileField = localModType.GetField("modFile", BindingFlags.Public | BindingFlags.Instance);
            object tmod = modFileField.GetValue(mod);
            return tmod;
        }

        private static object getLocalModName(object mod)
        {
            Type localModType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.LocalMod");

            FieldInfo Name = localModType.GetField("Name", BindingFlags.Public | BindingFlags.Instance);
            if (mod == null || Name == null)
            {
                Log.Warn("getLocalModName: mod or Name is null.");
                return string.Empty;
            }
            object name = Name.GetValue(mod);
            return name;
        }

        private static object getLastModified(object mod)
        {
            Type localModType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.LocalMod");

            FieldInfo Name = localModType.GetField("lastModified", BindingFlags.Public | BindingFlags.Instance);
            object lastModified = Name.GetValue(mod);
            return lastModified;
        }

        private static string GetLocalModDescription(object localMod)
        {
            try
            {
                if (localMod == null)
                {
                    Log.Warn("GetLocalModDescription: localMod is null.");
                    return string.Empty;
                }

                Type localModType = localMod.GetType();
                object buildPropertiesInstance = null;

                // First try to get 'properties' as a property.
                var propertiesProp = localModType.GetProperty("properties", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (propertiesProp != null)
                {
                    buildPropertiesInstance = propertiesProp.GetValue(localMod);
                }
                else
                {
                    // Fallback: try getting 'properties' as a field.
                    FieldInfo propertiesField = localModType.GetField("properties", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (propertiesField != null)
                    {
                        buildPropertiesInstance = propertiesField.GetValue(localMod);
                    }
                    else
                    {
                        Log.Warn("GetLocalModDescription: Could not find the 'properties' member in LocalMod.");
                        return string.Empty;
                    }
                }

                if (buildPropertiesInstance == null)
                {
                    Log.Warn("GetLocalModDescription: LocalMod.properties is null.");
                    return string.Empty;
                }

                // Now get the description from BuildProperties.
                Type buildPropertiesType = buildPropertiesInstance.GetType();
                string description = null;

                // Try as a property.
                var descriptionProp = buildPropertiesType.GetProperty("description", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (descriptionProp != null)
                {
                    description = descriptionProp.GetValue(buildPropertiesInstance) as string;
                }
                else
                {
                    // Fallback: try as a field.
                    FieldInfo descriptionField = buildPropertiesType.GetField("description", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (descriptionField != null)
                    {
                        description = descriptionField.GetValue(buildPropertiesInstance) as string;
                    }
                    else
                    {
                        Log.Warn("GetLocalModDescription: Could not find the 'description' member in BuildProperties.");
                        return string.Empty;
                    }
                }

                if (string.IsNullOrEmpty(description))
                {
                    Log.Info("GetLocalModDescription: Description is empty for LocalMod: " + localMod.ToString());
                }
                return description ?? string.Empty;
            }
            catch (Exception ex)
            {
                Log.Warn($"GetLocalModDescription: Exception occurred - {ex}");
                return string.Empty;
            }
        }

        private static string GetLocalModVersion(object localMod)
        {
            if (localMod == null)
            {
                Log.Warn("GetLocalModVersion: localMod is null.");
                return string.Empty;
            }

            try
            {
                // Attempt to retrieve the 'properties' member from LocalMod.
                var localModType = localMod.GetType();
                object propertiesInstance = null;

                // First, try to get it as a field.
                FieldInfo propertiesField = localModType.GetField("properties", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (propertiesField != null)
                {
                    propertiesInstance = propertiesField.GetValue(localMod);
                }
                else
                {
                    // Fallback: try as a property.
                    PropertyInfo propertiesProp = localModType.GetProperty("properties", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (propertiesProp != null)
                    {
                        propertiesInstance = propertiesProp.GetValue(localMod);
                    }
                }

                if (propertiesInstance == null)
                {
                    Log.Warn("GetLocalModVersion: Could not retrieve the 'properties' member from the LocalMod.");
                    return string.Empty;
                }

                // Now try to obtain the 'version' member from the BuildProperties instance.
                var propertiesType = propertiesInstance.GetType();
                object versionObj = null;

                // First, attempt as a property.
                PropertyInfo versionProp = propertiesType.GetProperty("version", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (versionProp != null)
                {
                    versionObj = versionProp.GetValue(propertiesInstance);
                }
                else
                {
                    // Fallback to a field.
                    FieldInfo versionField = propertiesType.GetField("version", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (versionField != null)
                    {
                        versionObj = versionField.GetValue(propertiesInstance);
                    }
                    else
                    {
                        Log.Warn("GetLocalModVersion: Could not find a 'version' property or field in BuildProperties.");
                        return string.Empty;
                    }
                }

                if (versionObj == null)
                {
                    Log.Warn("GetLocalModVersion: The 'version' value retrieved is null.");
                    return string.Empty;
                }

                return versionObj.ToString();
            }
            catch (Exception ex)
            {
                Log.Warn($"GetLocalModVersion: Exception occurred - {ex}");
                return string.Empty;
            }
        }

        private static string GetLocalModSide(object localMod)
        {
            if (localMod == null)
            {
                Log.Warn("GetLocalModSide: localMod is null.");
                return string.Empty;
            }

            try
            {
                // Attempt to retrieve the 'properties' member from LocalMod.
                var localModType = localMod.GetType();
                object propertiesInstance = null;

                // First, try to get it as a field.
                FieldInfo propertiesField = localModType.GetField("properties", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (propertiesField != null)
                {
                    propertiesInstance = propertiesField.GetValue(localMod);
                }
                else
                {
                    // Fallback: try as a property.
                    PropertyInfo propertiesProp = localModType.GetProperty("properties", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (propertiesProp != null)
                    {
                        propertiesInstance = propertiesProp.GetValue(localMod);
                    }
                }

                if (propertiesInstance == null)
                {
                    Log.Warn("GetLocalModSide: Could not retrieve the 'properties' member from the LocalMod.");
                    return string.Empty;
                }

                // Now try to obtain the internal 'ModSide' member from the BuildProperties instance.
                var propertiesType = propertiesInstance.GetType();
                object sideObject = null;

                // Try as a property first
                PropertyInfo sideProp = propertiesType.GetProperty("side", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (sideProp != null)
                {
                    sideObject = sideProp.GetValue(propertiesInstance);
                }
                else
                {
                    // Fallback to a field
                    FieldInfo sideField = propertiesType.GetField("side", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (sideField != null)
                    {
                        sideObject = sideField.GetValue(propertiesInstance);
                    }
                }

                if (sideObject == null)
                {
                    Log.Warn("GetLocalModSide: The 'side' value retrieved is null.");
                    return "Both"; // Default value if we can't determine
                }

                return sideObject.ToString();
            }
            catch (Exception ex)
            {
                Log.Warn($"GetLocalModSide: Exception occurred - {ex}");
                return string.Empty;
            }
        }

        #endregion

        #region Toggle all methods

        private void EnableAllMods()
        {
            // Combine allMods and enabledMods into one list
            var combinedMods = enabledMods.Concat(allMods);

            foreach (ModElement modElement in combinedMods)
            {
                modElement.SetState(State.Enabled);
                string internalName = modElement.internalModName; // Assuming InternalName is a property of ModElement

                // Use reflection to call SetModEnabled on internalModName
                var setModEnabled = typeof(ModLoader).GetMethod("SetModEnabled", BindingFlags.NonPublic | BindingFlags.Static);
                setModEnabled?.Invoke(null, [internalName, true]);
            }
        }

        private void DisableAllMods()
        {
            // Combine allMods and enabledMods into one list
            var combinedMods = enabledMods.Concat(allMods);

            foreach (ModElement modElement in combinedMods)
            {
                modElement.SetState(State.Disabled);
                string internalName = modElement.internalModName; // Assuming InternalName is a property of ModElement

                // Use reflection to call SetModEnabled on internalModName
                var setModEnabled = typeof(ModLoader).GetMethod("SetModEnabled", BindingFlags.NonPublic | BindingFlags.Static);
                setModEnabled?.Invoke(null, [internalName, false]);
            }
        }

        #endregion
    }
}
