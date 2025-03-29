using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper.Common.Players
{
    /// <summary>
    /// An empty item created only for its
    /// ItemID. All armor shaders require themselves
    /// to be bound to a specific item type.
    /// </summary>
    public class GodGlowDyeItem : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.ColorOnlyDye;

        public override void SetStaticDefaults()
        {
            if (!Conf.C.GodGlow)
            {
                return;
            }

            if (!Main.dedServ)
            {
                GameShaders.Armor.BindShader(Type, new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/OutlineEffect"), "Pass0"));
            }
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ColorOnlyDye);
        }
    }
}
