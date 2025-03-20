using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    public class DebugSystem : ModSystem
    {
        private UserInterface ui;
        private DebugState drawUIState;

        private List<UIElement> elements = new();

        // Flag to enable/disable UI debug drawing
        public bool isUIDebugDrawing = false;
        public bool isUIDebugSizeElementDrawing = false;

        public void ToggleUIDebugSizeElementDrawing()
        {
            isUIDebugSizeElementDrawing = !isUIDebugSizeElementDrawing;

            CombatText.NewText(Main.LocalPlayer.getRect(), isUIDebugSizeElementDrawing ? Color.Green : Color.Red, isUIDebugSizeElementDrawing ? "UI Size Text ON" : "UI Size Text OFF");
        }

        public void ToggleUIDebugDrawing()
        {
            isUIDebugDrawing = !isUIDebugDrawing;

            CombatText.NewText(Main.LocalPlayer.getRect(), isUIDebugDrawing ? Color.Green : Color.Red, isUIDebugDrawing ? "UI Debug ON" : "UI Debug OFF");
        }

        public override void Load()
        {
            ui = new UserInterface();
            drawUIState = new DebugState();
            ui.SetState(drawUIState);

            On_UIElement.Draw += UIElement_Draw;
        }

        public override void Unload()
        {
            On_UIElement.Draw -= UIElement_Draw;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "SquidTestingMod: DrawUISystem",
                    () =>
                    {
                        ui?.Draw(Main.spriteBatch, new GameTime());
                        Log.SlowInfo("Drawing UI State: " + drawUIState.GetType().Name);
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        private void UIElement_Draw(On_UIElement.orig_Draw orig, UIElement self, SpriteBatch spriteBatch)
        {
            Log.Info("DrawElement: " + self.GetType().Name);

            orig(self, spriteBatch); // Keep normal UI behavior

            // Also, log the element like this: Name: "UIElement", Inner: 100x100, Outer: 100x100
            // But only log it once, so track which elements we already logged
            if (!elements.Contains(self))
                elements.Add(self);

            // ensure we are not in a dedicated server and not in the main menu
            if (Main.dedServ || Main.gameMenu)
                return;

            // ensure we are not drawing the hitbox of a full-screen UI element
            if (self is MainState || self is DebugState)
                return;

            // ensure the UIelement is not bigger than 600, either by width or height
            if (self.GetOuterDimensions().Width > 900 || self.GetOuterDimensions().Height > 900)
                return;

            if (isUIDebugSizeElementDrawing)
            {
                drawUIState.DrawElementLabel(spriteBatch, self, self.GetOuterDimensions().Position().ToPoint());
            }

            // OK, all checks passed, now draw the hitbox for the UI element
            if (isUIDebugDrawing)
            {
                drawUIState.DrawHitbox(self, spriteBatch);
            }
        }

        public void PrintAllUIElements()
        {
            Main.NewText($"UIElements: (Name), Width x Height [total UIElements: {elements.Count}]", Color.Green);

            // Track which names have already been shown
            HashSet<string> shownNames = new();

            // Sort elements by name
            elements.Sort((a, b) => string.Compare(a.GetType().Name, b.GetType().Name, StringComparison.Ordinal));

            int count = 1;
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                string elementName = element.GetType().Name;

                if (!shownNames.Add(elementName))
                    continue; // Skip if we've already shown this name

                string elementText = $"{count}. ({elementName})";
                if (element.GetInnerDimensions().Width == element.GetOuterDimensions().Width
                    && element.GetInnerDimensions().Height == element.GetOuterDimensions().Height)
                {
                    elementText += $", {element.GetInnerDimensions().Width}x{element.GetInnerDimensions().Height}";
                }
                else
                {
                    elementText += $", Inner: {element.GetInnerDimensions().Width}x{element.GetInnerDimensions().Height}, Outer: {element.GetOuterDimensions().Width}x{element.GetOuterDimensions().Height}";
                }

                Main.NewText(elementText, Color.White);
                count++;
            }
        }
    }
}
