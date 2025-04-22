using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ModHelper.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI
{
    [Autoload(Side = ModSide.Client)]
    public class MainSystem : ModSystem
    {
        public UserInterface userInterface;
        public MainState mainState;

        public override void OnWorldLoad()
        {
            userInterface = new UserInterface();
            mainState = new MainState();
            userInterface.SetState(mainState);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            userInterface?.Update(gameTime);
        }

        // boilerplate code to draw the UI
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // Instead of drawing before "Vanilla: Mouse Text", draw just before the Fancy UI layer:
            int fancyIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Fancy UI"));
            if (fancyIndex != -1)
            {
                layers.Insert(fancyIndex, new LegacyGameInterfaceLayer(
                    "ModHelper: MainSystem (before Fancy UI)",
                    () =>
                    {
                        userInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
            else
            {
                // Fallback: if Fancy UI isn't found, insert before Mouse Text
                int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
                if (mouseTextIndex != -1)
                {
                    layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                        "ModHelper: MainSystem",
                        () =>
                        {
                            userInterface?.Draw(Main.spriteBatch, new GameTime());
                            return true;
                        },
                        InterfaceScaleType.UI
                    ));
                }
            }
        }
    }
}
