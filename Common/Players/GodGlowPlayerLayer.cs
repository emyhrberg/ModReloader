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

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;

            // Make sure this works for any player with god mode
            if (!player.active || Main.gameMenu || !Conf.C.GodGlow)
                return;

            PlayerCheatManager playerCheats = player.GetModPlayer<PlayerCheatManager>();
            if (!playerCheats.GetGod())
                return;

            if (!GodGlow.ready)
                return;

            // Create a pulsating effect
            float pulse = 0.2f * (float)System.Math.Sin(Main.GameUpdateCount * 0.05f) + 1f;
            Color glowColor = Color.Gold * (0.6f + 0.4f * pulse * 0.5f);

            // Position at player center
            Vector2 position = drawInfo.Position + player.Size / 2f - Main.screenPosition;

            // Draw the glow
            DrawData glow = GodGlow.sData;
            glow.position = position;
            glow.color = glowColor;
            glow.scale = new Vector2(1.5f * pulse, 1.5f * pulse);

            drawInfo.DrawDataCache.Add(glow);
        }
    }
}