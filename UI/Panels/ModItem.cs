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
        private string Hover;
        public bool IsSetToReload { get; set; }
        public string ModName { get; set; }

        private Color notSelected = new(89, 116, 213);
        private Color selected = new(36, 47, 110);

        // Constructor
        public ModItem(bool isSetToReload, string modName, Texture2D ModIcon, string hover) : base()
        {
            // Set hover text
            this.IsSetToReload = isSetToReload;
            this.Hover = hover;
            this.ModName = modName;

            // Set overall panel dimensions and style.
            Width.Set(-35f, 1f);
            Height.Set(30, 0);
            Left.Set(5, 0);
            SetPadding(6f);
            BorderColor = notSelected;
            BackgroundColor = notSelected * 0.7f;

            if (IsSetToReload)
            {
                BackgroundColor = selected;
                BorderColor = selected;
            }

            // --- Mod Icon ---
            UIImage ModImage = new(ModIcon)
            {
                Left = { Pixels = 0 },
                Top = { Pixels = 0 },
                Width = { Pixels = 30 },
                Height = { Pixels = 30 },
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

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                BackgroundColor = this.selected;
                BorderColor = this.selected;
            }
            else
            {
                BackgroundColor = notSelected;
                BorderColor = notSelected;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering)
                UICommon.TooltipMouseText(Hover);
        }
    }
}
