using System;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements
{
    public class OptionElement : UIPanel
    {
        public string text;
        private OptionEnabledText enabledText;
        private OptionTitleText optionTitleText;
        private Action leftClick;

        public enum EnabledState
        {
            Enabled,
            Disabled
        }

        private EnabledState state = EnabledState.Disabled;

        public OptionElement(Action leftClick, string text, string hover = "")
        {
            this.leftClick = leftClick;
            this.text = text;

            // size and position
            Width.Set(-35f, 1f);
            Height.Set(30, 0);
            Left.Set(5, 0);

            // mod name
            optionTitleText = new(text, hover);
            Append(optionTitleText);

            // enabled text
            enabledText = new("Disabled");
            Append(enabledText);
        }

        public void SetState(EnabledState state)
        {
            this.state = state;
            enabledText.SetTextState(state);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            SetState(state == EnabledState.Enabled ? EnabledState.Disabled : EnabledState.Enabled);

            // Invoke the left click action
            leftClick?.Invoke();
        }
    }
}