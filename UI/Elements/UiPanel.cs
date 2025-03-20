using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Systems;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.UI.Elements
{
    public class UiPanel : OptionPanel
    {
        private DebugSystem debugSystem;
        private DebugState debugState;

        private List<OnOffOption> allElements = [];

        public UiPanel() : base(title: "UI", scrollbarEnabled: true)
        {
            debugSystem = ModContent.GetInstance<DebugSystem>();
            debugState = debugSystem.debugState;

            AddPadding(5);
            AddHeader("UIElement Hitboxes");
            OnOffOption showAll = new(debugState.ToggleShowAll, "Show All Off", "Show all UI elements from mods");
            uiList.Add(showAll);
            OnOffOption showSize = new(debugState.ToggleShowSize, "Show Text Size Off", "Show all sizes for active UIElements");
            uiList.Add(showSize);

            // SliderOption xOffset = new(
            //     title: "X Offset",
            //     min: -20,
            //     max: 20,
            //     defaultValue: 0,
            //     onValueChanged: debugState.SetXOffset,
            //     hover: "Adjust the X offset of the text size",
            //     increment: 1
            // );
            // SliderOption yOffset = new(
            //     title: "Y Offset",
            //     min: -20,
            //     max: 20,
            //     defaultValue: 0,
            //     onValueChanged: debugState.SetYOffset,
            //     hover: "Adjust the Y offset of the text size",
            //     increment: 1
            // );
            // SliderOption textSize = new(
            //     title: "Text Size",
            //     min: 0.1f,
            //     max: 1f,
            //     defaultValue: 0.5f,
            //     onValueChanged: debugState.SetTextSize,
            //     hover: "Adjust the size of the text",
            //     increment: 0.1f
            // );

            // uiList.Add(textSize);
            // uiList.Add(xOffset);
            // uiList.Add(yOffset);

            // colors
            OnOffOption randomizeColors = new(debugState.RandomizeRainbowColors, "Randomize Colors", "Randomize the colors of the boxes");
            uiList.Add(randomizeColors);
            OnOffOption randomOutline = new(debugState.ToggleRandomOutlineColor, "Random Outline Off", "Randomize the outline color of the boxes");
            uiList.Add(randomOutline);

            SliderOption opacity = new(
                title: "Opacity",
                min: 0,
                max: 0.69f,
                defaultValue: 0.1f,
                onValueChanged: debugState.SetOpacity,
                hover: "Set the opacity of the UI elements hitboxes",
                increment: 0.01f
            );
            SliderOption outlineColor = new(
                title: "Color",
                min: 0,
                max: 1f,
                defaultValue: 0f,
                onValueChanged: (value) =>
                {
                    // Interpolate from Black (value=0f) to White (value=1f)
                    Color col = Color.Lerp(Color.White, Color.Black, value);
                    debugState.SetOutlineColor(col);
                },
                hover: "Set the opacity of the UI elements hitboxes",
                increment: 0.01f
            );
            SliderOption thickness = new(
                title: "Thickness",
                min: 1,
                max: 10,
                defaultValue: 1f,
                onValueChanged: (value) => debugState.SetThickness((int)value),
                hover: "Set the opacity of the UI elements hitboxes",
                increment: 1f
            );
            uiList.Add(opacity);
            uiList.Add(outlineColor);
            uiList.Add(thickness);
            AddPadding();

            AddHeader($"All UIElements");
        }

        public void UpdateText()
        {
            foreach (var toggle in allElements)
            {
                toggle.textElement.SetText($"{toggle.textElement.Text.Split(' ')[0]} {(toggle.textElement.Text.Contains("On") ? "Off" : "On")}");
            }
        }

        // add a field to track which types we added toggles for:
        private HashSet<string> addedTypes = new();

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Main.GameUpdateCount % 60 == 0) // Every 60 frames, check for new elements
            {
                var distinctTypes = debugState.elements
                    .Select(ele => ele.GetType().Name)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList();

                // Temporary list to store new toggles (to avoid modifying uiList while iterating)
                List<OnOffOption> newToggles = new();

                foreach (string typeName in distinctTypes)
                {
                    if (!addedTypes.Contains(typeName))
                    {
                        addedTypes.Add(typeName);

                        OnOffOption typeToggle = new(
                            leftClick: () => debugState.ToggleAllUIElementsOfType(typeName),
                            text: $"{typeName} Off",
                            tooltip: $"Click to toggle all {typeName} hitboxes\nRight click to print dimensions",
                            rightClick: () => debugState.PrintDimensionsForType(typeName)
                        );

                        newToggles.Add(typeToggle); // Add to temp list instead of modifying uiList directly
                    }
                }

                // Add new toggles *after* the iteration is finished
                foreach (var toggle in newToggles)
                {
                    uiList.Add(toggle);
                    allElements.Add(toggle);
                }
            }
        }
    }
}
