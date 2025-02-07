using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using Terraria.ModLoader;

namespace SquidTestingMod.UI
{
    public class NPCButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : BaseButton(buttonImgText, buttonImgNoText, hoverText)
    {
        public override void HandleClick()
        {
        }
    }
}