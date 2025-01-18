using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;
using Terraria;

namespace SkipSelect.MainCode
{
    public class MySystem : ModSystem
    {
        private UserInterface userInterface;
        private MyState myState;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                userInterface = new UserInterface();
                myState = new MyState();
                userInterface.SetState(myState);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            userInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "SkipSelect: MyState",
                    delegate
                    {
                        userInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
