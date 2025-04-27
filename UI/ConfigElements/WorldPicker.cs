using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI;
using Terraria;
using Microsoft.Xna.Framework;
using Humanizer;

namespace ModHelper.UI.ConfigElements
{
    internal class WorldPicker : ConfigElement
    {
        public IList<int> IntList { get; set; }
        public int Max
        {
            get
            {
                Main.LoadWorlds();
                return Main.WorldList.Count - 1;
            }
        }
        public int Increment { get; set; } = 1;

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
                if (int.TryParse(uIInputTextField.CurrentString, out int val))
                {
                    SetValue(val);
                    SetFieldText(uIInputTextField);
                }
                //else /{
                //	Interface.modConfig.SetMessage($"{uIInputTextField.currentString} isn't a valid value.", Color.Green);
                //}
            };
            uIInputTextField.OnUnfocus += (a, b) => uIInputTextField.SetText(GetValue().ToString());
            textBoxBackground.Append(uIInputTextField);

            UIModConfigHoverImageSplit upDownButton = new UIModConfigHoverImageSplit(UpDownTexture, "+" + Increment, "-" + Increment);
            upDownButton.Recalculate();
            upDownButton.Top.Set(4f, 0f);
            upDownButton.Left.Set(-30, 1f);
            upDownButton.OnLeftClick += (a, b) =>
            {
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
            };
            textBoxBackground.Append(upDownButton);
            Recalculate();
        }

        private void SetFieldText(UIFocusInputTextField uIInputTextField)
        {
            uIInputTextField.SetText($"{GetValue()}: {GetPlayerName()}");
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

        private string GetPlayerName()
        {
            Main.LoadWorlds();
            int id = GetValue();
            if (Main.WorldList.Count <= 0)
            {
                return "No World Found!";
            }
            if (id < 0 || id >= Main.WorldList.Count)
            {
                SetValue(0);
                return Main.WorldList[0].Name;
            }
            else
            {
                return Main.WorldList[id].Name;
            }

        }

    }
}
