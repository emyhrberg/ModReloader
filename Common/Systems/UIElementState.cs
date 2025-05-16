using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.UI.Elements.PanelElements;
using Terraria.GameContent;
using Terraria.UI;

namespace ModReloader.Common.Systems
{
    public class UIElementState : UIState
    {
        #region So many variables

        // Flag to enable/disable UI debug drawing
        public bool showAll = false;
        public bool DrawSizeOfElement = false;
        public bool DrawNameOfElement = false;

        // New dictionaries to cache offsets by UIElement.
        private Dictionary<UIElement, Vector2> sizeOffsets = [];
        private Dictionary<UIElement, Vector2> typeOffsets = [];

        // Elements
        public List<UIElement> elements = [];
        private readonly Dictionary<UIElement, bool> elementToggles = [];

        // Outline color
        private Color outlineColor = Color.White;
        public void SetOutlineColor(Color color) => outlineColor = color;
        private bool randomOutlineColor = false;
        public void ToggleRandomOutlineColor() => randomOutlineColor = !randomOutlineColor;

        // Thickness
        private int thickness = 1;
        public void SetThickness(int value) => thickness = value;

        // Settings
        private List<Color> rainbowColors;
        private float opacity = 0.1f;

        // Size text
        private float SizeXOffset = 0;
        private float SizeYOffset = 0;
        private float SizeTextSize = 0.5f;
        public void SetSizeXOffset(float value) => SizeXOffset = value;
        public void SetSizeYOffset(float value) => SizeYOffset = value;
        public void SetSizeTextSize(float value) => SizeTextSize = value;

        // Type text
        private float TypeXOffset = 0;
        private float TypeYOffset = 0;
        private float TypeTextSize = 0.5f;
        public void SetTypeXOffset(float value) => TypeXOffset = value;
        public void SetTypeYOffset(float value) => TypeYOffset = value;
        public void SetTypeTextSize(float value) => TypeTextSize = value;

        // Toggle stuff
        public void ToggleShowSize() => DrawSizeOfElement = !DrawSizeOfElement;
        public void ToggleShowType() => DrawNameOfElement = !DrawNameOfElement;

        public void ResetSizeOffset() => sizeOffsets.Clear();
        public void ResetTypeOffset() => typeOffsets.Clear();

        // Misc
        public void SetOpacity(float value) => opacity = value;
        public void RandomizeRainbowColors() => rainbowColors = rainbowColors.OrderBy(_ => Main.rand.Next()).ToList();
        private static int Random => Main.rand.Next(-20, 20); // the random offset in X and Y

        #endregion

        #region Constructor
        public UIElementState()
        {
            GenerateRainbowColors(count: 20);
        }
        #endregion

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
                Main.NewText("Showing all UI elements.", Color.Green);
                foreach (var elem in elements)
                    elementToggles[elem] = false;
            }
            else
            {
                Main.NewText("Hiding all UI elements.", new Color(226, 57, 39));
                foreach (var elem in elements)
                    elementToggles[elem] = true;
            }

            // Update text
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            UIElementPanel uiPanel = sys.mainState.uiElementPanel;

            foreach (var uiElement in uiPanel.dynamicOptions.Values)
            {
                if (uiElement is OptionElement o)
                {
                    if (showAll)
                    {
                        o.SetState(OptionElement.EnabledState.Enabled);
                    }
                    else
                    {
                        o.SetState(OptionElement.EnabledState.Disabled);
                    }
                }
            }
        }

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
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(typeText) * TypeTextSize;
            Vector2 textPosition = new Vector2(position.X, position.Y) - new Vector2(0, textSize.Y);

            // Use the cached offset if one exists
            if (typeOffsets.TryGetValue(element, out Vector2 offset))
            {
                textPosition += offset;
            }
            else
            {
                textPosition += new Vector2(TypeXOffset, TypeYOffset);
            }

            Utils.DrawBorderStringFourWay(
                spriteBatch,
                FontAssets.MouseText.Value,
                typeText,
                textPosition.X,
                textPosition.Y,
                Color.White,
                Color.Black,
                new Vector2(TypeTextSize),
                TypeTextSize);

            // ChatManager.DrawColorCodedStringWithShadow(
            //     spriteBatch: spriteBatch,
            //     font: FontAssets.MouseText.Value,
            //     text: typeText,
            //     position: textPosition,
            //     baseColor: Color.White,
            //     rotation: 0f,
            //     origin: Vector2.Zero,
            //     baseScale: new Vector2(TypeTextSize),
            //     maxWidth: -1,
            //     spread: 2f);
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

            Utils.DrawBorderStringFourWay(
                spriteBatch,
                FontAssets.MouseText.Value,
                text: sizeText,
                textPosition.X,
                textPosition.Y,
                Color.White,
                Color.Black,
                new Vector2(SizeTextSize),
                SizeTextSize);

            // ChatManager.DrawColorCodedStringWithShadow(
            //     spriteBatch,
            //     FontAssets.MouseText.Value,
            //     sizeText,
            //     textPosition,
            //     Color.White,
            //     0f,
            //     Vector2.Zero,
            //     new Vector2(SizeTextSize));
        }

        private void DrawOutline(SpriteBatch spriteBatch, Rectangle hitbox, Color? rainbowColor = null)
        {
            if (thickness == 0)
                return;

            Texture2D t = TextureAssets.MagicPixel.Value;

            // If rainbowcolor is given, draw with that. Otherwise, draw with outlineColor.
            Color colorToDraw = rainbowColor ?? outlineColor;

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
            if (self is MainState || self is UIElementState)
                return;
            if (self.GetOuterDimensions().Width > 900 || self.GetOuterDimensions().Height > 900)
                return;

            // Check if this *type* is toggled OFF
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