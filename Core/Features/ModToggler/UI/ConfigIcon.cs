using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModReloader.Core.Features.ModToggler.UI
{
    public class ConfigIcon : UIImage
    {
        private static ConfigIcon currentlyOpenConfig;

        private Asset<Texture2D> tex;
        private string hover;
        public bool isConfigOpen;
        public string modName;
        private readonly Action onBack;

        public ConfigIcon(Asset<Texture2D> texture, string modPath, string hover = "", string cleanModName = "", Action onBack = null) : base(texture)
        {
            tex = texture;
            this.hover = hover;
            modName = System.IO.Path.GetFileName(modPath);
            this.onBack = onBack;

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
            hover = "Open config";
            tex = Ass.ConfigOpen;
            Main.menuMode = 0;

            if (onBack != null)
                Main.MenuUI.SetState(null);
            else
                IngameFancyUI.Close();

            isConfigOpen = false;
            if (currentlyOpenConfig == this)
                currentlyOpenConfig = null;
        }

        public void SetStateToOpen()
        {
            hover = "Close config";
            isConfigOpen = true;
            Main.playerInventory = false;
            tex = Ass.ConfigClose;
            currentlyOpenConfig = this;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            SoundEngine.PlaySound(SoundID.MenuClose);

            if (isConfigOpen)
            {
                SetStateToClosed();
                return;
            }

            currentlyOpenConfig?.SetStateToClosed();

            try
            {
                var configs = typeof(ConfigManager).GetField("Configs", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null) as IDictionary<Mod, List<ModConfig>>;
                Mod mod = ModLoader.GetMod(modName);
                if (mod == null || configs == null || !configs.TryGetValue(mod, out List<ModConfig> modConfigs) || modConfigs.Count == 0)
                {
                    Main.NewText("No config available for mod: " + modName, Color.Yellow);
                    return;
                }

                UIState state = Interface.modConfig;
                Action closeAction = () =>
                {
                    SetStateToClosed();
                    onBack?.Invoke();
                };

                state.GetType().GetMethod("SetMod", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    ?.Invoke(state, [mod, modConfigs[0], onBack == null, closeAction, null, true]);

                if (onBack != null)
                {
                    Main.MenuUI.SetState(null);
                    Main.MenuUI.SetState(state);
                    Main.menuMode = 888;
                }
                else
                {
                    Main.InGameUI.SetState(null);
                    IngameFancyUI.OpenUIState(state);
                }

                SetStateToOpen();
            }
            catch (Exception ex)
            {
                Main.NewText($"No config found for mod '{modName}'. : {ex.Message}", Color.Red);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!isConfigOpen)
                return;

            UIState state = Interface.modConfig;
            UIState current = onBack != null ? Main.MenuUI?.CurrentState : Main.InGameUI?.CurrentState;

            if ((onBack != null && Main.menuMode != 888) || current?.GetType() != state.GetType())
                SetStateToClosed();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            DrawHelper.DrawProperScale(spriteBatch, this, tex.Value, scale: 1.0f);

            if (!string.IsNullOrEmpty(hover) && IsMouseHovering)
                UICommon.TooltipMouseText(hover);
        }
    }
}
