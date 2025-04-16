using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ModHelper.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("ModHelper")]

        // [DrawTicks]
        // [OptionStrings(["Default", "Publicizer"])]
        // [DefaultValue("Default")]
        // public string ReloadMode { get; set; } = "Default";

        [DefaultValue(true)]
        public bool AutoJoin;

        [DefaultValue(true)]
        public bool SaveWorld;
    }

    public static class Conf
    {
        public static Config C => ModContent.GetInstance<Config>();
    }
}