using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using ModReloader.Common.Systems;
using Terraria.GameContent.UI.Elements;
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
            //TODO: This option is special bc it should be enabled if all elements are enabled and disabled if even one of the elements are disabled
            //So idk how to make it work rn, maybe we just with each element that tougles on or off we just update this option

            AddOption("Show Hitboxes", elementState.GetDrawHitboxOfElement(), elementState.SetDrawHitboxOfElement, "Show all UI elements from mods");


            AddSlider(
                title: "Opacity",
                min: 0,
                max: 1f,
                defaultValue: elementState.GetOpacity(),
                onValueChanged: elementState.SetOpacity,
                hover: "Set the opacity of the UI elements hitboxes",
                increment: 0.01f
            );

            AddSlider(
                title: "Color",
                min: 0,
                max: 1f,
                defaultValue: 1f - (elementState.GetOutlineColor().R / 255f),
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
                defaultValue: elementState.GetThickness(),
                onValueChanged: (value) => elementState.SetThickness((int)value),
                hover: "Set the thickness of the UI elements hitboxes",
                increment: 1f
            );

            // colors
            // THIS IS SHOULD BE AddOption ELEMENTS BC THEY ACT LIKE THEM 
            ActionOption rand = new(elementState.RandomizeRainbowColors, "Randomize Colors", "Randomize the colors of the boxes");
            uiList.Add(rand);
            AddPadding(3f);
            ActionOption randOut = new(elementState.ToggleRandomOutlineColor, "Randomize Outline", "Randomize the outline color of the boxes");
            uiList.Add(randOut);
            AddPadding(20);

            AddHeader("UIElement Size");
            AddOption("Show Size", elementState.GetDrawSizeOfElement(), elementState.SetDrawSizeOfElement, "Show the size of every element for active UIElements");

            AddSlider(
                title: "X Offset",
                min: -100,
                max: 100,
                defaultValue: elementState.GetSizeXOffset(),
                onValueChanged: elementState.SetSizeXOffset,
                hover: "Set the X offset of the type text",
                increment: 1
            );

            AddSlider(
                title: "Y Offset",
                min: -100,
                max: 100,
                defaultValue: elementState.GetSizeYOffset(),
                onValueChanged: elementState.SetSizeYOffset,
                hover: "Set the Y offset of the type text",
                increment: 1
            );

            AddSlider(
                title: "Text Size",
                min: 0.1f,
                max: 2f,
                defaultValue: elementState.GetSizeTextSize(),
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
            AddOption("Show Type", elementState.GetDrawNameOfElement(), elementState.SetDrawNameOfElement, "Show the type of every element for active UIElements");

            AddSlider(
                title: "X Offset",
                min: -100,
                max: 100,
                defaultValue: elementState.GetTypeXOffset(),
                onValueChanged: elementState.SetTypeXOffset,
                hover: "Set the X offset of the type text",
                increment: 1
            );

            AddSlider(
                title: "Y Offset",
                min: -100,
                max: 100,
                defaultValue: elementState.GetTypeYOffset(),
                onValueChanged: elementState.SetTypeYOffset,
                hover: "Set the Y offset of the type text",
                increment: 1
            );

            AddSlider(
                title: "Text Size",
                min: 0.1f,
                max: 2f,
                defaultValue: elementState.GetTypeTextSize(),
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
            AddOption("Toggle All", true, elementState.SetShowAll, "Toggle all UI elements from mods");
        }

        // Dynamic UI elements for each UIElement type
        public Dictionary<string, UIElement> dynamicOptions = new();

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Only update once a second
            if (Main.GameUpdateCount % 60 != 0)
                return;

            // Remove the old dynamic options
            foreach (var pair in dynamicOptions)
            {
                uiList.Remove(pair.Value);
            }

            foreach (var uiElementName in elementState.deathList)
            {
                elementState.elementToggles.Remove(uiElementName);
            }

            // Remove any dynamic options that are no longer in the elementToggles
            foreach (var key in dynamicOptions.Keys.ToList())
            {
                if (!elementState.elementToggles.ContainsKey(key))
                {
                    dynamicOptions.Remove(key);
                }
            }

            // Add new dynamic options for each element in the elementToggles
            foreach (var pair in elementState.elementToggles)
            {
                if (!dynamicOptions.ContainsKey(pair.Key))
                {
                    var newOption = AddOption(
                        text: pair.Key,
                        defaultValue: elementState.GetElement(pair.Key, true),
                        leftClick: (bool value) => elementState.SetElement(pair.Key, value),
                        hover: $"Show all {pair.Key} UIElements",
                        padding: 0f,
                        autoAdding: false
                    );
                    dynamicOptions[pair.Key] = newOption;
                }
            }

            // Sort the dynamic options by key
            var dynamicOptionsKeys = dynamicOptions.Keys.ToList();
            dynamicOptionsKeys.Sort();

            // Add the dynamic options to the UI
            foreach (var key in dynamicOptionsKeys)
            {
                if (dynamicOptions.TryGetValue(key, out UIElement elem))
                {
                    uiList.Add(elem);
                }
            }

            // Reset deathList
            elementState.deathList = dynamicOptionsKeys;
        }
    }
}