using System.Collections.Generic;
using Terraria.UI;

namespace ModReloader.Common.Systems
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
            //ChatPosHook.OffsetX = -20;
            //ChatPosHook.OffsetY = -15;
        }

        // boilerplate code to draw the UI
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // Instead of drawing before "Vanilla: Mouse Text", draw just before the Fancy UI layer:
            int fancyIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Fancy UI"));
            if (fancyIndex != -1)
            {
                layers.Insert(fancyIndex, new LegacyGameInterfaceLayer(
                    "ModReloader: MainSystem (before Fancy UI)",
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
                        "ModReloader: MainSystem",
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
