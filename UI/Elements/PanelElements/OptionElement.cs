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
        private Action<bool> leftClick;
        private bool value;

        public enum EnabledState
        {
            Enabled,
            Disabled
        }

        private EnabledState state => value ? EnabledState.Enabled : EnabledState.Disabled;

        public OptionElement(bool value, Action<bool> leftClick, string text, string hover = "")
        {
            this.value = value;
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
            UpdateState();
        }

        private void UpdateState()
        {
            enabledText.SetTextState(state);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            value = !value;

            leftClick?.Invoke(value);

            UpdateState();
        }

        public void SetValue(bool value)
        {
            this.value = value;
            UpdateState();
        }
    }
}