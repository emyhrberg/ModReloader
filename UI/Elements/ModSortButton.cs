using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    /// <summary>
    /// A button that can be clicked to sort items in the ItemSpawner and NPCSpawner panels.
    /// </summary>
    public class ModSortButton : UIImageButton
    {
        // Variables
        private readonly Asset<Texture2D> icon;
        private readonly string HoverText = "";
        public bool Active = true; // whether this filter is currently active

        private string internalModName;
        public Texture2D givenTexture;
        public Texture2D updatedTex;

        public ModSortButton(Asset<Texture2D> texture, string hoverText, string internalModName, float left) : base(texture)
        {
            this.givenTexture = texture.Value;
            this.internalModName = internalModName;

            // size and position
            Width.Set(21f, 0f);
            Height.Set(21f, 0f);
            MaxWidth.Set(21f, 0f);
            MaxHeight.Set(21f, 0f);
            MinWidth.Set(21f, 0f);
            MinHeight.Set(21f, 0f);
            Top.Set(25, 0f);
            Left.Set(left, 0f);

            // init stuff
            icon = texture;
            HoverText = hoverText;
            SetImage(icon);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (icon != null && icon.Value != null)
            {
                // Get rectangle dimensions
                Rectangle drawRect = GetDimensions().ToRectangle();
                float opacity = IsMouseHovering ? 1f : 0.7f;

                // If active, draw the active background
                if (Active)
                    spriteBatch.Draw(Ass.FilterBGActive.Value, drawRect, Color.White);
                else
                    spriteBatch.Draw(Ass.FilterBG.Value, drawRect, Color.White);

                // Always draw the icon on top with full opacity if active,
                // otherwise draw with the opacity value.
                if (Active)
                    spriteBatch.Draw(icon.Value, drawRect, Color.White);
                else
                    spriteBatch.Draw(icon.Value, drawRect, Color.White * opacity);

                // Draw the icon
                if (internalModName != null)
                {
                    string path = $"{internalModName}/icon";

                    updatedTex = ModContent.Request<Texture2D>(path).Value;

                    if (updatedTex != null)
                    {
                        DrawHelper.DrawProperScale(spriteBatch, this, updatedTex);
                    }
                }
                else
                {
                    updatedTex = givenTexture;
                    DrawHelper.DrawProperScale(spriteBatch, this, updatedTex);
                }

                // Draw tooltip text if hovering.
                if (IsMouseHovering)
                    Main.hoverItemName = HoverText;
            }
        }
    }
}