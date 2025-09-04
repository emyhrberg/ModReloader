using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements.ModElements;

internal class ClearSearchButton : UIImage
{
    private Rectangle customBox;

    public ClearSearchButton(Asset<Texture2D> texture) : base(texture)
    {
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        var dims = GetDimensions();
        customBox = dims.ToRectangle();
        customBox.Width = 23;
        customBox.Height = 23;
    }

    public override bool ContainsPoint(Vector2 point)
    {
        return customBox.Contains(point.ToPoint());
    }

    public override void Draw(SpriteBatch sb)
    {
        if (_texture == null)
            return;

        // Debug draw custom hitbox
        //sb.Draw(TextureAssets.MagicPixel.Value, customBox, Color.Red * 0.25f);

        // Draw hover
        bool hover = customBox.Contains(Main.MouseScreen.ToPoint());
        float alpha = hover ? 1f : 0.6f;
        sb.Draw(_texture.Value, GetDimensions().ToRectangle(), Color.White * alpha);

        // Act on click
        if (hover && Main.mouseLeft)
        {
            if (Parent is Searchbox searchbox)
            {
                searchbox.SetText(string.Empty);
                //searchbox.currentFilter = string.Empty;
                //updateNeeded = true;
            }
        }
    }
}
