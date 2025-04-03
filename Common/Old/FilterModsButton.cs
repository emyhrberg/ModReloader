// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using ModHelper.Helpers;
// using ReLogic.Content;
// using Terraria.GameContent.UI.Elements;
// using Terraria.ModLoader;
// using Terraria.ModLoader.UI;

// namespace ModHelper.UI.Elements
// {
//     /// <summary>
//     /// A button that can be clicked to sort items in the ItemSpawner and NPCSpawner panels.
//     /// </summary>
//     public class FilterModsButton : UIImageButton
//     {
//         // Variables
//         private readonly Asset<Texture2D> icon;
//         private readonly string HoverText = "";
//         public bool Active = true; // whether this filter is currently active

//         private string internalModName;
//         public Texture2D givenTexture;
//         public Texture2D updatedTex;

//         public FilterModsButton(Asset<Texture2D> texture, string hoverText, string internalModName, float left) : base(texture)
//         {
//             this.givenTexture = texture.Value;
//             this.internalModName = internalModName;

//             // size and position
//             Width.Set(21f, 0f);
//             Height.Set(21f, 0f);
//             MaxWidth.Set(21f, 0f);
//             MaxHeight.Set(21f, 0f);
//             MinWidth.Set(21f, 0f);
//             MinHeight.Set(21f, 0f);
//             Top.Set(25, 0f);
//             Left.Set(left, 0f);

//             // init stuff
//             icon = texture;
//             HoverText = hoverText;
//             SetImage(icon);
//         }

//         protected override void DrawSelf(SpriteBatch spriteBatch)
//         {
//             if (icon != null && icon.Value != null)
//             {
//                 // Get rectangle dimensions
//                 Rectangle drawRect = GetDimensions().ToRectangle();
//                 float opacity = IsMouseHovering ? 1f : 0.8f;

//                 // Draw the icon
//                 if (internalModName != null)
//                 {
//                     string path = $"{internalModName}/icon";

//                     updatedTex = ModContent.Request<Texture2D>(path).Value;

//                     if (updatedTex != null)
//                     {
//                         DrawHelper.DrawProperScale(spriteBatch, this, updatedTex, opacity: opacity, active: Active);
//                     }
//                     // If active, draw the active background
//                     if (Active)
//                         spriteBatch.Draw(Ass.FilterBGActiveModSort.Value, drawRect, Color.White * 0.5f);
//                 }
//                 else
//                 {
//                     // If active, draw the active background
//                     if (Active)
//                         spriteBatch.Draw(Ass.FilterBGActive.Value, drawRect, Color.White);
//                     else
//                         spriteBatch.Draw(Ass.FilterBG.Value, drawRect, Color.White);

//                     // Draw the given texture
//                     updatedTex = givenTexture;
//                     DrawHelper.DrawProperScale(spriteBatch, this, updatedTex);
//                 }

//                 // Draw tooltip text if hovering.
//                 if (IsMouseHovering)
//                 {
//                     UICommon.TooltipMouseText(HoverText);
//                     // Vector2 mousePos = new(Main.mouseX, Main.mouseY);
//                     // spriteBatch.Draw(updatedTex, mousePos, Color.White);
//                 }
//                 // Main.hoverItemName = HoverText;
//             }
//         }
//     }
// }