using System;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements.ModElements
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
            modName = System.IO.Path.GetFileName(modPath);
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
            hover = $"More Info";
            tex = Ass.ConfigOpen;
            // Main.NewText("Closing config for " + modName, new Color(226, 57, 39));
            Main.menuMode = 0;
            //Main.InGameUI.SetState(null);
            IngameFancyUI.Close();
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

            Log.Info("Mod description: " + modDescription);

            // ModInfoState.instance.CurrentModDescription = this.modDescription;
            // ModInfoState.instance.modDisplayName = modCleanName;

            // NEW: set description, displayName AND internalName in one call
            ModInfoState.instance.SetModInfo(
                description: modDescription,
                displayName: modCleanName,
                internalName: modName
            );

            // OPEN state
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
            catch (Exception)
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
                UICommon.TooltipMouseText(hover);
            }
        }
    }
}