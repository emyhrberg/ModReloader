using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace SquidTestingMod.UI.Panels
{
    // A simplified UI panel that mimics the UIModItem appearance.
    public class CustomModItem : UIPanel
    {
        private UIImage modIcon;
        private UIText modName;

        public CustomModItem()
        {
            // Set overall panel dimensions and style.
            Width.Set(0, 1f);
            Height.Set(0f, 1f);
            SetPadding(6f);
            BorderColor = new Color(89, 116, 213) * 0.7f;
            BackgroundColor = new Color(50, 50, 50) * 0.8f;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            // --- Mod Icon ---
            // Request a default icon asset. In a real mod this might be replaced with a custom icon.
            Asset<Texture2D> iconAsset = Main.Assets.Request<Texture2D>("Images/UI/DefaultResourcePackIcon", AssetRequestMode.ImmediateLoad);
            modIcon = new UIImage(iconAsset)
            {
                Left = { Pixels = 0 },
                Top = { Pixels = 0 },
                Width = { Pixels = 80 },
                Height = { Pixels = 80 },
                ScaleToFit = true
            };
            Append(modIcon);

            // --- Mod Name & Version ---
            // Display a text element for the mod name and version.
            modName = new UIText("Example Mod v1.0")
            {
                Left = { Pixels = 90 },
                Top = { Pixels = 5 }
            };
            Append(modName);
        }
    }
}
