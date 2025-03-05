using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Players;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class Invisible : ModPlayer
    {
        public override void PostUpdate()
        {
            // Log.Slow("agro: " + Player.aggro);

            if (PlayerCheatManager.InvisibleToEnemies)
            {
                // Make the player "invisible"-ish but not really to flying enemies 
                // try to avoid making enemies target the player
                // todo make it better
                // agro
                Player.aggro = -9999;
            }
        }
    }
}
