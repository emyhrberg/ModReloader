using System.Diagnostics;
using Terraria;

namespace EliteTestingMod.Helpers
{
    //Class basically for universal helping functions
    internal class Utilities
    {
        public static int ProccesId => Process.GetCurrentProcess().Id;

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
