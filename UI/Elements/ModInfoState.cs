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