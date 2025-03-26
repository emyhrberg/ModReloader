using System;
using System.Collections.Generic;
using System.Linq;
using EliteTestingMod.Common.Configs;
using EliteTestingMod.UI;
using EliteTestingMod.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace EliteTestingMod.Common.Systems
{
    public class UIElementState : UIState
    {
        // Flag to enable/disable UI debug drawing
        public bool showAll = false;
        public bool DrawSizeOfElement = false;
        public bool DrawNameOfElement = false;

        // Elements
        public List<UIElement> elements = new();
        private Dictionary<UIElement, bool> elementToggles = new();

        // Outline color
        private Color outlineColor = Color.White;
        public void SetOutlineColor(Color color) => outlineColor = color;
        private bool randomOutlineColor = false;
        public void ToggleRandomOutlineColor() => randomOutlineColor = !randomOutlineColor;

        // the random offset in X and Y
        private int Random => Main.rand.Next(-20, 20);

        // Randomize offset
        public void RandomizeSizeOffset()
        {
            sizeOffsets.Clear();
            foreach (var elem in elements)
            {
                sizeOffsets[elem] = new Vector2(Random, Random);
            }
        }
        public void RandomizeTypeOffset()
        {
            typeOffsets.Clear();
            foreach (var elem in elements)
            {
                typeOffsets[elem] = new Vector2(Random, Random);
            }
        }
        public void ResetSizeOffset() => sizeOffsets.Clear();
        public void ResetTypeOffset() => typeOffsets.Clear();

        // Thickness
        private int thickness = 1;
        public void SetThickness(int value) => thickness = value;

        // Toggle stuff
        public void ToggleShowSize() => DrawSizeOfElement = !DrawSizeOfElement;
        public void ToggleShowType() => DrawNameOfElement = !DrawNameOfElement;
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
            {
                if (Conf.LogToChat) Main.NewText("Showing all UI elements.", Color.Green);
                foreach (var elem in elements)
                    elementToggles[elem] = true;
            }
            else
            {
                if (Conf.LogToChat) Main.NewText("Hiding all UI elements.", new Color(226, 57, 39));
                foreach (var elem in elements)
                    elementToggles[elem] = true;
            }

            // Update text
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            UIElementPanel uiPanel = sys.mainState.uiPanel;

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

        // Text settings
        private float SizeXOffset = 0;
        private float SizeYOffset = 0;
        private float SizeTextSize = 0.5f;

        public UIElementState()
        {
            GenerateRainbowColors(count: 20);
        }

        public void SetSizeXOffset(float value) => SizeXOffset = value;
        public void SetSizeYOffset(float value) => SizeYOffset = value;
        public void SetOpacity(float value) => opacity = value;
        public void RandomizeRainbowColors() => rainbowColors = rainbowColors.OrderBy(_ => Main.rand.Next()).ToList();
        public void SetSizeTextSize(float value) => SizeTextSize = value;

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

        public void DrawElementType(SpriteBatch spriteBatch, UIElement element, Point position)
        {
            string typeText = element.GetType().Name;
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(typeText) * SizeTextSize;
            Vector2 textPosition = new Vector2(position.X, position.Y) - new Vector2(0, textSize.Y);

            // Use the cached offset if one exists
            if (typeOffsets.TryGetValue(element, out Vector2 offset))
            {
                textPosition += offset;
            }
            else
            {
                textPosition += new Vector2(SizeXOffset, SizeYOffset);
            }

            ChatManager.DrawColorCodedStringWithShadow(
                spriteBatch,
                FontAssets.MouseText.Value,
                typeText,
                textPosition,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(SizeTextSize));
        }

        public void DrawElementSize(SpriteBatch spriteBatch, UIElement element, Point position)
        {
            // Round dimensions of the element.
            int width = (int)element.GetOuterDimensions().Width;
            int height = (int)element.GetOuterDimensions().Height;
            string sizeText = $"{width}x{height}";

            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(sizeText) * SizeTextSize;
            Vector2 textPosition = new Vector2(position.X, position.Y) - new Vector2(0, textSize.Y);

            // Use the cached offset if one exists
            if (sizeOffsets.TryGetValue(element, out Vector2 offset))
            {
                textPosition += offset;
            }
            else
            {
                textPosition += new Vector2(SizeXOffset, SizeYOffset);
            }

            ChatManager.DrawColorCodedStringWithShadow(
                spriteBatch,
                FontAssets.MouseText.Value,
                sizeText,
                textPosition,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(SizeTextSize));
        }

        private void DrawOutline(SpriteBatch spriteBatch, Rectangle hitbox, Color? rainbowColor = null)
        {
            if (thickness == 0)
                return;

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

        // New dictionaries to cache offsets by UIElement.
        private Dictionary<UIElement, Vector2> sizeOffsets = new();
        private Dictionary<UIElement, Vector2> typeOffsets = new();

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
            if (self is MainState || self is UIElementState)
                return;
            if (self.GetOuterDimensions().Width > 900 || self.GetOuterDimensions().Height > 900)
                return;

            // NEW: Check if this *type* is toggled OFF
            if (elementToggles.ContainsKey(self) && elementToggles[self])
                return;

            if (DrawSizeOfElement)
            {
                DrawElementSize(spriteBatch, self, self.GetOuterDimensions().Position().ToPoint());
            }

            if (DrawNameOfElement)
            {
                DrawElementType(spriteBatch, self, self.GetOuterDimensions().Position().ToPoint());
            }

            DrawHitbox(self, spriteBatch);
        }
    }
}
