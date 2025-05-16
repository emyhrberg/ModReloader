using System;
using System.Reflection;
using System.Threading.Tasks;
using ModReloader.Common.Configs;
using ModReloader.Helpers;
using ReLogic.OS;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModReloader.Common.Systems.Hooks
{
    /// <summary>
    /// This should work by dynamically changing the visibility based on the config value.
    /// (As opposed to the old version which hooked into its own state and appended continously.)
    /// </summary>
    public class ExceptionCopyToClipboardHook : ModSystem
    {
        // Variables
        private static UITextPanel<string> copyButton;
        private static bool buttonInitialized = false;

        #region hooks
        public override void Load()
        {
            if (Conf.C != null && !Conf.C.ShowCopyToClipboardButton)
            {
                Log.Info("ExceptionCopyToClipboardHook: ImproveExceptionMenu is set to false. Not hooking into Error Menu.");
                return;
            }
            On_Main.DrawVersionNumber += DrawCopyToClipboard;
        }
        public override void Unload()
        {
            if (Conf.C != null && !Conf.C.ShowCopyToClipboardButton)
            {
                Log.Info("ExceptionCopyToClipboardHook: ImproveExceptionMenu is set to false. Not unloading the hook.");
                return;
            }
            On_Main.DrawVersionNumber -= DrawCopyToClipboard;

            // Clean up button reference
            if (copyButton != null && copyButton.Parent != null)
            {
                copyButton.Parent.RemoveChild(copyButton);
                copyButton = null;
            }
            buttonInitialized = false;
        }
        #endregion

        private static void DrawCopyToClipboard(On_Main.orig_DrawVersionNumber orig, Color menucolor, float upbump)
        {
            // Draw vanilla stuff first
            orig(menucolor, upbump);

            // Check if the feature is enabled in config
            if (Conf.C == null || !Conf.C.ShowCopyToClipboardButton)
            {
                // If the feature is disabled but button exists, remove it
                if (buttonInitialized)
                {
                    RemoveCopyButtonFromErrorUI();
                }
                return;
            }

            // Check if we're in an error screen
            Type uiErrorMessageType = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIErrorMessage");
            if (uiErrorMessageType == null)
                return;

            // Try to get the active instance of UIErrorMessage
            UIState currentState = null;
            var state = Main.MenuUI.CurrentState;
            if (state != null && uiErrorMessageType.IsInstanceOfType(state))
            {
                currentState = state;

                // Check if the button is already initialized
                if (!buttonInitialized && currentState != null)
                {
                    // Initialize the button
                    AppendCopyButtonToErrorUI(currentState);
                    buttonInitialized = true;
                }
            }
        }

        public static void RemoveCopyButtonFromErrorUI()
        {
            // Remove the button if it exists
            if (copyButton != null && copyButton.Parent != null)
            {
                copyButton.Parent.RemoveChild(copyButton);
                copyButton = null;
            }
            buttonInitialized = false;
        }

        private static void AppendCopyButtonToErrorUI(UIState errorUIState)
        {
            // Get the error message
            Type uiErrorMessageType = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIErrorMessage");
            FieldInfo messageField = uiErrorMessageType.GetField("message", BindingFlags.NonPublic | BindingFlags.Instance);
            string errorMessage = messageField?.GetValue(errorUIState) as string;

            // Grab the private "area" field
            FieldInfo areaField = errorUIState.GetType().GetField("area", BindingFlags.NonPublic | BindingFlags.Instance);
            if (areaField == null)
                return;

            if (areaField.GetValue(errorUIState) is not UIElement area)
                return;

            // Check if we have "webHelpButton" in the bottom right spot.
            // If so, we must move our button or have it fade out.
            bool webHelpButtonExists = false;

            foreach (var child in area.Children)
            {
                if (child is UITextPanel<string> textPanel && textPanel.Text == "Open Web Help")
                {
                    //Log.SlowInfo("Found Web Help button. Moving our button.");
                    webHelpButtonExists = true;
                    break;
                }
            }

            float yOffset = 0;
            if (webHelpButtonExists)
            {
                //yOffset = -800;
                return;
            }

            // Create a standard UITextPanel with typical tModLoader colors
            copyButton = new UITextPanel<string>("Copy to Clipboard", 0.7f, large: true)
            {
                // BackgroundColor = new Color(73, 94, 171) * 0.9f, // Brighter blue for better visibility
                BorderColor = Color.Black,
                PaddingTop = 8f,
                PaddingBottom = 8f,
            };

            // Position
            copyButton.Width.Set(-10, 0.5f);
            // copyButton.Width.Set(200, 0f);
            copyButton.Height.Set(50f, 0f);
            copyButton.HAlign = 1f;  // Right align
            copyButton.VAlign = 1f;  // Bottom align
            copyButton.Left.Set(0f, 0f);  // Offset from right edge
            copyButton.Top.Set(yOffset, 0f);    // Offset from bottom edge
            copyButton.WithFadedMouseOver();
            // Hook up the click to copy with visual feedback
            copyButton.OnLeftClick += (evt, element) =>
            {
                if (errorMessage != null)
                {
                    Platform.Get<IClipboard>().Value = errorMessage;

                    // Visual feedback that copying worked
                    string originalText = copyButton.Text;
                    copyButton.SetText("Copied!");
                    copyButton.BackgroundColor = new Color(40, 130, 50) * 0.9f; // Green for success

                    // Reset after a moment
                    Task.Run(async () =>
                    {
                        await Task.Delay(1000); // About 1 second
                        Main.QueueMainThreadAction(() =>
                        {
                            copyButton.SetText(originalText);
                            copyButton.BackgroundColor = new Color(73, 94, 171) * 0.9f;
                        });
                    });
                }
            };

            area.Append(copyButton);
            Log.Info("Copy button appended to error UI");
        }
    }
}
