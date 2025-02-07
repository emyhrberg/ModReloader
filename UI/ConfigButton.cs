using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using Terraria.ModLoader;

namespace SquidTestingMod.UI
{
    public class ConfigButton(Asset<Texture2D> texture, string hoverText) : BaseButton(texture, hoverText)
    {
        public override void HandleClick()
        {
            Config c = ModContent.GetInstance<Config>();
            c.Open();
        }
    }
}