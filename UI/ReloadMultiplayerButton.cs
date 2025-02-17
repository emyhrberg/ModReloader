using System;
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
    public class ReloadMultiplayerButton : BaseButton
    {
        public ReloadMultiplayerButton(Asset<Texture2D> _image, string hoverText) : base(_image, hoverText)
        {
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                WorldGen.JustQuit();
                //TODO: add server 
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModNetHandler.RefreshServer.SendKillingServer(255, Main.myPlayer);
            }
        }
    }
}