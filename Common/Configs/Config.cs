using System.ComponentModel;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Systems;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SquidTestingMod.Common.Configs
{
    public class Config : ModConfig
    {
        // CLIENT SIDE
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public override void OnChanged()
        {
            // add null check for the class itself in case it's called before the mod is loaded
            // idk
            if (ModContent.GetInstance<Config>() == null)
                return;

            int type = ModContent.ItemType<BorderShaderDye>();

            if (GodModeOutlineSize == "Small")
            {
                Asset<Effect> smallOutlineEffect = Mod.Assets.Request<Effect>("Effects/LessOutlineEffect");
                GameShaders.Armor.BindShader(type, new ArmorShaderData(smallOutlineEffect, "Pass0"));
            }
            else if (GodModeOutlineSize == "Big")
            {
                Asset<Effect> bigOutlineEffect = Mod.Assets.Request<Effect>("Effects/OutlineEffect");
                GameShaders.Armor.BindShader(type, new ArmorShaderData(bigOutlineEffect, "Pass0"));
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

        [DefaultValue(false)]
        public bool SaveWorld;

        [DefaultValue(false)]
        public bool InvokeBuildAndReload;

        [DefaultValue("SquidTestingMod")]
        public string ModToReload;

        [DefaultValue(0)]
        [Range(0, 5000)]
        public int WaitingTimeBeforeNavigatingToModSources;

        [DefaultValue(1000)]
        [Range(0, 5000)]
        public int WaitingTimeBeforeBuildAndReload;

        // MISC HEADER
        [Header("UI")]
        [DefaultValue(false)]
        public bool ShowHitboxes;

        [Header("Gameplay")]

        [DefaultValue(false)]
        public bool AlwaysSpawnBossOnTopOfPlayer;

        [Header("GodMode")]
        [DefaultValue(false)]
        public bool StartInGodMode;

        [OptionStrings(["Small", "Big"])]
        [DefaultValue("Small")]
        [DrawTicks]
        public string GodModeOutlineSize = "Small";
    }
}
