using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SkipSelect.Core.System
{
    public class Config : ModConfig
    {
        // CLIENT SIDE
        public override ConfigScope Mode => ConfigScope.ClientSide;

        // -------------------------------------------------------------
        [Header("Singleplayer&Multiplayer")]
        [Label("Enable Singleplayer")]
        [Tooltip("Automatically loads the first player and world in singleplayer mode upon reloading mods.")]
        [DefaultValue(true)] // Default value is true.
        public bool EnableSingleplayer;

        [Label("Enable Multiplayer")]
        [Tooltip("Note: Does NOTHING. Its too hard to create a server programmatically rn.")]
        [DefaultValue(false)] // Default value is true.
        public bool EnableMultiplayer;

        // -------------------------------------------------------------
        [Header("Refresh")]

        [Label("Enable Refresh")]
        [Tooltip("Enables a big refresh button to quickly navigate back to mods.")]
        [DefaultValue(true)] // Default value is true.
        public bool EnableRefresh;

        [Tooltip("Set the waiting time before navigating, recommend 1000ms.")]
        [Label("Waiting Time")]
        [DefaultValue(1000)] // Default value is true.
        [Range(0, 5000)] // we have to change this to 5000 or something, otherwise default is max 100
        public int WaitingTime;

        [Label("Save World")]
        [Tooltip("Save world before going to mod menu.")]
        [DefaultValue(true)] // Default value is true.
        public bool SaveWorld;

        // -------------------------------------------------------------
        [Header("Misc")]

        [Label("Show Hitboxes")]
        [Tooltip("Show hitboxes of players and NPCs.")]
        [DefaultValue(false)] // Default value is true.
        public bool ShowHitboxes;
    }
}
