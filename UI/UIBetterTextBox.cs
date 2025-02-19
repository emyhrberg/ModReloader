using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    //ty jopojelly and darthmorf
    public class UIBetterTextBox : UIPanel
    {
        internal string currentString = string.Empty;

        internal bool focused = false;

        private readonly int _maxLength = int.MaxValue;

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

        internal UIBetterTextBox(string hintText, string text = "")
        {
            this.hintText = hintText;
            currentString = text;
            SetPadding(0);
            BackgroundColor = Color.White;
            BorderColor = Color.Black;
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

        public override void Update(GameTime gameTime)
        {
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

            //Draw panel
            base.DrawSelf(spriteBatch);

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

            // position
            Vector2 drawPos = space.Position() + new Vector2(8, 4);
            DynamicSpriteFont font = FontAssets.MouseText.Value;

            // draw outline
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
                spriteBatch.DrawString(font, displayString, drawPos + offset, outlineColor);
            }

            // draw text
            if (currentString.Length == 0 && !focused)
            {
                // Draw hintText
                color = Color.DimGray;
                spriteBatch.DrawString(font, hintText, drawPos, color);
            }
            else
            {
                // Draw currentString
                spriteBatch.DrawString(font, displayString, drawPos, color);
            }


        }
    }
}