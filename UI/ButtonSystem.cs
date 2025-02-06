using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

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
                        // TESTING_DRAW_TEMPORARY();
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        // private void TESTING_DRAW_TEMPORARY()
        // {
        //     for (int i = 1; i < 30; i++)
        //     {
        //         Main.instance.LoadItem(i);
        //         var texture = TextureAssets.Item[i].Value;
        //         Main.spriteBatch.Draw(texture, new Vector2(10 + 30 * i, 100), Color.White);
        //     }
        //     for (int i = 30; i < 60; i++)
        //     {
        //         Main.instance.LoadItem(i);
        //         var texture = TextureAssets.Item[i].Value;
        //         Main.spriteBatch.Draw(texture, new Vector2(10 + 30 * i, 150), Color.White);
        //     }
        // }
    }
}
