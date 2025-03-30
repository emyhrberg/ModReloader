using System.Collections.Generic;
using Microsoft.Xna.Framework;
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
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "ModHelper: MainSystem",
                    () =>
                    {
                        // actual drawing of entire user interface
                        // dont mess with this
                        userInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
