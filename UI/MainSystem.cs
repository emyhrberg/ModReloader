using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    [Autoload(Side = ModSide.Client)]
    public class MainSystem : ModSystem
    {
        public UserInterface userInterface;
        public MainState myState;

        public override void Load()
        {
            userInterface = new UserInterface();
            myState = new MainState();
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
                    "SquidTestingMod: User Interface Draw MainSystem",
                    () =>
                    {
                        userInterface?.Draw(Main.spriteBatch, new GameTime()); // actual draw
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        public void SetUIStateToMyState()
        {
            userInterface.SetState(myState);
        }

        public void SetUIStateToNull()
        {
            userInterface.SetState(null);
        }
    }
}
