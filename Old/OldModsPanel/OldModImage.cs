// using System;
// using System.IO;
// using System.Runtime.CompilerServices;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using SquidTestingMod.Helpers;
// using Terraria;
// using Terraria.GameContent.UI.Elements;
// using Terraria.ModLoader;
// using Terraria.UI;

// namespace SquidTestingMod.UI.Elements
// {
//     public class ModImage : UIImage
//     {
//         private Texture2D tex;
//         private string modPath;

//         public ModImage(Texture2D tex, string modPath) : base(tex)
//         {
//             this.tex = tex;
//             this.modPath = modPath;
//         }

//         public override void Draw(SpriteBatch spriteBatch)
//         {
//             if (tex != null)
//             {
//                 int widest = tex.Width > tex.Height ? tex.Width : tex.Height;

//                 // try catch to setimage to the modpath
//                 try
//                 {
//                     // Attempt whichever approach is appropriate
//                     Texture2D tex = ModContent.Request<Texture2D>(modPath).Value;
//                     if (tex != null)
//                     {
//                         SetImage(tex);
//                         Width.Set(20f, 0f);
//                         Height.Set(20f, 0f);
//                     }
//                 }
//                 catch (Exception ex)
//                 {
//                     if (Main.GameUpdateCount % 120 == 0)
//                     {
//                         Log.Info($"Failed to get tex at path '{modPath}' \n{ex}");
//                     }
//                 }
//             }
//             else
//             {
//                 Log.Warn("tex is null at " + modPath);
//             }


//             DrawHelper.DrawProperScale(spriteBatch, this, tex);
//         }
//     }
// }
