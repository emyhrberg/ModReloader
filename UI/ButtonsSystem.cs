using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    [Autoload(Side = ModSide.Client)]
    public class ButtonsSystem : ModSystem
    {
        public UserInterface userInterface;
        public ButtonsState myState;

        public override void Load()
        {
            userInterface = new UserInterface();
            myState = new ButtonsState();
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

        public void ShowUI()
        {
            userInterface.SetState(myState);
        }

        public void HideUI()
        {
            userInterface.SetState(null);
        }
    }
}
