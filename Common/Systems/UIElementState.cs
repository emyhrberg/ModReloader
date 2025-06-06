using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.UI.Elements.PanelElements;
using Terraria.GameContent;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.Default;
using Terraria.UI;

namespace ModReloader.Common.Systems
{
    public class UIElementState : UIState
    {
        #region So many variables

        // Flag to enable/disable UI debug drawing
        public bool showAll = false;
        public bool DrawHitboxOfElement = false;
        public bool DrawSizeOfElement = false;
        public bool DrawNameOfElement = false;

        // New dictionaries to cache offsets by UIElement.
        private Dictionary<UIElement, Vector2> sizeOffsets = [];
        private Dictionary<UIElement, Vector2> typeOffsets = [];

        // Elements
        public Dictionary<string, bool> elementToggles = [];

        // Elements that no longer exists
        public HashSet<string> activeUIElementsNameList = [];

        // Outline color
        private Color outlineColor = Color.White;
        public void SetOutlineColor(Color color) => outlineColor = color;

        public Color GetOutlineColor() => outlineColor;

        private bool randomOutlineColor = false;
        public void ToggleRandomOutlineColor() => randomOutlineColor = !randomOutlineColor;

        // Thickness
        private int thickness = 1;
        public void SetThickness(int value) => thickness = value;

        public int GetThickness() => thickness;

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

        // Getters
        public float GetOpacity() => opacity;
        public bool GetDrawSizeOfElement() => DrawSizeOfElement;
        public bool GetDrawHitboxOfElement() => DrawHitboxOfElement;
        public bool GetRandomOutlineColor() => randomOutlineColor;
        public float GetSizeXOffset() => SizeXOffset;
        public float GetSizeYOffset() => SizeYOffset;
        public float GetSizeTextSize() => SizeTextSize;
        public float GetTypeXOffset() => TypeXOffset;
        public float GetTypeYOffset() => TypeYOffset;
        public float GetTypeTextSize() => TypeTextSize;
        public bool GetDrawNameOfElement() => DrawNameOfElement;

        // Toggle stuff
        public void SetDrawHitboxOfElement(bool value) => DrawHitboxOfElement = value;
        public void SetDrawSizeOfElement(bool value) => DrawSizeOfElement = value;
        public void SetDrawNameOfElement(bool value) => DrawNameOfElement = value;

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
            // Load settings from JSON
            UIElementSettingsJson.Initialize();
            opacity = UIElementSettingsJson.TryGetValue("Opacity", 0.1f);
            DrawHitboxOfElement = UIElementSettingsJson.TryGetValue("ShowHitbox", false);
            DrawSizeOfElement = UIElementSettingsJson.TryGetValue("ShowSize", false);
            DrawNameOfElement = UIElementSettingsJson.TryGetValue("ShowType", false);
            outlineColor = UIElementSettingsJson.TryGetValue("OutlineColor", Color.White);
            randomOutlineColor = UIElementSettingsJson.TryGetValue("RandomOutlineColor", false);
            thickness = UIElementSettingsJson.TryGetValue("Thickness", 1);
            SizeXOffset = UIElementSettingsJson.TryGetValue("SizeXOffset", 0);
            SizeYOffset = UIElementSettingsJson.TryGetValue("SizeYOffset", 0);
            SizeTextSize = UIElementSettingsJson.TryGetValue("SizeTextSize", 0.5f);
            TypeXOffset = UIElementSettingsJson.TryGetValue("TypeXOffset", 0);
            TypeYOffset = UIElementSettingsJson.TryGetValue("TypeYOffset", 0);
            TypeTextSize = UIElementSettingsJson.TryGetValue("TypeTextSize", 0.5f);
            elementToggles = UIElementSettingsJson.TryGetValue("ElementToggles", new Dictionary<string, bool>());
            //RefreshUIState();
        }

        public void WriteAllInJson()
        {
            // Clear the settings before writing new ones
            UIElementSettingsJson.ClearSettings();

            // Writting variables to settings
            UIElementSettingsJson.WriteValue("Opacity", opacity);
            UIElementSettingsJson.WriteValue("ShowHitbox", DrawHitboxOfElement);
            UIElementSettingsJson.WriteValue("ShowSize", DrawSizeOfElement);
            UIElementSettingsJson.WriteValue("ShowType", DrawNameOfElement);
            UIElementSettingsJson.WriteValue("OutlineColor", outlineColor);
            UIElementSettingsJson.WriteValue("RandomOutlineColor", randomOutlineColor);
            UIElementSettingsJson.WriteValue("Thickness", thickness);
            UIElementSettingsJson.WriteValue("SizeXOffset", SizeXOffset);
            UIElementSettingsJson.WriteValue("SizeYOffset", SizeYOffset);
            UIElementSettingsJson.WriteValue("SizeTextSize", SizeTextSize);
            UIElementSettingsJson.WriteValue("TypeXOffset", TypeXOffset);
            UIElementSettingsJson.WriteValue("TypeYOffset", TypeYOffset);
            UIElementSettingsJson.WriteValue("TypeTextSize", TypeTextSize);
            UIElementSettingsJson.WriteValue("ElementToggles", elementToggles);

            // Save the settings to JSON
            UIElementSettingsJson.Save();
        }
        #endregion

        // Randomize offset
        public void RandomizeSizeOffset()
        {
            sizeOffsets.Clear();
            //foreach (var elem in elements)
            //{
            //    sizeOffsets[elem] = new Vector2(Random, Random);
            //}
        }
        public void RandomizeTypeOffset()
        {
            typeOffsets.Clear();
            //foreach (var elem in elements)
            //{
            //    typeOffsets[elem] = new Vector2(Random, Random);
            //}
        }

        public bool GetElement(string typeName, bool defaultValue)
        {
            return elementToggles.TryGetValue(typeName, out bool value) ? value : defaultValue;
        }

        public void SetElement(string typeName, bool value)
        {
            elementToggles[typeName] = value;
        }

        public void ToggleShowAll()
        {
            showAll = !showAll;
            SetShowAll(showAll);
        }

        public void SetShowAll(bool value)
        {
            showAll = value;
            if (showAll)
            {
                Main.NewText(Loc.Get("UIElementPanel.ShowAll"), new Color(57, 226, 39)); // green
                foreach (var keys in elementToggles.Keys)
                    elementToggles[keys] = true;
            }
            else
            {
                Main.NewText(Loc.Get("UIElementPanel.HideAll"), new Color(226, 57, 39));
                foreach (var keys in elementToggles.Keys)
                    elementToggles[keys] = false;
            }

            // Update text
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            UIElementPanel uiPanel = sys.mainState.uiElementPanel;

            foreach (var uiElement in uiPanel.dynamicOptions.Values)
            {
                if (uiElement is OptionElement o)
                {
                    o.SetValue(showAll);
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

        public void RefreshUIState()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys == null)
            {
                Log.Error("dang is null");
                return;
            }
            UIElementPanel uiPanel = sys.mainState.uiElementPanel;
            uiPanel?.Update(Main._drawInterfaceGameTime); // Force immediate update
        }

        public void UIElement_Draw(On_UIElement.orig_Draw orig, UIElement self, SpriteBatch spriteBatch)
        {
            orig(self, spriteBatch); // Normal UI behavior

            if (Main.dedServ || Main.gameMenu)
                return;
            if (self is MainState || self is UIElementState)
                return;
            if (self.GetOuterDimensions().Width > 900 || self.GetOuterDimensions().Height > 900)
                return;

            string typeName = self.GetType().Name;

            if (!elementToggles.ContainsKey(typeName))
            {
                elementToggles[typeName] = showAll;
            }

            if (elementToggles.TryGetValue(typeName, out bool value))
            {
                // Add the type name to the activeUIElementsNameList
                activeUIElementsNameList.Add(typeName);

                // Check if this *type* is toggled OFF
                if (!value)
                    return;
                //Main.NewText($"{GetModInstance(self).DisplayName}");
            }

            if (DrawSizeOfElement)
            {
                DrawElementSize(spriteBatch, self, self.GetOuterDimensions().Position().ToPoint());
            }

            if (DrawNameOfElement)
            {
                DrawElementType(spriteBatch, self, self.GetOuterDimensions().Position().ToPoint());
            }
            if (DrawHitboxOfElement)
            {
                // Draw the hitbox
                DrawHitbox(self, spriteBatch);
            }
        }
    }
}