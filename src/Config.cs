using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SquidTestingMod.src
{
    public class Config : ModConfig
    {
        // CLIENT SIDE
        public override ConfigScope Mode => ConfigScope.ClientSide;

        // ACTUAL CONFIG
        [Header("Autoload")]
        [OptionStrings(["None", "Singleplayer", "Multiplayer"])]
        [DefaultValue("None")]
        [DrawTicks]
        public string AutoloadWorld = "None";

        [Header("Refresh")]
        [DefaultValue(true)]
        public bool EnableRefreshButton;

        [DefaultValue(1000)]
        [Range(0, 5000)]
        public int WaitingTime;

        [DefaultValue(true)]
        public bool SaveWorld;

        [Header("Misc")]
        [DefaultValue(false)]
        public bool ShowHitboxes;

        [DefaultValue(false)]
        public bool StartInGodMode;

        [DefaultValue(false)]
        public bool AlwaysSpawnBossOnTopOfPlayer;

    }
}
