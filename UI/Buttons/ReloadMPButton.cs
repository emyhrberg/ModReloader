using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Buttons;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ReloadMPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : ReloadButton(spritesheet, buttonText, hoverText)
    {
        // Set custom animation dimensions
        protected override float SpriteScale => 1.25f;
        protected override int MaxFrames => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;

        public async override void LeftClick(UIMouseEvent evt)
        {
            ReloadUtilities.PrepareClient(ClientMode.MPMain);

            // we must be in multiplayer for this to have an effect
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                await ReloadUtilities.ExitAndKillServer();
                ReloadUtilities.BuildAndReloadMod();
            }
        }
    }
}