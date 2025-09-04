﻿using Microsoft.Xna.Framework.Graphics;
using ModReloader.UI.Shared;
using ReLogic.Content;
using Terraria.UI;

namespace ModReloader.Core.Features.LogFeatures
{
    public class LogButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set custom animation dimensions
        protected override float BaseAnimScale => 0.5f;
        protected override int FrameCount => 16;
        protected override int FrameSpeed => 4;
        protected override int FrameWidth => 74;
        protected override int FrameHeight => 78;

        public override void RightClick(UIMouseEvent evt)
        {
            if (!Conf.C.RightClickToolOptions) return;
            Log.OpenClientLog();
        }
    }
}