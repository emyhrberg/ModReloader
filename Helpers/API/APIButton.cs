using System;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.UI.Elements.ButtonElements;
using ReLogic.Content;
using Terraria.UI;

namespace ModReloader.Helpers.API
{
    /// <summary>
    /// Also, <see cref="ModReloader"/> for Mod.Call integration. 
    /// </summary>
    public class APIButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        public Action Action { get; private set; }
        public void SetAction(Action action) => Action = action;

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            try
            {
                Action?.Invoke();
            }
            catch (Exception e)
            {
                Log.Error($"Failed to execute button action: {e.Message}");
                Main.NewText("Button action failed", Color.Red);
            }
        }
    }
}