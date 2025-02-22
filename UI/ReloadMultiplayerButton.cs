using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ReloadMultiplayerButton(Asset<Texture2D> _image, string hoverText) : BaseButton(_image, hoverText)
    {

        public async override void LeftClick(UIMouseEvent evt)
        {
            ReloadUtilities.PrepareClient(ClientMode.MPMain);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                await ReloadUtilities.ExitWorldOrServer();
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                await ReloadUtilities.ExitAndKillServer();
            }

            ReloadUtilities.ReloadMod();
        }
    }
}