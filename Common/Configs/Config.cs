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

        // ------------------- Reload -------------------
        [Header("Reload")]
        [DefaultValue("EnterYourModHere")]
        public string ModToReload = "EnterYourModHere";

        [DefaultValue(false)]
        public bool SaveWorldOnReload = false;

        [DefaultValue(false)]
        public bool ClearClientLogOnReload = false;

        // ------------------- NPC/Item Spawner -------------------
        [Header("ItemNPCSpawner")]
        [DefaultValue(1000)]
        [Range(100, 6000)]
        [Increment(1000)]
        public int MaxItemsToDisplay;

        [Range(-1000f, 1000f)]
        [Increment(100f)]
        [DefaultValue(typeof(Vector2), "0, 0")]
        public Vector2 NPCSpawnLocation;

        // ------------------- Misc -------------------
        [Header("Misc")]

        [DefaultValue(true)]
        public bool ShowCombatTextOnToggle = true;

        // ------------------- Buttons ------------------- (the buttons in the UI)
        [DefaultValue(true)]
        public bool HoverEffectButtons = true;

        [DefaultValue(false)]
        public bool ReloadButtonsOnly = false;

        // ------------------- Player ------------------- (the player panel)
        [DefaultValue(false)]
        public bool StartInGodMode = false;

        [DefaultValue(true)]
        public bool DrawGodGlow = true;

        // ------------------- Debug ------------------- (the debug panel)

        // ------------------- World ------------------- (the world panel)

        public override void OnChanged()
        {
            // Here we can update the game based on the new config values
        }
    }
}
