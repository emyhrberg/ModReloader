using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Buttons;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ReloadMPButton(Asset<Texture2D> _image, string hoverText, bool animating) : BaseButton(_image, hoverText, animating)
    {
        // Set custom animation dimensions
        protected override Asset<Texture2D> Spritesheet => Assets.ButtonReloadMPSS;
        protected override float SpriteScale => 1.25f;
        protected override int MaxFrames => 5;
        protected override int FrameSpeed => 8;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;

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

            ReloadUtilities.BuildAndReloadMod();
        }
    }
}