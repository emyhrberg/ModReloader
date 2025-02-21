using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ConfigButton(Asset<Texture2D> _image, string hoverText) : BaseButton(_image, hoverText)
    {
        public override void LeftClick(UIMouseEvent evt)
        {
            Conf.Instance.Open();
        }
    }
}