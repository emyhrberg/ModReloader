using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.Common.Systems
{
    public class DrawUISystem : ModSystem
    {
        private UserInterface ui;
        private DrawUIState drawUIState;

        private List<UIElement> elementsLogged = new();

        // Flag to enable/disable UI debug drawing
        private bool isUIDebugDrawing = false;

        public bool GetDebugDrawing() => isUIDebugDrawing;
        public void ToggleUIDebugDrawing()
        {
            isUIDebugDrawing = !isUIDebugDrawing;

            if (isUIDebugDrawing)
            {
                Main.NewText("UIElements: (Type), Width x Height", Color.Green);
            }

            if (C.ShowCombatTextOnToggle)
                CombatText.NewText(Main.LocalPlayer.getRect(), isUIDebugDrawing ? Color.Green : Color.Red, isUIDebugDrawing ? "UI Debug ON" : "UI Debug OFF");
        }

        public override void Load()
        {
            ui = new UserInterface();
            drawUIState = new DrawUIState();
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
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        private void UIElement_Draw(On_UIElement.orig_Draw orig, UIElement self, SpriteBatch spriteBatch)
        {
            orig(self, spriteBatch); // Keep normal UI behavior

            // ensure we are drawing the UI debug hitboxes
            if (!isUIDebugDrawing)
                return;

            // ensure we are not in a dedicated server and not in the main menu
            if (Main.dedServ || Main.gameMenu)
                return;

            // ensure we are not drawing the hitbox of a full-screen UI element
            if (self is MainState || self is DrawUIState)
                return;

            // ensure the UIelement is not bigger than 600, either by width or height
            if (self.GetOuterDimensions().Width > 600 || self.GetOuterDimensions().Height > 600)
                return;

            // OK, all checks passed, now draw the hitbox for the UI element
            drawUIState.DrawHitbox(self, spriteBatch);

            // Also, log the element like this: Name: "UIElement", Inner: 100x100, Outer: 100x100
            // But only log it once, so track which elements we already logged
            if (elementsLogged.Contains(self))
                return;

            elementsLogged.Add(self);

            // Log text
            string elementText = $"{elementsLogged.Count}. ({self.GetType().Name})";
            // Check if inner width equals outer width and inner height equals outer height, if so, only log one dimension
            if (self.GetInnerDimensions().Width == self.GetOuterDimensions().Width && self.GetInnerDimensions().Height == self.GetOuterDimensions().Height)
            {
                elementText += $", {self.GetInnerDimensions().Width}x{self.GetInnerDimensions().Height}";
            }
            else
            {
                elementText += $", Inner: {self.GetInnerDimensions().Width}x{self.GetInnerDimensions().Height}, Outer: {self.GetOuterDimensions().Width}x{self.GetOuterDimensions().Height}";
            }

            Main.NewText(elementText, Color.White);
        }
    }
}
