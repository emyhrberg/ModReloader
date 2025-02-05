using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;
using Terraria;
using SquidTestingMod.src;

namespace SquidTestingMod.UI
{
    public class RefreshSystem : ModSystem
    {
        private UserInterface userInterface;
        private RefreshState myState;

        public override void Load()
        {
            if (!Main.dedServ) // ensure that this is only run on the client
            {
                var config = ModContent.GetInstance<Config>();
                if (config.EnableRefreshButton)
                {
                    userInterface = new UserInterface();
                    myState = new RefreshState();
                    userInterface.SetState(myState);
                }
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // check if toggle
            Config c = ModContent.GetInstance<Config>();
            if (c.EnableRefreshButton)
            {
                userInterface?.Update(gameTime);
            }
        }

        // boilerplate code to draw the UI
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "SquidTestingMod: MyState",
                    delegate
                    {
                        Config config = ModContent.GetInstance<Config>();
                        if (config.EnableRefreshButton)
                        {
                            userInterface?.Draw(Main.spriteBatch, new GameTime()); // actual draw
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
