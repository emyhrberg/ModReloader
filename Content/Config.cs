using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SquidTestingMod.src
{
    public class Config : ModConfig
    {
        // CLIENT SIDE
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public override void OnChanged()
        {
            Config c = ModContent.GetInstance<Config>();

            if (c == null)
            {
                Mod.Logger.Info("CONFIG NULL!");
                return;
            }

            int type = ModContent.ItemType<BorderShaderDye>();

            if (c.GodModeOutlineSize == "Small")
            {
                GameShaders.Armor.BindShader<ArmorShaderData>(type, new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/LessOutlineEffect"), "Pass0"));
            }
            else
            {
                GameShaders.Armor.BindShader<ArmorShaderData>(type, new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/OutlineEffect"), "Pass0"));
            }
        }

        // ACTUAL CONFIG
        [Header("Autoload")]
        [OptionStrings(["None", "Singleplayer", "Multiplayer"])]
        [DefaultValue("None")]
        [DrawTicks]
        public string AutoloadWorld = "None";

        // REFRESH HEADER
        [Header("Refresh")]
        [DefaultValue(true)]
        public bool EnableRefreshButton;

        [DefaultValue(1000)]
        [Range(0, 5000)]
        public int WaitingTime;

        [DefaultValue(true)]
        public bool SaveWorld;

        [DefaultValue(false)]
        public bool InvokeBuildAndReload;

        // MISC HEADER
        [Header("Misc")]
        [DefaultValue(false)]
        public bool ShowHitboxes;

        [DefaultValue(false)]
        public bool StartInGodMode;

        [OptionStrings(["Small", "Big"])]
        [DefaultValue("Small")]
        [DrawTicks]
        public string GodModeOutlineSize = "Small";

        [DefaultValue(false)]
        public bool AlwaysSpawnBossOnTopOfPlayer;
    }
}
