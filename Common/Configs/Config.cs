using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SquidTestingMod.Common.Configs
{
    public class Config : ModConfig
    {
        // CLIENT SIDE
        public override ConfigScope Mode => ConfigScope.ClientSide;

        // RELOAD CONFIG
        [Header("Reload")]
        [DefaultValue("EnterYourModHere")]
        public string ModToReload = "EnterYourModHere";

        [DefaultValue(false)]
        public bool SaveWorldOnReload = false;

        [DefaultValue(false)]
        public bool ClearClientLogOnReload = false;

        // GAMEPLAY CONFIG
        [Header("Gameplay")]

        [DefaultValue(false)]
        public bool StartInGodMode;

        // ITEM SPAWNER CONFIG
        [Header("ItemNPCSpawner")]
        [DefaultValue(1000)]
        [Range(100, 6000)]
        [Increment(1000)]
        public int MaxItemsToDisplay;

        [Range(-500f, 500f)]
        [Increment(100f)]
        [DefaultValue(typeof(Vector2), "0, 0")]
        public Vector2 NPCSpawnLocation;

        public override void OnChanged()
        {
            // Here we can update the game based on the new config values
        }
    }
}
