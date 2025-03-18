using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;

namespace SquidTestingMod.UI.Elements
{
    // A simplified UI panel that mimics the UIModItem appearance.
    public class ModItemPanel : UIPanel
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
        public ModItemPanel(bool isSetToReload, string modName, string hover, string modPath=null) : base()
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
            Asset<Texture2D> defaultIconTemp = Main.Assets.Request<Texture2D>("Images/UI/DefaultResourcePackIcon", AssetRequestMode.ImmediateLoad);

            if (modPath != null)
            {
                float size = 20f;
                ModImage modImage = new(defaultIconTemp.Value, modPath)
                {
                    Left = { Pixels = 0 },
                    Top = { Pixels = 0 },
                    Width = { Pixels = size },
                    Height = { Pixels = size },
                    MaxWidth = { Pixels = size },
                    MaxHeight = { Pixels = size },
                    VAlign = 0.5f,
                    ScaleToFit = true
                };
                Log.Info("created ModImage: " + ModName);
                Append(modImage);
            }
            else
            {
                Log.Info("modTex is null: " + ModName);
            }


            // --- Mod Name & Version ---
            string modName2;

            if (ModName.Length > 28)
                modName2 = string.Concat(ModName.AsSpan(0, 28), "...");
            else
                modName2 = ModName;

            ModTitle = new(modName2);
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
                    //ModTitle.IsEnabled = false; // uncomment to make ModTitle text less opacity
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
