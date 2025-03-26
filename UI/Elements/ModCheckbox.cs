using System;
using System.Linq;
using ErkysModdingUtilities.Common.Configs;
using ErkysModdingUtilities.Helpers;
using ErkysModdingUtilities.UI.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ErkysModdingUtilities.UI.Elements
{
    public class ModCheckbox : UIImage
    {
        private bool isChecked = false;
        private Texture2D uncheck;
        private Texture2D check;
        private string hover;
        private string modSourcePathString;

        public ModCheckbox(Texture2D uncheck, string modSourcePathString, string hover = "") : base(uncheck)
        {
            this.uncheck = uncheck;
            this.check = Ass.ModCheck.Value;
            this.hover = hover;
            this.modSourcePathString = modSourcePathString;

            // size and pos
            float size = 25f;
            MaxHeight.Set(size, 0f);
            MaxWidth.Set(size, 0f);
            Width.Set(size, 0f);
            Height.Set(size, 0f);
            VAlign = 0.5f;
        }

        public void ToggleCheckState()
        {
            isChecked = !isChecked;
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
                if (mod.modSourcePathString == modSourcePathString)
                {
                    // set checkbox
                    mod.checkbox.ToggleCheckState();

                    // update list
                    if (mod.checkbox.isChecked)
                    {
                        // Update config
                        Conf.C.ModToReload = modSourcePathString;
                        Conf.ForceSaveConfig(Conf.C);

                        // only add if it doesnt already exist
                        if (!ModsToReload.modsToReload.Contains(modSourcePathString))
                        {
                            ModsToReload.modsToReload.Add(modSourcePathString);
                        }
                    }
                    else
                    {
                        ModsToReload.modsToReload.Remove(modSourcePathString);
                    }

                    // set hovertext in reloadSP
                    ReloadSPButton sp = sys.mainState.reloadSPButton;
                    ReloadMPButton mp = sys.mainState.reloadMPButton;
                    sp.UpdateHoverText();
                    mp.UpdateHoverText();

                    Log.Info("mods to reload: " + string.Join(", ", ModsToReload.modsToReload));
                }
                // else
                // {
                // mod.checkbox.SetCheckState(false);
                // }
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