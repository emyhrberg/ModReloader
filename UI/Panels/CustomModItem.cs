using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace SquidTestingMod.UI.Panels
{
    // A simplified UI panel that mimics the UIModItem appearance.
    public class ModItem : UIPanel
    {
        private string hover;

        // Constructor
        public ModItem(string modName, Texture2D ModIcon, string hover) : base()
        {
            // Set hover text
            this.hover = hover;

            // Set overall panel dimensions and style.
            Width.Set(-35f, 1f);
            Height.Set(80, 0);
            Left.Set(5, 0);
            SetPadding(6f);
            BorderColor = new Color(89, 116, 213) * 0.7f;
            BackgroundColor = new Color(50, 50, 50) * 0.8f;

            // --- Mod Icon ---
            UIImage ModImage = new(ModIcon)
            {
                Left = { Pixels = 0 },
                Top = { Pixels = 0 },
                Width = { Pixels = 80 },
                Height = { Pixels = 80 },
                ScaleToFit = true
            };
            Append(ModImage);

            // --- Mod Name & Version ---
            // Display a text element for the mod name and version.
            // Check if the mod name is too long and truncate it if necessary.
            string modNameTruncated = modName.Length > 30 ? modName.Substring(0, 30) + "..." : modName;

            UIText ModTitle = new UIText(modNameTruncated)
            {
                Left = { Pixels = 90 },
                Top = { Pixels = 5 }
            };
            Append(ModTitle);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering)
                UICommon.TooltipMouseText(hover);
        }
    }
}
