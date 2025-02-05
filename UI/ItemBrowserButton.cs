using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ItemBrowserButton : BaseButton
    {
        public ItemBrowserButton(Asset<Texture2D> texture, string hoverText)
            : base(texture, hoverText)
        {
        }

        public void HandleClick(UIMouseEvent evt, UIElement listeningElement)
        {
            ModContent.GetInstance<SquidTestingMod>().Logger.Info("Item Browser button clicked.");
        }
    }
}
