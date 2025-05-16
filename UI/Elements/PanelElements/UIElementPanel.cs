using System.Collections.Generic;
using System.Linq;
using ModReloader.Common.Systems;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements
{
    public class UIElementPanel : BasePanel
    {
        private UIElementSystem elementSystem;
        private UIElementState elementState;

        public UIElementPanel() : base(header: "UI")
        {
            elementSystem = ModContent.GetInstance<UIElementSystem>();
            elementState = elementSystem.debugState;

            AddHeader("UIElement Hitboxes");
            AddPadding(5);
            AddOption("Show All", elementState.ToggleShowAll, "Show all UI elements from mods");

            AddSlider(
                title: "Opacity",
                min: 0,
                max: 1f,
                defaultValue: 0.1f,
                onValueChanged: elementState.SetOpacity,
                hover: "Set the opacity of the UI elements hitboxes",
                increment: 0.01f
            );

            AddSlider(
                title: "Color",
                min: 0,
                max: 1f,
                defaultValue: 0f,
                onValueChanged: (value) =>
                {
                    // Interpolate from Black (value=0f) to White (value=1f)
                    Color col = Color.Lerp(Color.White, Color.Black, value);
                    elementState.SetOutlineColor(col);
                },
                hover: "Set the outline color of the UI elements hitboxes",
                increment: 0.01f
            );

            AddSlider(
                title: "Thickness",
                min: 0,
                max: 10,
                defaultValue: 1f,
                onValueChanged: (value) => elementState.SetThickness((int)value),
                hover: "Set the thickness of the UI elements hitboxes",
                increment: 1f
            );

            // colors
            ActionOption rand = new(elementState.RandomizeRainbowColors, "Randomize Colors", "Randomize the colors of the boxes");
            uiList.Add(rand);
            AddPadding(3f);
            ActionOption randOut = new(elementState.ToggleRandomOutlineColor, "Randomize Outline", "Randomize the outline color of the boxes");
            uiList.Add(randOut);
            AddPadding(20);

            AddHeader("UIElement Size");
            AddOption("Show Size", elementState.ToggleShowSize, "Show the size of every element for active UIElements");

            AddSlider(
                title: "X Offset",
                min: -100,
                max: 100,
                defaultValue: 0,
                onValueChanged: elementState.SetSizeXOffset,
                hover: "Set the X offset of the type text",
                increment: 1
            );

            AddSlider(
                title: "Y Offset",
                min: -100,
                max: 100,
                defaultValue: 0,
                onValueChanged: elementState.SetSizeYOffset,
                hover: "Set the Y offset of the type text",
                increment: 1
            );

            AddSlider(
                title: "Text Size",
                min: 0.1f,
                max: 2f,
                defaultValue: 0.5f,
                onValueChanged: elementState.SetSizeTextSize,
                hover: "Set the text size of the size text",
                increment: 0.1f
            );
            AddPadding(3f);
            ActionOption randomizeSizeOffset = new(
                leftClick: elementState.RandomizeSizeOffset,
                text: "Randomize Offset",
                hover: "Randomize the offset of the size text"
            );
            uiList.Add(randomizeSizeOffset);

            ActionOption resetSizeOffset = new(
                leftClick: elementState.ResetSizeOffset,
                text: "Reset Offset",
                hover: "Reset the offset of the size text"
            );
            uiList.Add(resetSizeOffset);
            AddPadding(20);

            AddHeader("UIElement Types");
            AddOption("Show Type", elementState.ToggleShowType, "Show the type of every element for active UIElements");

            AddSlider(
                title: "X Offset",
                min: -100,
                max: 100,
                defaultValue: 0,
                onValueChanged: elementState.SetTypeXOffset,
                hover: "Set the X offset of the type text",
                increment: 1
            );

            AddSlider(
                title: "Y Offset",
                min: -100,
                max: 100,
                defaultValue: 0,
                onValueChanged: elementState.SetTypeYOffset,
                hover: "Set the Y offset of the type text",
                increment: 1
            );

            AddSlider(
                title: "Text Size",
                min: 0.1f,
                max: 2f,
                defaultValue: 0.5f,
                onValueChanged: elementState.SetTypeTextSize,
                hover: "Set the text size of the type text",
                increment: 0.1f
            );
            AddPadding(3f);

            ActionOption randomizeTypeOffset = new(
                leftClick: elementState.RandomizeTypeOffset,
                text: "Randomize Offset",
                hover: "Randomize the offset of the type text"
            );
            uiList.Add(randomizeTypeOffset);
            AddPadding(3f);

            ActionOption resetTypeOffset = new(
                leftClick: elementState.ResetTypeOffset,
                text: "Reset Offset",
                hover: "Reset the offset of the type text"
            );
            uiList.Add(resetTypeOffset);

            AddPadding(20);

            AddHeader($"All UIElements");
        }

        // Dynamic UI elements for each UIElement type
        private List<string> elements = new List<string>();
        public Dictionary<string, UIElement> dynamicOptions = new();

        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            base.Update(gameTime);

            // Only update once a second
            if (Main.GameUpdateCount % 60 != 0)
                return;

            // Gather distinct UIElement type names
            var distinctTypes = elementState.elements
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
                        leftClick: () => elementState.ToggleElement(typeName),
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