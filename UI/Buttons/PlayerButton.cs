using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Players;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class PlayerButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set custom animation dimensions
        private float _scale = 0.9f;
        protected override float Scale => _scale;
        protected override int StartFrame => 3;
        protected override int FrameCount => 17;
        protected override int FrameSpeed => 5;
        protected override int FrameWidth => 44;
        protected override int FrameHeight => 54;

        public override void RightClick(UIMouseEvent evt)
        {
            //base.RightClick(evt);

            // Toggle super mode
            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            p.ToggleSuperMode();
        }
    }
}