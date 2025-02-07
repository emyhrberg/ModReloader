using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using Terraria.ModLoader;

namespace SquidTestingMod.UI
{
    public class ConfigButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : BaseButton(buttonImgText, buttonImgNoText, hoverText)
    {
        public bool IsConfigOpen = false;

        public override void HandleClick()
        {
            Config c = ModContent.GetInstance<Config>();
            c.Open();

            // set to ConfigUIState open
            IsConfigOpen = true;
        }
    }
}