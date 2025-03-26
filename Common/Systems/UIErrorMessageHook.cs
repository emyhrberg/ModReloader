using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using EliteTestingMod.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace EliteTestingMod.Common.Systems
{
    public class MainMenuErrorPopup : ModSystem
    {
        private Hook OnInitializeHook;
        private CopyButton copyButton;

        public override void Load()
        {
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

            // Add a custom copy button next to all the others
            copyButton = new("Copy To Clipboard", 0.7f, true, errorMessage);
            copyButton.WithFadedMouseOver(); // add yellow hover effect
            area.Append(copyButton);

            if (webHelpButtonExists)
            {
                // Move our button up
                copyButton.Top.Set(-108 - 50, 1f);
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

        public class CopyButton : UITextPanel<string>
        {
            private string errorMessage;
            private bool hasCopied = false;
            DateTime copyTime;

            public CopyButton(string text, float textScale, bool large, string errorMessage) : base(text, textScale, large)
            {
                this.errorMessage = errorMessage;

                // Bottom right position. There are 3 other buttons.
                Width.Set(-10, 0.5f);
                Height.Set(50, 0f);
                Top.Set(-108 + 50 + 5, 1f);
                HAlign = 1.0f;
            }

            public override void LeftClick(UIMouseEvent evt)
            {
                base.LeftClick(evt);

                ReLogic.OS.Platform.Get<IClipboard>().Value = errorMessage;
                Log.Info("Copied error message to clipboard.");
                hasCopied = true;

                // Start the timer
                copyTime = DateTime.Now;
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);

                // Draw the button with hover:
                if (IsMouseHovering)
                {
                    if (!hasCopied)
                        UICommon.TooltipMouseText("Copy the current exception message to your clipboard");
                    else
                    {
                        UICommon.TooltipMouseText("Copied!");

                        // If it's been 3 seconds, reset the button
                        TimeSpan interval = TimeSpan.FromSeconds(3);
                        bool has3SecondsPassed = DateTime.Now - copyTime >= interval;
                        if (has3SecondsPassed)
                        {
                            hasCopied = false;
                        }
                    }
                }
            }
        }
    }
}