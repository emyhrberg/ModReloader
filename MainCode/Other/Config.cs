using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SkipSelect.MainCode.Other
{
    public class Config : ModConfig
    {
        // Server-side scope because it affects gameplay and can be shared with others.
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // -------------------------------------------------------------
        [Header("SP/MP")]
        [Label("Efficiency Selection")]
        [Tooltip("Automatically loads the first player and world in singleplayer mode upon reloading mods.")]
        [DefaultValue(true)] // Default value is true.
        public bool EnableSingleplayer;

        [Tooltip("Note: Does NOTHING. Its too hard to create a server programmatically rn.")]
        [DefaultValue(false)] // Default value is true.
        public bool EnableMultiplayer;

        // -------------------------------------------------------------
        [Header("Refresh")]
        [Tooltip("Enables a big refresh button to quickly navigate back to mods.")]
        [DefaultValue(true)] // Default value is true.
        public bool EnableRefresh;

        [Tooltip("Set the waiting time before navigating, recommend 1000ms.")]
        [DefaultValue(1000)] // Default value is true.
        [Range(0,5000)] // we have to change this to 5000 or something, otherwise default is max 100
        public int WaitingTime;

        [Tooltip("Save world before going to mod menu.")]
        [DefaultValue(true)] // Default value is true.
        public bool SaveWorld;

        // -------------------------------------------------------------
    }
}
