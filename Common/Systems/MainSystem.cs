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
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "ModReloader: MainSystem UI",
                    () => { userInterface?.Draw(Main.spriteBatch, new GameTime()); return true; },
                    InterfaceScaleType.UI
                ));
            }
        }
    }
}