using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SquidTestingMod.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Reload")]
        [DefaultValue("EnterYourModHere")]
        public string ModToReload = "EnterYourModHere";

        [DefaultValue(false)]
        public bool SaveWorldOnReload = false;

        [DefaultValue(false)]
        public bool ClearClientLogOnReload = false;

        [Header("ItemNPCSpawner")]
        [DefaultValue(1000)]
        [Range(100, 6000)]
        [Increment(1000)]
        public int MaxItemsToDisplay;

        [Range(-500f, 500f)]
        [Increment(100f)]
        [DefaultValue(typeof(Vector2), "0, 0")]
        public Vector2 NPCSpawnLocation;

        [Header("Misc")]

        [DefaultValue(true)]
        public bool ShowCombatTextOnToggle = true;

        [DefaultValue(true)]
        public bool HoverEffectButtons = true;

        [DefaultValue(false)]
        public bool ReloadButtonsOnly = false;

        [DefaultValue(false)]
        public bool StartInGodMode = false;

        [DefaultValue(true)]
        public bool DrawGodGlow = true;

        // Debug Panel Config Settings Goes Here For Temporary Storage

        // World Panel Config Settings Goes Here For Temporary Storage

        public override void OnChanged()
        {
            // Here we can update the game based on the new config values
        }
    }
}
