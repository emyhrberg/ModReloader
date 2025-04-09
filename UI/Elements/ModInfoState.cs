using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class ModInfoState : UIState, ILoadable
    {
        // public names
        public static ModInfoState instance;
        public string CurrentModDescription = "";
        public string modDisplayName;
        public UITextPanel<string> titlePanel;

        // Add these fields to your ModInfoState class:
        private string modInternalName;
        private object publishedFileId; // Will store the ModPubId_t

        // elements
        private UIPanel descriptionContainer;
        private UIElement messageBox; // This will hold the reflection-created UIMessageBox
        private UIScrollbar scrollbar;

        // Store reflection info to avoid looking it up repeatedly
        private static Type messageBoxType;
        private static MethodInfo setTextMethod;
        private static MethodInfo setScrollbarMethod;

        public void Load(Mod mod)
        {
            instance = this;

            // Get reflection info once during loading
            Assembly assembly = typeof(UICommon).Assembly;
            messageBoxType = assembly.GetType("Terraria.ModLoader.UI.UIMessageBox");
            setTextMethod = messageBoxType?.GetMethod("SetText");
            setScrollbarMethod = messageBoxType?.GetMethod("SetScrollbar");
        }

        public void Unload()
        {
            instance = null;
        }

        public override void OnInitialize()
        {
            // Main container
            UIElement uiContainer = new UIElement
            {
                Width = { Percent = 0.8f },
                MaxWidth = new StyleDimension(800f, 0f),
                Top = { Pixels = 220f },
                Height = { Pixels = -220f, Percent = 1f },
                HAlign = 0.5f
            };
            Append(uiContainer);

            // Main panel
            UIPanel panel = new UIPanel
            {
                Width = { Percent = 1f },
                Height = { Pixels = -110f, Percent = 1f },
                BackgroundColor = UICommon.MainPanelBackground
            };
            uiContainer.Append(panel);

            // Create a container for our message box
            descriptionContainer = new UIPanel
            {
                Width = { Pixels = -25f, Percent = 1f },
                Height = { Percent = 1f },
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent
            };
            panel.Append(descriptionContainer);

            // Create UIMessageBox using reflection
            if (messageBoxType != null)
            {
                // Create instance with constructor(string)
                messageBox = (UIElement)Activator.CreateInstance(
                    messageBoxType,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new object[] { "" },
                    null
                );

                // Configure the message box
                messageBox.Width.Set(0, 1f);
                messageBox.Height.Set(0, 1f);

                // Add to container
                descriptionContainer.Append(messageBox);
            }

            // Scrollbar
            scrollbar = new UIScrollbar
            {
                Height = { Pixels = -12f, Percent = 1f },
                VAlign = 0.5f,
                HAlign = 1f
            }.WithView(100f, 1000f);
            panel.Append(scrollbar);

            // Connect scrollbar to message box using reflection
            if (messageBox != null && setScrollbarMethod != null)
            {
                setScrollbarMethod.Invoke(messageBox, [scrollbar]);
            }

            // Title panel
            titlePanel = new UITextPanel<string>($"Mod Info: {modDisplayName}", 0.8f, true)
            {
                HAlign = 0.5f,
                Top = { Pixels = -35f },
                BackgroundColor = UICommon.DefaultUIBlue
            }.WithPadding(15f);
            uiContainer.Append(titlePanel);

            // Back button
            // Container for buttons
            var bottomContainer = new UIElement
            {
                Width = { Percent = 1f },
                Height = { Pixels = 40f },
                VAlign = 1f,
                Top = { Pixels = -60f }
            };
            uiContainer.Append(bottomContainer);

            var backButton = new UITextPanel<string>(Language.GetText("UI.Back").Value)
            {
                Width = { Percent = 0.333f },
                Height = { Pixels = 40f }
            }.WithFadedMouseOver();
            backButton.OnLeftClick += BackButton_OnLeftClick;
            bottomContainer.Append(backButton);

            var steamButton = new UITextPanel<string>("Steam Workshop")
            {
                Width = { Percent = 0.333f },
                Height = { Pixels = 40f },
                Left = { Percent = 0.333f }
            }.WithFadedMouseOver();
            bottomContainer.Append(steamButton);

            var deleteButton = new UITextPanel<string>("Delete")
            {
                Width = { Percent = 0.333f },
                Height = { Pixels = 40f },
                Left = { Percent = 0.666f }
            }.WithFadedMouseOver();
            bottomContainer.Append(deleteButton);

            // Try to find the workshop ID for this mod
            FindWorkshopId();
        }

        // Add this method to find the workshop ID
        private void FindWorkshopId()
        {
            try
            {
                if (string.IsNullOrEmpty(modInternalName))
                    return;

                // Get the Assembly
                Assembly assembly = typeof(ModLoader).Assembly;

                // Find the needed types using reflection
                Type interfaceType = assembly.GetType("Terraria.ModLoader.UI.Interface");
                Type modPubIdType = assembly.GetType("Terraria.ModLoader.UI.ModBrowser.ModPubId_t");

                if (interfaceType == null || modPubIdType == null)
                    return;

                // Get the static modBrowser field
                var modBrowserField = interfaceType.GetField("modBrowser", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (modBrowserField == null)
                    return;

                // Get the modBrowser instance
                var modBrowser = modBrowserField.GetValue(null);
                if (modBrowser == null)
                    return;

                // Get the SocialBackend property
                var socialBackendProp = modBrowser.GetType().GetProperty("SocialBackend", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (socialBackendProp == null)
                    return;

                // Get the SocialBackend instance
                var socialBackend = socialBackendProp.GetValue(modBrowser);
                if (socialBackend == null)
                    return;

                // Get the installed mods from SocialBackend
                var getInstalledModsMethod = socialBackend.GetType().GetMethod("GetInstalledMods", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (getInstalledModsMethod == null)
                    return;

                // Get the mod ID lookup method
                var getModIdFromLocalFilesMethod = socialBackend.GetType().GetMethod("GetModIdFromLocalFiles", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (getModIdFromLocalFilesMethod == null)
                    return;

                // Get installed mods
                var installedMods = getInstalledModsMethod.Invoke(socialBackend, null) as IEnumerable<object>;
                if (installedMods == null)
                    return;

                // Find our mod
                foreach (var mod in installedMods)
                {
                    // Get mod name
                    var nameField = mod.GetType().GetField("Name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (nameField == null)
                        continue;

                    string name = nameField.GetValue(mod) as string;
                    if (name != modInternalName)
                        continue;

                    // Found our mod! Now get its modFile
                    var modFileField = mod.GetType().GetField("modFile", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (modFileField == null)
                        continue;

                    var modFile = modFileField.GetValue(mod);
                    if (modFile == null)
                        continue;

                    // Get its workshop ID
                    var parameters = new object[] { modFile, null };
                    bool result = (bool)getModIdFromLocalFilesMethod.Invoke(socialBackend, parameters);
                    if (result && parameters[1] != null)
                    {
                        publishedFileId = parameters[1]; // Store the ModPubId_t
                        Log.Info($"Found workshop ID for mod {modInternalName}");
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error finding workshop ID: {ex.Message}");
            }
        }

        public override void OnActivate()
        {
            // Set the text using reflection when the UI is activated
            if (messageBox != null && setTextMethod != null)
            {
                setTextMethod.Invoke(messageBox, [CurrentModDescription]);
            }

            // Update the header text UITitlePanel
            if (titlePanel != null)
            {
                titlePanel.SetText($"Mod Info: {modDisplayName}");
            }
        }

        private void BackButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            IngameFancyUI.Close();
        }
    }
}