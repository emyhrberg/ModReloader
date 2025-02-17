using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SquidTestingMod.Helpers
{
    internal static class ClientDataHandler
    {
        //Function that handles writing info that shoud survive modlreload
        public static void WriteData(int playerId, int worldId, bool shoudServerBeOpened)
        {
            Main.instance.Window.Title = $"{playerId}, {worldId}, {(shoudServerBeOpened ? 1 : 0)}";
        }

        public static string[] ReadDataRaw()
        {
            return Main.instance.Window.Title.Split(", ");
        }

        public static int ReadDataPlayerId()
        {
            return Convert.ToInt32(ReadDataRaw()[0]);
        }

        public static int ReadDataWorldId()
        {
            return Convert.ToInt32(ReadDataRaw()[1]);
        }

        public static bool ReadDataShoudServerBeOpened()
        {
            return Convert.ToInt32(ReadDataRaw()[2]) == 1;
        }
    }
}
