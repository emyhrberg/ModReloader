using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ReloadSingleplayerButton(Asset<Texture2D> _image, string hoverText) : BaseButton(_image, hoverText)
    {
        public async override void LeftClick(UIMouseEvent evt)
        {

            // 1 Clear logs if needed
            if (Conf.ClearClientLogOnReload)
                Utilities.ClearClientLog();

            // 2 Prepare client data
            ReloadUtilities.PrepareClient(ClientMode.SinglePlayer);

            // 3 Exit server or world
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                await ReloadUtilities.ExitWorldOrServer();
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                await ReloadUtilities.ExitAndKillServer();
            }

            // 3 Reload
            await ReloadUtilities.BuildAndReloadMod();
        }

        
    }
}