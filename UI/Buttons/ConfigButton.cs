using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ConfigButton(Asset<Texture2D> image, string hoverText, bool animating) : BaseButton(image, hoverText, animating)
    {
        public override void LeftClick(UIMouseEvent evt)
        {
            Conf.Instance.Open();
        }
    }
}