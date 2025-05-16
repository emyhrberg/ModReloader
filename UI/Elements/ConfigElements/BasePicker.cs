using ModReloader.Helpers;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;

namespace ModReloader.UI.Elements.ConfigElements
{
    internal class BasePicker : ConfigElement
    {
        public IList<int> IntList { get; set; }
        protected virtual int Max => 100;
        public int Increment { get; set; } = 1;

        private bool focusFlag = false;

        public override void OnBind()
        {
            base.OnBind();

            IntList = (IList<int>)List;

            if (IntList != null)
            {
                TextDisplayFunction = () => Index + 1 + ": " + IntList[Index];
            }
            if (IncrementAttribute != null && IncrementAttribute.Increment is int)
            {
                Increment = (int)IncrementAttribute.Increment;
            }

            UIPanel textBoxBackground = new UIPanel();
            textBoxBackground.SetPadding(0);
            UIFocusInputTextField uIInputTextField = new UIFocusInputTextField(Language.GetTextValue("tModLoader.ModConfigTypeHere"));
            textBoxBackground.Top.Set(0f, 0f);
            textBoxBackground.Left.Set(-190, 1f);
            textBoxBackground.Width.Set(180, 0f);
            textBoxBackground.Height.Set(30, 0f);
            Append(textBoxBackground);

            SetFieldText(uIInputTextField);
            uIInputTextField.Top.Set(5, 0f);
            uIInputTextField.Left.Set(10, 0f);
            uIInputTextField.Width.Set(-42, 1f); // allow space for arrows
            uIInputTextField.Height.Set(20, 0);
            uIInputTextField.OnTextChange += (a, b) =>
            {
                focusFlag = false;
                if (int.TryParse(uIInputTextField.CurrentString.Split(": ")[0], out int val))
                {
                    SetValue(val);

                }
                Log.Info($"OnTextChange: {uIInputTextField.CurrentString}");

                //else /{
                //	Interface.modConfig.SetMessage($"{uIInputTextField.currentString} isn't a valid value.", Color.Green);
                //}
            };
            uIInputTextField.OnUnfocus += (a, b) =>
            {
                if (!focusFlag)
                {
                    focusFlag = true;
                    SetFieldText(uIInputTextField);
                    Log.Info($"OnUnfocus: {uIInputTextField.CurrentString}");
                }


            };
            textBoxBackground.Append(uIInputTextField);

            UIModConfigHoverImageSplit upDownButton = new UIModConfigHoverImageSplit(UpDownTexture, "+" + Increment, "-" + Increment);
            upDownButton.Recalculate();
            upDownButton.Top.Set(4f, 0f);
            upDownButton.Left.Set(-30, 1f);
            upDownButton.OnLeftClick += (a, b) =>
            {
                focusFlag = false;
                Rectangle r = b.GetDimensions().ToRectangle();
                if (a.MousePosition.Y < r.Y + r.Height / 2)
                {
                    SetValue(GetValue() + Increment);
                }
                else
                {
                    SetValue(GetValue() - Increment);
                }
                SetFieldText(uIInputTextField);
                Log.Info($"OnLeftClick: {uIInputTextField.CurrentString}");
            };
            textBoxBackground.Append(upDownButton);
            Recalculate();
        }

        private void SetFieldText(UIFocusInputTextField uIInputTextField)
        {
            uIInputTextField.SetText($"{GetValue()}: {GetName()}");
        }

        protected virtual int GetValue() => (int)GetObject();

        protected virtual void SetValue(int value)
        {
            if (value > Max)
                value = 0;
            if (value < 0)
                value = Max;
            SetObject(value);
        }

        /// <summary>
        /// Get the name of the current value.
        /// </summary>
        /// <returns>Name of the current value. </returns>
        protected virtual string GetName()
        {
            return "";
        }

    }
}
