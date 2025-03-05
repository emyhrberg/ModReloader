using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace SquidTestingMod.UI.Panels
{
    public class PlayerInfoPanel : DraggablePanel
    {
        // Variables
        UIText pos = new UIText("Position: (0, 0)");
        protected Color darkBlueLowAlpha = new(73, 85, 186, 100);

        public PlayerInfoPanel(string header) : base(header)
        {
            Active = true;
            Draggable = true;
            VAlign = 0.5f; // top aligned
            HAlign = 0.5f;
            Top.Set(140f, 0f); // move to below player
            Height.Set(250, 0f);
            BackgroundColor = darkBlueLowAlpha;

            // add position info
            pos.Top.Set(40f, 0f);
            Append(pos);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // update position info
            pos.SetText($"Position: ({(int)Main.LocalPlayer.position.X}, {(int)Main.LocalPlayer.position.Y})");
        }
    }
}