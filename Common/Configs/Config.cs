using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Xna.Framework;
using ModHelper.Helpers;
using ModHelper.UI;
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
        [Increment(0.01f)]
        [DefaultValue(typeof(Vector2), "0.5, 1.0")]
        public Vector2 ButtonPosition = new Vector2(0.5f, 1.0f);

        public override void OnChanged()
        {
            base.OnChanged();

            // Update modstoreload list with the config
            ModsToReload.modsToReload.Clear();

            if (!ModsToReload.modsToReload.Contains(ModToReload) && !string.IsNullOrEmpty(ModToReload))
            {
                ModsToReload.modsToReload.Add(ModToReload);
                Log.Info("Added " + ModToReload + " to modsToReload list.");
            }

            // Re-create the MainState
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;

            if (mainState != null)
            {
                mainState.RemoveAllChildren();
                mainState.AddEverything();
                mainState.Recalculate();
            }
            else
            {
                Log.Error("MainState is null in Config::OnChanged().");
            }
        }
    }

    internal static class Conf
    {
        /// <summary>
        /// Reference:
        // https://github.com/CalamityTeam/CalamityModPublic/blob/1.4.4/CalamityMod.cs#L550
        /// This is a workaround to save the config manually.
        /// </summary>
        internal static void ForceSaveConfig()
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
