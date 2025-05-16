using System;
using System.Reflection;
using ModReloader.Common.Systems;
using ModReloader.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.UI;
using Terraria.Social.Steam;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements.ModElements
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
        private string workshopURL;

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
                    [""],
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
                Left = { Percent = 0.333f },
            }.WithFadedMouseOver();
            steamButton.OnLeftClick += SteamButton_OnLeftClick;
            bottomContainer.Append(steamButton);

            var deleteButton = new UITextPanel<string>("Delete")
            {
                Width = { Percent = 0.333f },
                Height = { Pixels = 40f },
                Left = { Percent = 0.666f }
            }.WithFadedMouseOver();
            deleteButton.OnLeftClick += DeleteButton_OnLeftClick;
            bottomContainer.Append(deleteButton);
        }

        // Add this method to find the workshop ID
        private void GetAndSetWorkshopURL()
        {
            Log.Info("Finding workshop ID for mod: " + modInternalName);

            // Get the Mod instance for the current mod
            // This only works for enabled mods.
            LocalMod[] mods = ModOrganizer.FindAllMods();
            LocalMod mod = Array.Find(mods, m => m.Name.Equals(modInternalName, StringComparison.OrdinalIgnoreCase));
            TmodFile modFile = mod.modFile;

            if (mod != null)
            {
                // Use the correct signature for GetPublishIdLocal
                if (WorkshopHelper.GetPublishIdLocal(modFile, out ulong publishId))
                {
                    Log.Info("Found publish ID: " + publishId);
                    // Set the workshop URL using the publish ID
                    workshopURL = $"https://steamcommunity.com/sharedfiles/filedetails/?id={publishId}";
                }
                else
                {
                    // Set a default message if not found
                    workshopURL = null;
                }
            }
            else
            {
                workshopURL = null;
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

            // Try to find the workshop ID for this mod
            GetAndSetWorkshopURL();
        }

        private void DeleteButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            // Get the mod
            LocalMod[] mods = ModOrganizer.FindAllMods();
            LocalMod mod = Array.Find(mods, m => m.Name.Equals(modInternalName, StringComparison.OrdinalIgnoreCase));

            // check if null, return
            if (mod == null)
            {
                Log.Error("Failed to find mod: " + modDisplayName);
                Main.NewText($"Failed to find mod: {modDisplayName}", Color.Orange);
                return;
            }

            // Delete the mod
            ModOrganizer.DeleteMod(mod);

            // TODO update the UIList of mods. Call FilterItem
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            RebuildModLists(sys.mainState.modsPanel); // YES, it works! (atleast i tested deleting one mod and it rebuilt properly)

            // now we close the panel and write to Main.newText
            IngameFancyUI.Close();
            Main.NewText($"Deleted mod: {modDisplayName}", Color.Orange);
        }

        private static void RebuildModLists(ModsPanel panel)
        {
            // Clear entire UIList 
            panel.uiList.Clear();

            // Add initial top container and padding
            panel.AddPadding(10f);
            panel.uiList.Add(panel.topContainer);
            panel.AddPadding(20f);

            // Clear existing lists
            panel.enabledMods.Clear();
            panel.allMods.Clear();

            panel.ConstructEnabledMods();
            panel.ConstructAllMods();

            // Ensure everything is properly displayed
            panel.Recalculate();
        }

        private void SteamButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (string.IsNullOrEmpty(workshopURL))
            {
                Main.NewText($"Failed to open workshop page for {modDisplayName}.", Color.Orange);
                return;
            }

            // Use Terraria's built-in URL opener
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = workshopURL,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Main.NewText($"Failed to open workshop page: {ex.Message}", Color.Red);
            }
            Main.NewText("Opening workshop page...", Color.Green);
        }

        // Update ModInfoIcon to pass the modName:
        public void SetModInfo(string description, string displayName, string internalName)
        {
            CurrentModDescription = description;
            modDisplayName = displayName;
            modInternalName = internalName;
        }

        private void BackButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            IngameFancyUI.Close();
        }
    }
}