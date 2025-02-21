using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.PacketHandlers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
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
                ReloadUtilities.ExitWorldOrServer();
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ReloadUtilities.ExitAndKillServer();
            }

            await ReloadUtilities.ReloadOrBuildAndReloadAsync(true);
        }
    }
}