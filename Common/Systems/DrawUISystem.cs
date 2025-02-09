using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class UIElementDebugger : ModSystem
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
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "SquidTestingMod: DebugUI",
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

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            bool isUIDebugDrawing = sys.mainState.uiDebugButton.IsUIDebugDrawing;

            if (!Main.dedServ && !Main.gameMenu && isUIDebugDrawing)
            {
                if (self is not MainState && self is not DrawUIState) // Skip full-screen UI elements
                {
                    drawUIState.DrawHitbox(self, spriteBatch);
                }
            }
        }
    }
}
