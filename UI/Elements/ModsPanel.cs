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
using Terraria.Graphics.Effects;
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
        private UIElement topContainer;

        // enabled mods
        private readonly List<ModElement> enabledMods = [];

        // disabled mods
        private List<ModElement> allMods = [];

        // filter state
        private string currentFilter = "";
        private bool updateNeeded = false;

        #region Constructor
        public ModsPanel() : base(title: "Manage Mods", scrollbarEnabled: true)
        {
            AddPadding(20f);

            Active = true; // show by default for testing

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
            searchbox.Left.Set(5, 0f); // Place at the left edge

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

            // Create and configure the "Enable All" header.
            HeaderElement enableAll = new HeaderElement("Enable All", leftClick: EnableAllMods, hover: "Enable all mods", color: Color.Green, HAlign: 0.5f);
            enableAll.Width.Set(100, 0f);
            enableAll.Height.Set(20, 0f);
            enableAll.Top.Set(-5, 0);
            enableAll.HAlign = 0.92f;
            topContainer.Append(enableAll);

            // Create and configure the "Disable All" header.
            HeaderElement disableAll = new HeaderElement("Disable All", leftClick: DisableAllMods, hover: "Disable all mods", color: ColorHelper.CalamityRed, HAlign: 0.5f);
            disableAll.Width.Set(100, 0f);
            disableAll.Height.Set(20, 0f);
            disableAll.HAlign = 0.92f;
            disableAll.Top.Set(25, 0);
            topContainer.Append(disableAll);

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
        private void FilterMods()
        {
            // Clear all elements except the first one in the UIList
            uiList.Clear();

            // re-add the first element (the top container)
            AddPadding(20f);
            uiList.Add(topContainer);
            AddPadding(20f);

            // add all mods that match the current filter
            List<ModElement> filteredMods = [];

            List<ModElement> allAndEnabledMods = allMods.Concat(enabledMods).ToList(); // Combine all mods into one list
            foreach (ModElement modElement in allAndEnabledMods)
            {
                // Check if the mod name contains the current filter string
                if (modElement.cleanModName.Contains(currentFilter, StringComparison.OrdinalIgnoreCase))
                {
                    // Add the mod element to the UIList
                    filteredMods.Add(modElement);
                }
            }
            Log.Info("found " + filteredMods.Count + " mods matching filter: " + currentFilter);
            uiList.AddRange(filteredMods);
            AddPadding(3f);
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

        private void ConstructEnabledMods()
        {
            var mods = ModLoader.Mods.Skip(1);//ignore the built in Modloader mod
            foreach (Mod mod in mods)
            {
                // you could change this to send the "clean name"
                // this is where we set the text of the mod element
                ModElement modElement = new(mod.DisplayNameClean, mod.Name);
                uiList.Add(modElement);
                enabledMods.Add(modElement);
                AddPadding(3);
            }
        }

        private void ConstructAllMods()
        {
            List<object> sortedMods = GetAllWorkshopMods();

            // Get all mods the user has installed via reflection
            foreach (var mod in sortedMods) // "mod" is of type LocalMod
            {
                // Get the clean name using reflection for the LocalMod mod.
                string cleanName = GetCleanName(mod);
                string internalName = mod.ToString();

                // Skip mods that are already in the enabled mods list
                // to avoid duplicates.
                if (enabledMods.Any(modElement => modElement.internalName == internalName))
                {
                    continue;
                }

                // Log.Info("InternalName: " + internalName + " CleanName: " + cleanName);

                // We want to pass the LocalMod's TmodFile to GetModIconFromAllMods
                object tmod = getTmodFile(mod);

                Texture2D modIcon = GetModIconFromAllMods(tmod);

                ModElement modElement = new(
                    cleanModName: cleanName,
                    internalModName: internalName,
                    icon: modIcon
                    );

                modElement.SetState(State.Disabled);
                uiList.Add(modElement);
                allMods.Add(modElement);
                AddPadding(3);
            }
        }

        #endregion

        #region Helpers for getting mod lists
        // Note: Lots of reflection is used here, so be careful with error handling.

        // Helper method to get all workshop mods
        private static List<object> GetAllWorkshopMods()
        {
            // Get all mods the user has installed via reflection
            try
            {
                Assembly assembly = typeof(ModLoader).Assembly;
                Type modOrganizerType = assembly.GetType("Terraria.ModLoader.Core.ModOrganizer");
                MethodInfo findWorkshopModsMethod = modOrganizerType.GetMethod("FindWorkshopMods", BindingFlags.NonPublic | BindingFlags.Static);

                var workshopMods = (IReadOnlyList<object>)findWorkshopModsMethod.Invoke(null, null);

                // Sort workshop mods by clean name
                var sortedMods = workshopMods.ToList();
                sortedMods.Sort((a, b) =>
                {
                    // Get clean names for comparison
                    string nameA = GetCleanName(a);
                    string nameB = GetCleanName(b);
                    return string.Compare(nameA, nameB, StringComparison.OrdinalIgnoreCase);
                });

                Log.Info("Found " + sortedMods.Count + " workshop mods.");
                return sortedMods;
            }
            catch
            {
                Log.Warn("An error occurred while retrieving workshop mods.");
            }
            return [];
        }

        // Helper method to get clean name from a mod object
        private static string GetCleanName(object mod)
        {
            FieldInfo displayNameField = mod.GetType().GetField("DisplayNameClean", BindingFlags.Public | BindingFlags.Instance);
            return (string)displayNameField.GetValue(mod);
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

        private object getLocalModName(object mod)
        {
            Type localModType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.LocalMod");

            FieldInfo Name = localModType.GetField("Name", BindingFlags.Public | BindingFlags.Instance);
            object name = Name.GetValue(mod);
            return name;
        }

        private object getLastModified(object mod)
        {
            Type localModType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.LocalMod");

            FieldInfo Name = localModType.GetField("lastModified", BindingFlags.Public | BindingFlags.Instance);
            object lastModified = Name.GetValue(mod);
            return lastModified;
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
                string internalName = modElement.internalName; // Assuming InternalName is a property of ModElement

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
                string internalName = modElement.internalName; // Assuming InternalName is a property of ModElement

                // Use reflection to call SetModEnabled on internalModName
                var setModEnabled = typeof(ModLoader).GetMethod("SetModEnabled", BindingFlags.NonPublic | BindingFlags.Static);
                setModEnabled?.Invoke(null, [internalName, false]);
            }
        }

        #endregion
    }
}
