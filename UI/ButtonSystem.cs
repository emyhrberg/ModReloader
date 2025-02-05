using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;
using Terraria;

namespace SquidTestingMod.UI
{
    [Autoload(Side = ModSide.Client)]
    public class ButtonSystem : ModSystem
    {
        private UserInterface userInterface;
        private ButtonState myState;

        public override void Load()
        {
            userInterface = new UserInterface();
            myState = new ButtonState();
            userInterface.SetState(myState);
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
                    "SquidTestingMod: MyState",
                    () =>
                    {
                        userInterface?.Draw(Main.spriteBatch, new GameTime()); // actual draw
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
