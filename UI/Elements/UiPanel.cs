using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class UiPanel : OptionPanel
    {
        private DebugSystem debugSystem;
        private DebugState debugState;

        public UiPanel() : base(title: "UI", scrollbarEnabled: true)
        {
            debugSystem = ModContent.GetInstance<DebugSystem>();
            debugState = debugSystem.debugState;

            AddPadding(5);
            AddOption("Show All", debugState.ToggleShowAll, "Show all UI elements from mods");
            AddOption("Show Text Size", debugState.ToggleShowSize, "Show all sizes for active UIElements");


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
            ActionOption rand = new(debugState.RandomizeRainbowColors, "Randomize Colors", "Randomize the colors of the boxes");
            uiList.Add(rand);
            AddPadding(3f);
            ActionOption randOut = new(debugState.ToggleRandomOutlineColor, "Randomize Outline", "Randomize the outline color of the boxes");
            uiList.Add(randOut);
            AddPadding(3f);

            SliderPanel opacity = new(
                title: "Opacity",
                min: 0,
                max: 0.69f,
                defaultValue: 0.1f,
                onValueChanged: debugState.SetOpacity,
                hover: "Set the opacity of the UI elements hitboxes",
                increment: 0.01f
            );
            SliderPanel outlineColor = new(
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
            SliderPanel thickness = new(
                title: "Thickness",
                min: 1,
                max: 10,
                defaultValue: 1f,
                onValueChanged: (value) => debugState.SetThickness((int)value),
                hover: "Set the opacity of the UI elements hitboxes",
                increment: 1f
            );
            uiList.Add(opacity);
            AddPadding(3f);
            uiList.Add(outlineColor);
            AddPadding(3f);
            uiList.Add(thickness);
            AddPadding();

            AddHeader($"All UIElements");
        }

        // Dynamic UI elements for each UIElement type
        private List<string> elements = new List<string>();
        public Dictionary<string, UIElement> dynamicOptions = new();

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Only update once a second
            if (Main.GameUpdateCount % 60 != 0)
                return;

            // Gather distinct UIElement type names
            var distinctTypes = debugState.elements
                .Select(ele => ele.GetType().Name)
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct()
                .OrderBy(name => name)
                .ToList();

            // 1. Remove old ones that no longer exist
            for (int i = elements.Count - 1; i >= 0; i--)
            {
                if (!distinctTypes.Contains(elements[i]))
                {
                    // Remove from the UI if we’re tracking it
                    if (dynamicOptions.TryGetValue(elements[i], out UIElement oldOption))
                    {
                        uiList.Remove(oldOption);
                        dynamicOptions.Remove(elements[i]);
                    }
                    elements.RemoveAt(i);
                }
            }

            // 2. Add new ones
            foreach (var typeName in distinctTypes)
            {
                if (!elements.Contains(typeName))
                {
                    elements.Add(typeName);

                    // Create the UI option
                    var newOption = AddOption(
                        text: typeName,
                        leftClick: () => debugState.ToggleElement(typeName),
                        hover: $"Show all {typeName} UIElements",
                        padding: 0f
                    );

                    dynamicOptions[typeName] = newOption;
                }
            }

            // 3. Sort everything in alphabetical order by typeName
            elements.Sort();

            // 4. Remove existing “dynamic” UI elements from uiList
            foreach (var pair in dynamicOptions)
            {
                uiList.Remove(pair.Value);
            }

            // 5. Re-add them in sorted order
            foreach (var typeName in elements)
            {
                if (dynamicOptions.TryGetValue(typeName, out UIElement elem))
                    uiList.Add(elem);
            }
        }
    }
}
