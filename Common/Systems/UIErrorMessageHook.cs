using System;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using MonoMod.RuntimeDetour;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.Common.Systems
{
    public class MainMenuErrorPopup : ModSystem
    {
        private Hook OnInitializeHook;
        private CopyButton copyButton;

        public override void Load()
        {
            if (Conf.C != null && !Conf.C.ImproveMainMenu)
            {
                Log.Info("MainMenuHook: CreateMainMenuButtons is set to false. Not hooking into Main Menu.");
                return;
            }

            // Get the UIErrorMessage type
            Type UIErrorMessage = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIErrorMessage");

            // Hook OnActivate (public override void OnActivate())
            MethodInfo onActivateMethod = UIErrorMessage.GetMethod("OnActivate", BindingFlags.Public | BindingFlags.Instance);
            if (onActivateMethod != null)
            {
                OnInitializeHook = new Hook(
                    source: onActivateMethod,
                    target: new Action<Action<UIState>, UIState>(OnActivateHook)
                );
            }
            else
            {
                Log.Warn("Could not find UIErrorMessage.OnActivate.");
            }
        }

        public override void Unload()
        {
            OnInitializeHook?.Dispose();

            if (copyButton != null)
            {
                copyButton.Remove();
                copyButton = null;
            }
        }

        private void OnActivateHook(Action<UIState> orig, UIState self)
        {
            orig(self);
            Log.Info("OnActivateHook triggered (after calling original).");

            // Get the error message from the UIErrorMessage
            string errorMessage = GetErrorMessage(self);

            // Get the area of the UIErrorMessage
            FieldInfo areaField = self.GetType().GetField("area", BindingFlags.NonPublic | BindingFlags.Instance);
            UIElement area = (UIElement)areaField.GetValue(self);

            // Check if we have "webHelpButton" in the bottom right spot.
            // If so, we must move our button or have it fade out.
            bool webHelpButtonExists = false;

            foreach (var child in area.Children)
            {
                if (child is UITextPanel<string> textPanel && textPanel.Text == "Open Web Help")
                {
                    Log.Info("Found Web Help button. Moving our button.");
                    webHelpButtonExists = true;
                    break;
                }
            }

            if (webHelpButtonExists)
            {
                // Move our button up
                copyButton = new("Copy to Clipboard", 0.3f, true, errorMessage);
                copyButton.Top.Set(-108 - 30, 1f);
                copyButton.Left.Set(-30, 0f);
                copyButton.Height.Set(20, 0);
                copyButton.Width.Set(200, 0);
                copyButton.WithFadedMouseOver(); // add yellow hover effect
                area.Append(copyButton);
            }
            else
            {
                // Add a custom copy button next to all the others
                copyButton = new("Copy to Clipboard", 0.7f, true, errorMessage);
                copyButton.Top.Set(-108 + 50 + 5, 1f);
                copyButton.WithFadedMouseOver(); // add yellow hover effect
                area.Append(copyButton);
            }
        }

        private string GetErrorMessage(UIState self)
        {
            // Get the error message from the UIErrorMessage
            // private string message from internal class UIErrorMessage : UIState
            Assembly a = typeof(Main).Assembly;
            Type UIErrorMessage = a.GetType("Terraria.ModLoader.UI.UIErrorMessage");
            FieldInfo messageField = UIErrorMessage.GetField("message", BindingFlags.NonPublic | BindingFlags.Instance);
            string errorMessage = (string)messageField.GetValue(self);
            return errorMessage;
        }
    }
}