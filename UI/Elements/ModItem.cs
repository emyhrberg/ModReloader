using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.UI;

namespace SquidTestingMod.UI.Elements
{
    // A simplified UI panel that mimics the UIModItem appearance.
    public class ModItem : UIPanel
    {
        private string Hover;
        public bool IsSetToReload { get; set; }
        public string ModName { get; set; }
        public CustomModTitle ModTitle { get; set; }

        public enum ModItemState
        {
            Default,
            Selected,
            Unselected,
            Disabled
        }

        public ModItemState state = ModItemState.Default;

        private Color defaultColor = new(89, 116, 213);
        private Color selected = new(36, 47, 110);
        private Color disabled = new(20, 20, 20, 255);

        // Constructor
        public ModItem(bool isSetToReload, string modName, Texture2D ModIcon, string hover) : base()
        {
            // Set hover text
            IsSetToReload = isSetToReload;
            Hover = hover;
            ModName = modName;

            // Set overall panel dimensions and style.
            Width.Set(-35f, 1f);
            Height.Set(30, 0);
            Left.Set(5, 0);
            SetPadding(6f);
            BackgroundColor = defaultColor;
            BorderColor = Color.Black * 0.7f;

            if (IsSetToReload)
            {
                BackgroundColor = selected;
            }

            // --- Mod Icon ---
            UIImage ModImage = new(ModIcon)
            {
                Left = { Pixels = 0 },
                Top = { Pixels = 0 },
                Width = { Pixels = 25 },
                Height = { Pixels = 25 },
                MaxWidth = { Pixels = 25 },
                MaxHeight = { Pixels = 25 },
                VAlign = 0.5f,
                ScaleToFit = true
            };
            Append(ModImage);

            // --- Mod Name & Version ---
            // Display a text element for the mod name and version.
            // Check if the mod name is too long and truncate it if necessary.
            string modName2;
            if (modName.Length > 30)
                modName2 = string.Concat(modName.AsSpan(0, 30), "...");
            else
                modName2 = modName;

            ModTitle = new(modName);
            Append(ModTitle);
        }

        public void SetState(ModItemState state)
        {
            this.state = state;

            switch (state)
            {
                case ModItemState.Default:
                    BackgroundColor = defaultColor;
                    ModTitle.IsEnabled = true;
                    break;
                case ModItemState.Selected:
                    BackgroundColor = selected;
                    ModTitle.IsEnabled = true;
                    break;
                case ModItemState.Disabled:
                    BackgroundColor = disabled;
                    ModTitle.IsEnabled = false;
                    break;
                case ModItemState.Unselected:
                    BackgroundColor = defaultColor;
                    ModTitle.IsEnabled = false;
                    break;
                default:
                    BackgroundColor = Color.Red;
                    break;
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
