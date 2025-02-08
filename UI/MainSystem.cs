using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    [Autoload(Side = ModSide.Client)]
    public class MainSystem : ModSystem
    {
        public UserInterface userInterface;
        public MainState mainState;

        public override void Load()
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
                    "SquidTestingMod: MainSystem",
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
            userInterface.SetState(mainState);
        }

        public void SetUIStateToNull()
        {
            userInterface.SetState(null);
        }
    }
}
