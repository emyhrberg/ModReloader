using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using System.IO;
using SquidTestingMod.Helpers;
using Terraria;

namespace SquidTestingMod.UI.Elements
{
    public class ModImage : UIImage
    {
        private Texture2D tex;
        private string modPath;

        public ModImage(Texture2D tex, string modPath) : base(tex)
        {
            this.tex = tex;
            this.modPath = modPath;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (tex != null)
            {
                int widest = tex.Width > tex.Height ? tex.Width : tex.Height;

                if (Main.GameUpdateCount % 60 == 0)
                {
                    // try catch to setimage to the modpath
                    try
                    {
                        Texture2D tex = ModContent.Request<Texture2D>(modPath).Value;
                        SetImage(tex);
                        //Log.Info("Success");
                    }
                    catch
                    {
                        //Log.Info("Failed");
                    }

                    Log.Info("drawing in ModImage");
                }

                // draw the mod icon
                Rectangle pos = new((int)Left.Pixels, (int)Top.Pixels, (int)Width.Pixels, (int)Height.Pixels);
                //spriteBatch.Draw(tex, pos, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
            else
            {
                Log.Warn("tex is null");
                //Utils.DrawBorderString(spriteBatch, mod.DisplayName[..2], target.Center.ToVector2(), Color.White, 1, 0.5f, 0.4f);
            }

        }
    }
}
