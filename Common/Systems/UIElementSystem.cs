using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    public class UIElementSystem : ModSystem
    {
        // State
        private UserInterface ui;
        public UIElementState debugState;

        public override void Load()
        {
            ui = new UserInterface();
            debugState = new UIElementState();
            ui.SetState(debugState);

            On_UIElement.Draw += debugState.UIElement_Draw;
        }

        public override void Unload()
        {
            On_UIElement.Draw -= debugState.UIElement_Draw;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "SquidTestingMod: DrawUISystem",
                    () =>
                    {
                        ui?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}