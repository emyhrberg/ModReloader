using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Buttons;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class ModCheckbox : UIImage
    {
        private bool isChecked = false;
        private Texture2D uncheck;
        private Texture2D check;
        private string hover;
        private string modName;

        public ModCheckbox(Texture2D uncheck, string modName, string hover = "") : base(uncheck)
        {
            this.uncheck = uncheck;
            this.check = Ass.ModCheck.Value;
            this.hover = hover;
            this.modName = modName;

            // size and pos
            float size = 25f;
            MaxHeight.Set(size, 0f);
            MaxWidth.Set(size, 0f);
            Width.Set(size, 0f);
            Height.Set(size, 0f);
            VAlign = 0.5f;
        }

        public void SetCheckState(bool state)
        {
            isChecked = state;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            // toggle config and check status
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            ModsPanel modsPanel = sys.mainState.modsPanel;

            foreach (var mod in modsPanel.modSourcesElements)
            {
                if (mod.modName == modName)
                {
                    // set config and save
                    Conf.C.ModToReload = modName;
                    Conf.ForceSaveConfig(Conf.C);

                    // set hovertext in reloadSP
                    ReloadSPButton sp = sys.mainState.reloadSPButton;
                    sp.UpdateHoverText("Reload " + modName);

                    // set checkbox
                    mod.checkbox.SetCheckState(true);
                }
                else
                {
                    mod.checkbox.SetCheckState(false);
                }
            }
            modsPanel.Recalculate();
        }

        public override void Draw(SpriteBatch sb)
        {
            // base.Draw(sb);

            Top.Set(-0, 0f);

            if (isChecked)
            {
                DrawHelper.DrawProperScale(sb, this, check, scale: 1.0f);
            }
            else
            {
                DrawHelper.DrawProperScale(sb, this, uncheck, scale: 1.0f);
            }

            if (!string.IsNullOrEmpty(hover) && IsMouseHovering)
            {
                UICommon.TooltipMouseText(hover);
            }
        }
    }
}