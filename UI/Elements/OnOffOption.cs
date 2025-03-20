using System;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class OnOffOption : PanelElement
    {
        private Action _leftClickAction; // store the delegate
        private Action _rightClickAction; // store the delegate

        public OnOffOption(Action leftClick, string text, string hoverText, Action rightClick = null) : base(text, hoverText)
        {
            _leftClickAction = leftClick; // save the provided action
            _rightClickAction = rightClick; // save the provided action
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Execute base functionality if any
            base.LeftClick(evt);

            // Execute the left click action
            _leftClickAction?.Invoke();

            // Toggle text between "On" and "Off"
            UpdateText(textElement.Text.Contains("On")
                ? textElement.Text.Replace("On", "Off")
                : textElement.Text.Replace("Off", "On"));
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);

            _rightClickAction?.Invoke();
        }
    }
}
