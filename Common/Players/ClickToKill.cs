

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class ClickToKill : ModSystem
    {
        public override void PostUpdateInput()
        {
            if (Main.dedServ) return; // Don't run on a dedicated server

            if (PlayerCheatManager.ClickToKill)
            {
                // Ensure the mouse click was released (prevents rapid-fire detection)
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Vector2 mousePosition = Main.MouseWorld;

                    foreach (NPC npc in Main.npc)
                    {
                        if (npc.active && npc.Hitbox.Contains(mousePosition.ToPoint()))
                        {
                            Main.NewText($"You clicked on {npc.FullName}!", Color.LightGreen);
                            npc.StrikeInstantKill();
                            break; // Stop checking once a clicked NPC is found
                        }
                    }
                }
            }
        }
    }
}

