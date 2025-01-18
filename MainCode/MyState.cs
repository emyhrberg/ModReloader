using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.UI;
using Terraria.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SkipSelect.MainCode
{
    public class MyState : UIState
    {
        public override void OnInitialize()
        {
            // Load button texture
            Asset<Texture2D> buttonCloseTexture = ModContent.Request<Texture2D>("SkipSelect/MainCode/ButtonClose");

            // Create the button
            MyHoverButton buttonClose = new(buttonCloseTexture, "Close");

            // Set button size and position
            buttonClose.Width.Set(100f, 0f);
            buttonClose.Height.Set(100f, 0f);
            buttonClose.Top.Set(10f, 0f);
            buttonClose.Left.Set(10f, 0f);

            // Add click event
            buttonClose.OnLeftClick += CloseButtonClicked;

            // Add the button to the UIState
            Append(buttonClose);
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            Main.NewText("Close button clicked!");
        }
    }
}
