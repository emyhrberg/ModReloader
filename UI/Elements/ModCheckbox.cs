using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ModHelper.UI.Buttons;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Elements
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

            // check if this mod is the current mod to reload in the config. if so, check it.
            // if (Conf.C.ModToReload == modSourcePathString)
            // {
            //     ToggleCheckState();
            //     // add to mods to reload also

            //     ReloadUtilities.ModsToReload.Add(modSourcePathString);
            // }

            // update: read the json file, and update the checkboxes according to the json file.
            List<string> modsToReloadFromJsonFile = ModsToReloadJsonHelper.ReadModsToReload();
            foreach (var checkedMod in modsToReloadFromJsonFile)
            {
                if (checkedMod == modSourcePathString)
                {
                    ToggleCheckState();
                    ReloadUtilities.ModsToReload.Add(modSourcePathString);
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
                Log.Info("checking mod: " + internalFolderNameFromMod);

                if (internalFolderNameFromMod == modSourcePathString)
                {
                    // set checkbox
                    mod.checkbox.ToggleCheckState();

                    // update list
                    if (mod.checkbox.isChecked)
                    {
                        // Update config
                        // Conf.C.ModToReload = modSourcePathString;
                        // Conf.Save();

                        // only add if it doesnt already exist
                        if (!ReloadUtilities.ModsToReload.Contains(modSourcePathString))
                        {
                            ReloadUtilities.ModsToReload.Add(modSourcePathString);
                            // Log.Info("added mod to reload: " + modSourcePathString);
                        }
                    }
                    else
                    {
                        // Log.Info("removing mod to reload: " + modSourcePathString);
                        ReloadUtilities.ModsToReload.Remove(modSourcePathString);
                        // Conf.C.ModToReload = ReloadHelper.ModsToReload.Count > 0 ? ReloadHelper.ModsToReload.LastOrDefault() : string.Empty;
                        // Conf.Save();

                        // unchecked.
                        // update config if modstoreload only has one entry
                        if (ReloadUtilities.ModsToReload.Count == 1)
                        {
                            // Conf.C.ModToReload = ReloadUtilities.ModsToReload.FirstOrDefault();
                            // Log.Info("Setting single mod to reload to: " + Conf.C.ModToReload);
                            // Conf.Save();
                        }
                    }

                    // set hovertext in reloadSP
                    ReloadSPButton sp = sys.mainState.reloadSPButton;
                    ReloadMPButton mp = sys.mainState.reloadMPButton;
                    if (sp != null)
                    {
                        sp.UpdateHoverTextDescription();
                    }
                    if (mp != null)
                    {
                        mp.UpdateHoverTextDescription();
                    }

                    // Log.Info("mods to reload: " + string.Join(", ", ModsToReload.modsToReload));
                    Main.NewText("Mods to reload: " + string.Join(", ", ReloadUtilities.ModsToReload));

                    // Write to json file
                    ModsToReloadJsonHelper.WriteModsToReload(ReloadUtilities.ModsToReload);
                }
            }
            modSourcesPanel.Recalculate();
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