using Terraria.GameContent.UI.Elements;

namespace SquidTestingMod.UI
{
    // Subclass of UITextBox that overrides the minimum height
    public class UISearchBox : UITextBox
    {
        public UISearchBox(string text, float textScale = 1f) : base(text, textScale)
        {
        }

        public override void Recalculate()
        {
            base.Recalculate();
            // Force a smaller height
            Height.Pixels = 20f;
        }
    }
}