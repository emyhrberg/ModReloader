using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ModHelper.Common.Players
{
    public class PlayerGlowEffectLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);
        public override bool IsHeadLayer => false;

        protected override void Draw(ref PlayerDrawSet _set)
        {
            Player player = _set.drawPlayer;

            // Make sure this works for any player with god mode
            if (!player.active || Main.gameMenu || !Conf.C.GodGlow)
                return;

            PlayerCheatManager playerCheats = player.GetModPlayer<PlayerCheatManager>();
            if (!playerCheats.GetGod())
                return;

            if (!GodGlow.ready)
                return;

            // Set a constant glow color
            Color glowColor = Color.Gold * 0.8f;

            // Position at player center
            Vector2 position = _set.Position + player.Size / 2f - Main.screenPosition;

            // Draw the glow
            DrawData glow = GodGlow.sData;
            glow.position = position;
            glow.color = glowColor;

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
            // glow.scale = new Vector2(1.5f, 1.5f);

            _set.DrawDataCache.Add(glow);
        }
    }
}