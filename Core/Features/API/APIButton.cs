using System;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.UI.Shared;
using ReLogic.Content;
using Terraria.UI;

namespace ModReloader.Core.Features.API
{
    /// <summary>
    /// Also, <see cref="ModReloader"/> for Mod.Call integration. 
    /// </summary>
    public class APIButton : BaseButton
    {
        public APIButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription)
            : base(spritesheet, buttonText, hoverText, hoverTextDescription)
        {
            Spritesheet = spritesheet;
            ButtonText = new ButtonText(buttonText);
            HoverText = hoverText;
            HoverTextDescription = hoverTextDescription;
        }

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