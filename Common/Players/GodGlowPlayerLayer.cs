using Microsoft.Xna.Framework;
using ModHelper.Common.Configs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

/// <summary>
/// Draws/adds the glow image thing behind the player
/// </summary>
namespace ModHelper.Common.Players
{
    public class PlayerGlowEffectLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => PlayerDrawLayers.BeforeFirstVanillaLayer;
        public override bool IsHeadLayer => false;

        protected override void Draw(ref PlayerDrawSet _set)
        {
            if (Main.gameMenu)
                return;

            if (!Conf.C.GodGlow)
                return;

            if (Main.LocalPlayer.whoAmI != _set.drawPlayer.whoAmI)
                return;

            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            if (!p.GetGod())
                return;

            if (GodGlow.ready)
            {
                // draw the draw data 4 times, but shifted in different
                // directions for each. This gives it a softer
                // gradiant and fills in more holes
                GodGlow.sData.position = _set.Position - Main.screenPosition + (_set.drawPlayer.Size / 2).Floor();
                DrawData d0, d1, d2, d3;
                d0 = d1 = d2 = d3 = GodGlow.sData;
                d0.position += Vector2.UnitY * 3;
                d1.position += Vector2.UnitY * -3;
                d2.position += Vector2.UnitX * 3;
                d3.position += Vector2.UnitY * -3;
                _set.DrawDataCache.Add(d0);
                _set.DrawDataCache.Add(d1);
                _set.DrawDataCache.Add(d2);
                _set.DrawDataCache.Add(d3);
            }
        }
    }
}
