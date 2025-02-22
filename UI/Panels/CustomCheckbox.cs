using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// Draws a checkbox,
    /// a UIText next to it
    /// and a checkmark if the checkbox is checked.
    /// </summary>
    public class CustomCheckbox : UIElement
    {
        // Assets
        private readonly Asset<Texture2D> checkboxTexture = Assets.CheckBox;
        private readonly Asset<Texture2D> checkmarkTexture = Assets.CheckMark;

        private bool Checked;
        private float Opacity = 1f;

        public CustomCheckbox()
        {
            // Set the size of the checkbox element
            Width.Set(20, 0f); // 20 for checkbox width
            Height.Set(20, 0f); // 20 for checkbox height
        }

        public void Toggle()
        {
            Checked = !Checked;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            // make opacity 0.5 if mouse is over
            Opacity = 0.5f;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            // make opacity 1 if mouse is not over
            Opacity = 1f;
        }

        // Draw the checkbox
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            // Draw the checkbox
            Vector2 position = GetDimensions().Position();
            spriteBatch.Draw(checkboxTexture.Value, position, null, Color.White * Opacity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Draw the checkmark if the checkbox is checked
            if (Checked)
            {
                spriteBatch.Draw(checkmarkTexture.Value, GetDimensions().Position(), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}