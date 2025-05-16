using System.Collections.Generic;
using Terraria.UI;

namespace ModReloader.Common.Systems
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
                    name: "ModReloader: UIElementSystem",

                    drawMethod: delegate
                    {
                        ui?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },

                    scaleType: InterfaceScaleType.UI));
            }
        }
    }
}