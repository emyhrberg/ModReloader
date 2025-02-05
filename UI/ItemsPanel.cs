using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ItemsPanel : UIPanel
    {
        public override void OnInitialize()
        {
            Width.Set(100f, 0f);
            Height.Set(100f, 0f);
            HAlign = 0.4f;
            VAlign = 0.02f;
            Activate();
        }
    }
}
