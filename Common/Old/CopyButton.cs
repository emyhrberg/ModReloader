// using System;
// using Microsoft.Xna.Framework.Graphics;
// using ModHelper.Helpers;
// using ReLogic.OS;
// using Terraria.GameContent.UI.Elements;
// using Terraria.UI;

// namespace ModHelper.Common.Systems.Menus
// {
//     public class CopyButton : UITextPanel<string>
//     {
//         private string errorMessage;
//         private bool hasCopied = false;
//         DateTime copyTime;

//         public CopyButton(string text, float textScale, bool large, string errorMessage) : base(text, textScale, large)
//         {
//             this.errorMessage = errorMessage;

//             // Bottom right position. There are 3 other buttons.
//             Width.Set(-10, 0.5f);
//             Height.Set(50, 0f);
//             HAlign = 1.0f;
//         }

//         public override void LeftClick(UIMouseEvent evt)
//         {
//             base.LeftClick(evt);

//             ReLogic.OS.Platform.Get<IClipboard>().Value = errorMessage;
//             Log.Info("Copied error message to clipboard.");
//             hasCopied = true;

//             // Start the timer
//             copyTime = DateTime.Now;
//         }

//         public override void Draw(SpriteBatch spriteBatch)
//         {
//             base.Draw(spriteBatch);

//             // Draw the button with hover:
//             if (IsMouseHovering)
//             {
//                 if (!hasCopied)
//                 {
//                     DrawHelper.DrawTooltipPanel(this, "Click to copy", "Copy the error message to your clipboard.");
//                 }
//                 else
//                 {
//                     DrawHelper.DrawTooltipPanel(this, "Copied!", "The error message has been copied to your clipboard.");
//                 }

//                 // If it's been 3 seconds, reset the button
//                 TimeSpan interval = TimeSpan.FromSeconds(3);
//                 bool has3SecondsPassed = DateTime.Now - copyTime >= interval;
//                 if (has3SecondsPassed)
//                 {
//                     hasCopied = false;
//                 }
//             }
//         }
//     }
// }