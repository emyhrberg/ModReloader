using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Buttons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ReloadMPButton : BaseButton
    {
        // Constructor
        public ReloadMPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : base(spritesheet, buttonText, hoverText)
        {
            // deactived by default since the SP button is active
            Active = false;
            buttonUIText.Active = false;
        }

        // Set custom animation dimensions
        protected override float SpriteScale => 1.25f;
        protected override int MaxFrames => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;

        public async override void LeftClick(UIMouseEvent evt)
        {
            // If alt+click, toggle the mode and return
            // bool altClick = Main.keyState.IsKeyDown(Keys.LeftAlt);
            // if (altClick)
            // {
            //     Active = false;
            //     buttonUIText.Active = false;

            //     // set MP active
            //     MainSystem sys = ModContent.GetInstance<MainSystem>();
            //     foreach (var btn in sys?.mainState?.AllButtons)
            //     {
            //         if (btn is ReloadSPButton spBtn)
            //         {
            //             spBtn.Active = true;
            //             spBtn.buttonUIText.Active = true;
            //         }
            //     }
            //     return;
            // }

            // ReloadUtilities.PrepareClient(ClientMode.MPMain);

            // // we must be in multiplayer for this to have an effect
            // if (Main.netMode == NetmodeID.MultiplayerClient)
            // {
            //     await ReloadUtilities.ExitAndKillServer();
            //     ReloadUtilities.BuildAndReloadMod();
            // }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            // If right click, toggle the mode and return
            Active = false;
            buttonUIText.Active = false;

            // set MP active
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            foreach (var btn in sys?.mainState?.AllButtons)
            {
                if (btn is ReloadSPButton spBtn)
                {
                    spBtn.Active = true;
                    spBtn.buttonUIText.Active = true;
                }
            }
            return;
        }
    }
}