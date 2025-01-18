using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SkipSelect.MainCode
{
    public class Config : ModConfig
    {
        // Server-side scope because it affects gameplay and can be shared with others.
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Label("Efficiency Selection")]
        [Tooltip("Automatically loads the first player and world in singleplayer mode upon reloading mods.")]
        [DefaultValue(true)] // Default value is true.
        public bool EnableSingleplayer;

        [Tooltip("Automatically loads the first player and world in multiplayer mode upon reloading mods.")]
        [DefaultValue(false)] // Default value is true.
        public bool EnableMultiplayer;
    }
}
