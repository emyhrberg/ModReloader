using SquidTestingMod.Common.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Helpers
{
    //Class basically for universal helping functions
    internal class Utilities
    {
        public static int FindPlayerId()
        {
            Main.LoadPlayers();
            var playerId = Main.PlayerList.FindIndex(p => p.Path == Main.ActivePlayerFileData.Path);
            return playerId;
        }

        public static int FindWorldId()
        {
            Main.LoadWorlds();
            var worldId = Main.WorldList.FindIndex(w => w.Path == Main.ActiveWorldFileData.Path);
            return worldId;
        }
    }
}
