using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class ModInfoIcon : UIImage
    {
        private static ModInfoIcon currentlyOpenInfo = null;

        private Asset<Texture2D> tex;
        private string hover;
        public bool isInfoOpen = false;
        public string modName;
        private string modDescription;
        private string modCleanName;

        public ModInfoIcon(Asset<Texture2D> texture, string modPath, string hover = "", string modDescription = "", string modCleanName = "") : base(texture)
        {
            tex = texture;
            this.hover = hover;
            this.modName = System.IO.Path.GetFileName(modPath);
            this.modDescription = modDescription;
            this.modCleanName = modCleanName;

            float size = 23f;
            MaxHeight.Set(size, 0f);
            MaxWidth.Set(size, 0f);
            Width.Set(size, 0f);
            Height.Set(size, 0f);
            VAlign = 1.0f;
            Top.Set(6, 0);
        }

        public void SetStateToClosed()
        {
            hover = $"Open {modName} info";
            tex = Ass.ConfigOpen;
            // Main.NewText("Closing config for " + modName, new Color(226, 57, 39));
            Main.menuMode = 0;
            Main.InGameUI.SetState(null);
            isInfoOpen = false;

            // If this is the currently open config, clear the static reference
            if (currentlyOpenInfo == this)
            {
                currentlyOpenInfo = null;
            }
        }

        public void SetStateToOpen()
        {
            hover = $"Close {modName} info";
            isInfoOpen = true;
            Main.playerInventory = false;
            tex = Ass.ConfigClose;
            currentlyOpenInfo = this;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            // Log.Info("Mod description: " + modDescription);

            ModInfoState.instance.SetModInfo(modDescription, modCleanName, modName);
            IngameFancyUI.OpenUIState(ModInfoState.instance);

            if (isInfoOpen)
            {
                SetStateToClosed();
                return;
            }

            // Close any other open info
            if (currentlyOpenInfo != null && currentlyOpenInfo != this)
            {
                currentlyOpenInfo.SetStateToClosed();
            }

            try
            {

            }
            catch (Exception ex)
            {
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            DrawHelper.DrawProperScale(spriteBatch, this, tex.Value, scale: 1.0f);

            if (!string.IsNullOrEmpty(hover) && IsMouseHovering)
            {
                if (!Conf.C.ShowTooltips)
                {
                    return;
                }
                UICommon.TooltipMouseText(hover);
            }
        }
    }
}