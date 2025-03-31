using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Xna.Framework;
using ModHelper.Helpers;
using ModHelper.UI;
using ModHelper.UI.Buttons;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ModHelper.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Reload")]

        [DefaultValue("ModHelper")]
        public string ModToReload = "ModHelper";

        [DefaultValue(true)]
        public bool SaveWorldOnReload = true;

        [Header("UI")]

        [Range(50f, 80f)]
        [Increment(5f)]
        [DefaultValue(70)]
        public float ButtonSize = 70;

        [Range(0f, 1f)]
        [Increment(0.05f)]
        [DefaultValue(typeof(Vector2), "0.5, 1.0")]
        public Vector2 ButtonPosition = new Vector2(0.5f, 1.0f);

        public override void OnChanged()
        {
            base.OnChanged();
            UpdateModsToReload();
            UpdateButtonPosition();
        }

        private void UpdateModsToReload()
        {
            ModsToReload.modsToReload.Clear();
            if (!string.IsNullOrEmpty(ModToReload) && !ModsToReload.modsToReload.Contains(ModToReload))
            {
                ModsToReload.modsToReload.Add(ModToReload);
                Log.Info("Added " + ModToReload + " to modsToReload list.");
            }
        }

        private void UpdateButtonPosition()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;
            if (mainState == null)
            {
                Log.Error("MainState is null in Config::OnChanged().");
                return;
            }

            // We'll anchor the row of buttons using the config's HAlign, VAlign
            float hAlign = this.ButtonPosition.X;  // e.g. 0.5f
            float vAlign = this.ButtonPosition.Y;  // e.g. 1.0f

            float size = this.ButtonSize;
            float spacing = 0f; // or pick a small gap, e.g. 4f

            // Loop through each button and position them
            for (int i = 0; i < mainState.AllButtons.Count; i++)
            {
                BaseButton btn = mainState.AllButtons[i];

                // 1) All buttons share the same anchor
                btn.HAlign = hAlign;
                btn.VAlign = vAlign;

                // 2) The first button has 0 offset, subsequent ones are spaced horizontally
                float offsetX = i * (size + spacing);

                // This offset is from the anchor, so each subsequent button is to the right
                btn.Left.Set(offsetX, 0f);

                // 3) Apply the new width/height
                btn.Width.Set(size, 0f);
                btn.Height.Set(size, 0f);

                // 4) Recalculate each button
                btn.Recalculate();
            }

            // Finally, recalc the entire UI
            mainState.Recalculate();
        }

    }

    internal static class Conf
    {
        /// <summary>
        /// Reference:
        // https://github.com/CalamityTeam/CalamityModPublic/blob/1.4.4/CalamityMod.cs#L550
        /// This is a workaround to save the config manually.
        /// </summary>
        internal static void Save()
        {
            try
            {
                MethodInfo saveMethodInfo = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
                if (saveMethodInfo is not null)
                    saveMethodInfo.Invoke(null, [Conf.C]);
            }
            catch
            {
                Log.Error("An error occurred while manually saving ModConfig!.");
            }
        }

        // Instance
        public static Config C => ModContent.GetInstance<Config>();
    }
}
