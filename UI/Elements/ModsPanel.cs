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
        public Searchbox searchbox;

        // enabled mods
        private readonly List<ModElement> enabledMods = [];

        // disabled mods
        private List<ModElement> allMods = [];

        #region Constructor
        public ModsPanel() : base(title: "Manage Mods", scrollbarEnabled: true)
        {
            AddPadding(3f);
            AddHeader("Enable All", enableAllMods, color: Color.Green);
            AddHeader("Disable All", disableAllMods, color: ColorHelper.CalamityRed);
            searchbox = AddSearchbox();


            UIImageButton clearSearchButton = new UIImageButton(Main.Assets.Request<Texture2D>("Images/UI/SearchCancel"))
            {
                HAlign = 1f,
                VAlign = 0.5f,
                Left = new StyleDimension(-2f, 0f)
            };
            clearSearchButton.OnLeftClick += (evt, el) =>
            {
                searchbox.currentString = "";
                FilterMods("");
            };
            searchbox.Append(clearSearchButton);

            searchbox.OnTextChanged += () =>
            {
                FilterMods(searchbox.currentString);
            };

            AddPadding(20f);

            ConstructEnabledMods();
            ConstructAllMods();
            AddPadding(3f);
        }
        #endregion

        #region filter
        private string currentSearchText = "";
        private bool filterScheduled = false;

        private void FilterMods(string searchText)
        {
            // Just store the search text and set a flag - don't modify collections during event handling
            currentSearchText = searchText;
            filterScheduled = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Apply filtering in Update, not during event handling
            if (filterScheduled)
            {
                ApplyFilter();
                filterScheduled = false;
            }
        }

        private void ApplyFilter()
        {
            // Create a new list for filtered mods
            List<UIElement> filteredItems = new List<UIElement>();

            // Add the buttons first (they should always be visible)
            // Use uiList._items to get access to the internal items list
            int headerCount = 0;
            foreach (var item in uiList._items)
            {
                if (item is HeaderElement || item is Searchbox || item is UIPanel)
                {
                    filteredItems.Add(item);
                    headerCount++;
                    if (headerCount >= 3) break; // First 3 elements are headers and searchbox
                }
            }

            // Add padding
            filteredItems.Add(new HeaderElement("") { Height = { Pixels = 3f } });

            // Filter mods
            var combinedMods = enabledMods.Concat(allMods);
            foreach (var modElement in combinedMods)
            {
                if (string.IsNullOrEmpty(currentSearchText) ||
                    modElement.cleanModName.IndexOf(currentSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    filteredItems.Add(modElement);
                    // Add padding after each mod
                    filteredItems.Add(new HeaderElement("") { Height = { Pixels = 3f } });
                }
            }

            // Replace the entire list at once to prevent collection modification during enumeration
            uiList._items.Clear();
            foreach (var item in filteredItems)
            {
                uiList._items.Add(item);
            }

            // Recalculate the list
            uiList.Recalculate();
        }
        #endregion


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

        private void enableAllMods()
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

        private void disableAllMods()
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

        #region Navigation methods
        private void GoToModsList()
        {
            WorldGen.JustQuit();
            Main.menuMode = 10000;
        }
        #endregion
    }
}
