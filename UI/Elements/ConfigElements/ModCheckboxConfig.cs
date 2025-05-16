using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Systems;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModReloader.UI.Elements.ConfigElements
{
    public class ModCheckboxConfig : UIImage
    {
        private bool isChecked = false;
        private Texture2D uncheck;
        private Texture2D check;
        private string hover;
        private string modSourcePathString;
        public ModSourcesConfig parentConfig;


        public ModCheckboxConfig(ModSourcesConfig parent, Texture2D uncheck, string modSourcePathString, string hover = "") : base(uncheck)
        {
            parentConfig = parent;

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

            foreach (var checkedMod in parentConfig.GetValue())
            {
                if (checkedMod == modSourcePathString)
                {
                    ToggleCheckState();
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
            var l = parentConfig.GetValue();

            foreach (var mod in parentConfig.modSourcesPanelConfig.modSourcesElements)
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
                        if (!l.Contains(modSourcePathString))
                        {
                            l.Add(modSourcePathString);
                            Log.Info("added mod to reload: " + modSourcePathString);
                        }
                    }
                    else
                    {
                        // unchecked. 
                        if (l.Remove(modSourcePathString))
                        {
                            Log.Info("removed mod to reload: " + modSourcePathString);
                        }
                        else
                        {
                            Log.Info("mod not found in reload list: " + modSourcePathString);
                        }
                    }

                    // set hovertext in reloadSP
                    if (sys != null && sys.mainState != null)
                    {
                        // we need this check if we are main menu
                        Log.Info("Updating hover text description");
                        sys.mainState.reloadSPButton?.UpdateHoverTextDescription();
                        sys.mainState.reloadMPButton?.UpdateHoverTextDescription();
                    }

                    //Conf.Save(); // Save the config after updating the ModsToReload string
                    parentConfig.SetValue(l);
                }
            }
            parentConfig.modSourcesPanelConfig.Recalculate();
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