using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Players
{
    public class PlayerLightHack : GlobalWall
    {
        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            if (p.GetLightMode())
            {
                r = MathHelper.Clamp(r + 0.5f, 0, 1);
                g = MathHelper.Clamp(g + 0.5f, 0, 1);
                b = MathHelper.Clamp(b + 0.5f, 0, 1);
            }
        }
    }
}
