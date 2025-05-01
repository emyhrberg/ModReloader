using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Common.Systems;
using ModHelper.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.ModElements
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
            check = Ass.ModCheck.Value;
            this.hover = hover;
            this.modSourcePathString = modSourcePathString;

            // size and pos
            float size = 25f;
            MaxHeight.Set(size, 0f);
            MaxWidth.Set(size, 0f);
            Width.Set(size, 0f);
            Height.Set(size, 0f);
            VAlign = 1.0f;
            Top.Set(6, 0);

            // update: read the json file, and update the checkboxes according to the json file.

            HashSet<string> ModsToReload = Conf.C.ModsToReload.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(modName => modName.Trim())
                .Where(modName => !string.IsNullOrEmpty(modName))
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            foreach (var checkedMod in ModsToReload)
            {
                if (checkedMod == modSourcePathString)
                {
                    ToggleCheckState();
                    Conf.C.ModsToReload = Conf.C.ModsToReload.Replace(modSourcePathString + ",", "");
                    Conf.Save(); // Save the config after updating the ModsToReload string
                }
            }
        }

        public void ToggleCheckState()
        {
            isChecked = !isChecked;

            if (isChecked)
            {
                hover = $"Remove {modSourcePathString} from reload";
            }
            else
            {
                hover = $"Add {modSourcePathString} to reload";
            }
        }
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            // toggle config and check status
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            ModSourcesPanel modSourcesPanel = sys.mainState.modSourcesPanel;

            foreach (var mod in modSourcesPanel.modSourcesElements)
            {
                string internalFolderNameFromMod = Path.GetFileName(mod.modPath);
                Log.Info("clicked on mod: " + modSourcePathString);
                // Log.Info("checking mod: " + internalFolderNameFromMod);

                if (internalFolderNameFromMod == modSourcePathString)
                {
                    // set checkbox
                    mod.checkbox.ToggleCheckState();

                    // update list
                    if (mod.checkbox.isChecked)
                    {
                        // only add if it doesnt already exist
                        //if (!ReloadUtilities.ModsToReload.Contains(modSourcePathString))
                        {
                            //ReloadUtilities.ModsToReload.Add(modSourcePathString);
                            // Log.Info("added mod to reload: " + modSourcePathString);
                        }
                    }
                    else
                    {
                        // unchecked. 
                        // Log.Info("removing mod to reload: " + modSourcePathString);
                        //ReloadUtilities.ModsToReload.Remove(modSourcePathString);
                    }

                    // set hovertext in reloadSP
                    ReloadSPButton sp = sys.mainState.reloadSPButton;
                    ReloadMPButton mp = sys.mainState.reloadMPButton;
                    sp?.UpdateHoverTextDescription();
                    mp?.UpdateHoverTextDescription();

                    // Add mod to reload list if not already present

                    if (!Conf.C.ModsToReload.Contains(modSourcePathString))
                    {
                        Conf.C.ModsToReload += modSourcePathString + ",";
                    }
                    else
                    {
                        Conf.C.ModsToReload = Conf.C.ModsToReload.Replace(modSourcePathString + ",", "");
                    }
                    Conf.Save(); // Save the config after updating the ModsToReload string
                }
            }
            modSourcesPanel.Recalculate();
        }

        public override void Draw(SpriteBatch sb)
        {
            // base.Draw(sb);

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