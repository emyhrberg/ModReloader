using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.UI;
using SquidTestingMod.UI.Elements;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace SquidTestingMod.Common.Systems
{
    public class DebugState : UIState
    {
        // Flag to enable/disable UI debug drawing
        public bool showAll = false;
        public bool isUIDebugSizeElementDrawing = false;

        // Elements
        public List<UIElement> elements = new();
        private Dictionary<UIElement, bool> elementToggles = new();

        // Outline color
        private Color outlineColor = Color.White;
        public void SetOutlineColor(Color color) => outlineColor = color;
        private bool randomOutlineColor = false;
        public void ToggleRandomOutlineColor() => randomOutlineColor = !randomOutlineColor;

        // Thickness
        private int thickness = 1;
        public void SetThickness(int value) => thickness = value;

        // Toggle stuff
        public void ToggleShowSize() => isUIDebugSizeElementDrawing = !isUIDebugSizeElementDrawing;
        public void ToggleElement(string typeName)
        {
            UIElement firstMatch = elements.Find(e => e.GetType().Name == typeName);
            if (firstMatch == null)
                return;

            // Find out if it's currently ON or OFF
            bool oldValue = elementToggles[firstMatch];
            bool newValue = !oldValue;

            // Flip for all elements of that type
            foreach (var elem in elements)
            {
                if (elem.GetType().Name == typeName)
                {
                    elementToggles[elem] = newValue;
                }
            }
        }

        public void ToggleShowAll()
        {
            showAll = !showAll;
            if (showAll)
                foreach (var elem in elements)
                    elementToggles[elem] = false;
            else
                foreach (var elem in elements)
                    elementToggles[elem] = true;


            // Update text
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            UiPanel uiPanel = sys.mainState.uiPanel;

            foreach (var uiElement in uiPanel.dynamicOptions.Values)
            {
                if (uiElement is Option o)
                {
                    if (showAll)
                    {
                        o.SetState(Option.State.Enabled);
                    }
                    else
                    {
                        o.SetState(Option.State.Disabled);
                    }
                }
            }
        }

        // Settings
        private List<Color> rainbowColors;
        private float opacity = 0.1f;
        private float xOffset = 0;
        private float yOffset = 0;
        private float textSize = 0.5f;

        public DebugState()
        {
            GenerateRainbowColors(20);
        }

        public void SetXOffset(float value) => xOffset = value;
        public void SetYOffset(float value) => yOffset = value;
        public void SetOpacity(float value) => opacity = value;
        public void RandomizeRainbowColors() => rainbowColors = rainbowColors.OrderBy(_ => Main.rand.Next()).ToList();
        public void SetTextSize(float value) => textSize = value;

        public void DrawHitbox(UIElement element, SpriteBatch spriteBatch)
        {
            Rectangle hitbox = element.GetOuterDimensions().ToRectangle();

            // Get a color from the rainbow
            int colorIndex = element.UniqueId % rainbowColors.Count;
            Color hitboxColor = rainbowColors[colorIndex] * opacity;

            // Draw the hitbox
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, hitboxColor);

            // Draw outline
            if (randomOutlineColor)
            {
                DrawOutline(spriteBatch, hitbox, rainbowColors[colorIndex]);
            }
            else
            {
                DrawOutline(spriteBatch, hitbox, outlineColor);
            }
        }

        public void DrawElementLabel(SpriteBatch spriteBatch, UIElement element, Point position)
        {
            // round width and height to whole numbers
            int width = (int)element.GetOuterDimensions().Width;
            int height = (int)element.GetOuterDimensions().Height;

            string sizeText = $"{width}x{height}";

            Vector2 textSize2 = FontAssets.MouseText.Value.MeasureString(sizeText) * textSize; // Smaller text
            Vector2 textPosition = new(position.X + xOffset, position.Y - textSize2.Y + yOffset); // Offset to the left by 5px

            ChatManager.DrawColorCodedStringWithShadow(
                spriteBatch,
                FontAssets.MouseText.Value,
                sizeText,
                textPosition,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(textSize)); // Scale down text
        }

        private void DrawOutline(SpriteBatch spriteBatch, Rectangle hitbox, Color? rainbowColor = null)
        {
            Texture2D t = TextureAssets.MagicPixel.Value;

            // If rainbowcolor is given, draw with that. Otherwise, draw with outlineColor.
            Color colorToDraw = rainbowColor ?? this.outlineColor;

            // Thinner outline width (1 pixel instead of 2)
            spriteBatch.Draw(t, new Rectangle(hitbox.X, hitbox.Y, hitbox.Width, thickness), colorToDraw);
            spriteBatch.Draw(t, new Rectangle(hitbox.X, hitbox.Y, thickness, hitbox.Height), colorToDraw);
            spriteBatch.Draw(t, new Rectangle(hitbox.X + hitbox.Width - thickness, hitbox.Y, thickness, hitbox.Height), colorToDraw);
            spriteBatch.Draw(t, new Rectangle(hitbox.X, hitbox.Y + hitbox.Height - thickness, hitbox.Width, thickness), colorToDraw);
        }


        private void GenerateRainbowColors(int count)
        {
            rainbowColors = [];
            for (int i = 0; i < count; i++)
            {
                float hue = (float)i / count;
                rainbowColors.Add(Main.hslToRgb(hue, 1f, 0.5f));
            }
        }

        public void UIElement_Draw(On_UIElement.orig_Draw orig, UIElement self, SpriteBatch spriteBatch)
        {
            orig(self, spriteBatch); // Normal UI behavior

            // Register the element
            if (!elements.Contains(self))
            {
                elements.Add(self);

                // Ensure the element has a default toggle state (ON by default)
                if (!elementToggles.ContainsKey(self))
                {
                    elementToggles[self] = true;
                }
            }

            if (Main.dedServ || Main.gameMenu)
                return;
            if (self is MainState || self is DebugState)
                return;
            if (self.GetOuterDimensions().Width > 900 || self.GetOuterDimensions().Height > 900)
                return;

            // NEW: Check if this *type* is toggled OFF
            if (elementToggles.ContainsKey(self) && elementToggles[self])
                return;

            if (isUIDebugSizeElementDrawing)
            {
                DrawElementLabel(spriteBatch, self, self.GetOuterDimensions().Position().ToPoint());
            }

            DrawHitbox(self, spriteBatch);
        }

        public void PrintDimensionsForType(string typeName)
        {
            var matches = elements.Where(e => e.GetType().Name == typeName).ToList();
            Main.NewText($"UIElements: {typeName} [total UIElements: {matches.Count}]", Color.Green);
            for (int i = 0; i < matches.Count; i++)
            {
                var elem = matches[i];
                var outer = elem.GetOuterDimensions();
                var inner = elem.GetInnerDimensions();

                // Round dimensions to 2 decimal places
                float outerWidth = (float)Math.Round(outer.Width, 2);
                float outerHeight = (float)Math.Round(outer.Height, 2);
                float innerWidth = (float)Math.Round(inner.Width, 2);
                float innerHeight = (float)Math.Round(inner.Height, 2);

                // If outer width matches inner width and outer height matches inner height, only print one size
                if (outerWidth == innerWidth && outerHeight == innerHeight)
                {
                    Main.NewText($"{i + 1}. {typeName}: {outerWidth}x{outerHeight}", Color.White);
                }
                else
                {
                    Main.NewText($"{i + 1}. {typeName}: Inner: {innerWidth}x{innerHeight}, Outer: {outerWidth}x{outerHeight}", Color.White);
                }
            }
        }

        // public void PrintAllUIElements()
        // {
        //     Main.NewText($"UIElements: (Name), Width x Height [total UIElements: {elements.Count}]", Color.Green);

        //     // Track which names have already been shown
        //     HashSet<string> shownNames = new();

        //     // Sort elements by name
        //     elements.Sort((a, b) => string.Compare(a.GetType().Name, b.GetType().Name, StringComparison.Ordinal));

        //     int count = 1;
        //     for (int i = 0; i < elements.Count; i++)
        //     {
        //         var element = elements[i];
        //         string elementName = element.GetType().Name;

        //         if (!shownNames.Add(elementName))
        //             continue; // Skip if we've already shown this name

        //         string elementText = $"{count}. ({elementName})";
        //         if (element.GetInnerDimensions().Width == element.GetOuterDimensions().Width
        //             && element.GetInnerDimensions().Height == element.GetOuterDimensions().Height)
        //         {
        //             elementText += $", {element.GetInnerDimensions().Width}x{element.GetInnerDimensions().Height}";
        //         }
        //         else
        //         {
        //             elementText += $", Inner: {element.GetInnerDimensions().Width}x{element.GetInnerDimensions().Height}, Outer: {element.GetOuterDimensions().Width}x{element.GetOuterDimensions().Height}";
        //         }

        //         Main.NewText(elementText, Color.White);
        //         count++;
        //     }
        // }
    }
}
