using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements.ModElements
{
    //ty jopojelly and darthmorf
    public class Searchbox : UIPanel
    {
        internal string currentString = string.Empty;

        internal bool focused = false;

        private readonly int _maxLength = 20;

        private readonly string hintText;
        private int textBlinkerCount;
        private int textBlinkerState;

        public event Action OnFocus;

        public event Action OnUnfocus;

        public event Action OnTextChanged;

        public event Action OnTabPressed;

        public event Action OnEnterPressed;

        internal bool unfocusOnEnter = true;

        internal bool unfocusOnTab = true;

        internal Searchbox(string hintText, string text = "")
        {
            this.hintText = hintText;
            currentString = text;
            SetPadding(0);
            BackgroundColor = Color.White;
            BorderColor = Color.Black;

            // more:
            BackgroundColor = Color.White;
            BorderColor = Color.Black;

            // uiList parent container will do positioning for us.
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            Focus();
            base.LeftClick(evt);
        }

        internal void Unfocus()
        {
            if (focused)
            {
                focused = false;
                Main.blockInput = false;

                OnUnfocus?.Invoke();
            }
        }

        internal void Focus()
        {
            if (!focused)
            {
                Main.clrInput();
                focused = true;
                Main.blockInput = true;

                OnFocus?.Invoke();
            }
        }

        // public override bool ContainsPoint(Vector2 point)
        // {
        //     // Use the panel’s actual dimensions (outer or inner as you prefer):
        //     CalculatedStyle dims = GetInnerDimensions();
        //     Rectangle hitbox = dims.ToRectangle();

        //     // Inflate by 50% on each axis:
        //     hitbox.Inflate(hitbox.Width / 2, hitbox.Height / 2);

        //     return hitbox.Contains((int)point.X, (int)point.Y);
        // }

        public override void Update(GameTime gameTime)
        {
            // if (IsMouseHovering)
            // {
            //     Log.Info("Mouse is hovering over searchbox");
            // }
            // else
            // {
            //     Log.Info("Not hovering over searchbox");
            // }

            Vector2 MousePosition = new(Main.mouseX, Main.mouseY);
            if (!ContainsPoint(MousePosition) && (Main.mouseLeft || Main.mouseRight)) //This solution is fine, but we need a way to cleanly "unload" a UIElement
            {
                //TODO, figure out how to refocus without triggering unfocus while clicking enable button.
                Unfocus();
            }
            base.Update(gameTime);
        }

        internal void SetText(string text)
        {
            if (text.Length > _maxLength)
            {
                text = text.Substring(0, _maxLength);
            }
            if (currentString != text)
            {
                currentString = text;
                OnTextChanged?.Invoke();
            }
        }

        private static bool JustPressed(Keys key)
        {
            return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle hitbox = GetInnerDimensions().ToRectangle();

            // Draw panel
            base.DrawSelf(spriteBatch);

            // draw debug
            // DrawHelper.DrawDebugHitbox(this, Color.Red);

            if (focused)
            {
                Terraria.GameInput.PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                string newString = Main.GetInputText(currentString);
                if (!newString.Equals(currentString))
                {
                    currentString = newString;
                    OnTextChanged?.Invoke();
                }
                else
                {
                    currentString = newString;
                }

                if (JustPressed(Keys.Tab))
                {
                    if (unfocusOnTab) Unfocus();
                    OnTabPressed?.Invoke();
                }
                if (JustPressed(Keys.Enter))
                {
                    Main.drawingPlayerChat = false;
                    if (unfocusOnEnter) Unfocus();
                    OnEnterPressed?.Invoke();
                }
                if (++textBlinkerCount >= 20)
                {
                    textBlinkerState = (textBlinkerState + 1) % 2;
                    textBlinkerCount = 0;
                }
                Main.instance.DrawWindowsIMEPanel(new Vector2(98f, Main.screenHeight - 36), 0f);
            }
            string displayString = currentString;
            if (textBlinkerState == 1 && focused)
            {
                displayString += "|";
            }
            CalculatedStyle space = GetDimensions();
            Color color = Color.White;

            // Position
            Vector2 drawPos = space.Position() + new Vector2(8, 6);
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            float textScale = 0.9f; // Adjust this value to resize the text

            // Draw outline
            //if (!string.IsNullOrEmpty(currentString))
            //{
            Color outlineColor = Color.Black;
            Vector2[] offsets =
            [
                new(-1, -1),
                    new(1, -1),
                    new(-1, 1),
                    new(1, 1)
            ];

            foreach (var offset in offsets)
            {
                spriteBatch.DrawString(font, displayString, drawPos + offset, outlineColor, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
            }
            //}

            // Draw text
            if (currentString.Length == 0)
            {
                // Draw hintText
                color = Color.DimGray;
                spriteBatch.DrawString(font, hintText, drawPos, color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
            }
            else
            {
                // Draw currentString
                spriteBatch.DrawString(font, displayString, drawPos, color, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
            }
        }
    }
}