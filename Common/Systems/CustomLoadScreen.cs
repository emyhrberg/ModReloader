using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.Common.Systems
{
    public class CustomLoadScreen : UIState
    {
        // ProgressBar child replicating TML's approach
        private class UIProgressBar : UIPanel
        {
            private string _cachedText = "";
            private UIAutoScaleTextTextPanel<string> _textPanel;
            public float Progress { get; private set; }

            public string DisplayText
            {
                get => _textPanel?.Text ?? _cachedText;
                set
                {
                    if (_textPanel == null)
                        _cachedText = value;
                    else
                        _textPanel.SetText(value ?? _textPanel.Text);
                }
            }

            public override void OnInitialize()
            {
                _textPanel = new UIAutoScaleTextTextPanel<string>(_cachedText ?? "", 1f, true)
                {
                    Top = { Pixels = 10f },
                    HAlign = 0.5f,
                    Width = { Percent = 1f },
                    Height = { Pixels = 60f },
                    DrawPanel = false
                };
                Append(_textPanel);
            }

            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);
                if (!string.IsNullOrEmpty(_cachedText) && _textPanel != null)
                {
                    _textPanel.SetText(_cachedText);
                    _cachedText = string.Empty;
                }
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                base.DrawSelf(spriteBatch);
                var space = GetInnerDimensions();
                int barLeft = (int)space.X + 10;
                int barTop = (int)space.Y + (int)space.Height / 2 + 20;
                int barWidth = (int)space.Width - 20;
                int barHeight = 10;

                // Background (dark blue)
                spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                    new Rectangle(barLeft, barTop, barWidth, barHeight),
                    new Color(0, 0, 70));

                // Fill (yellow)
                spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                    new Rectangle(barLeft, barTop, (int)(barWidth * Progress), barHeight),
                    new Color(255, 255, 70));
            }

            public void UpdateProgress(float value)
            {
                Progress = MathHelper.Clamp(value, 0f, 1f);
            }
        }

        private UIProgressBar _progressBar;
        private UIText _subProgress; // Subtext
        private UITextPanel<LocalizedText> _cancelButton;

        public float Progress
        {
            get => _progressBar.Progress;
            set => _progressBar.UpdateProgress(value);
        }

        public string DisplayText
        {
            get => _progressBar.DisplayText;
            set => _progressBar.DisplayText = value;
        }

        public string SubText
        {
            set => _subProgress?.SetText(value);
        }

        public override void OnInitialize()
        {
            // Main progress bar
            _progressBar = new UIProgressBar
            {
                Width = { Percent = 0.8f },
                Height = { Pixels = 150f },
                HAlign = 0.5f,
                VAlign = 0.5f,
            };
            Append(_progressBar);

            // Subprogress text
            _subProgress = new UIText("", 0.5f, true)
            {
                Top = { Pixels = 65f },
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            Append(_subProgress);

            // Cancel button
            _cancelButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Cancel"), 0.75f, true)
            {
                Top = { Pixels = 170f },
                HAlign = 0.5f,
                VAlign = 0.5f
            }.WithFadedMouseOver();
            _cancelButton.OnLeftClick += CancelClick;
            Append(_cancelButton);
        }

        private void CancelClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(in SoundID.MenuOpen);
            Main.menuMode = 0; // Return to main menu
        }

        public static CustomLoadScreen Show(string mainText = "Loading...", string subText = "")
        {
            // Create instance
            var screen = new CustomLoadScreen();
            // Switch to a custom menuMode
            Main.menuMode = 888;
            // Activate this UI
            Main.MenuUI.SetState(screen);
            // Initialize texts
            screen.DisplayText = mainText;
            screen.SubText = subText;
            screen.Progress = 0f;
            return screen;
        }
    }
}