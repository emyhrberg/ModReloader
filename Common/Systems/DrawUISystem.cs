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
    public class DrawUISystem : ModSystem
    {
        private UserInterface ui;
        private DrawUIState drawUIState;

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

            // Get MainSystem safely
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys == null || sys.mainState?.uiDebugButton == null)
                return;

            // ensure we are drawing the UI debug hitboxes
            bool isUIDebugDrawing = sys.mainState.uiDebugButton.IsUIDebugDrawing;
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
        }
    }
}
