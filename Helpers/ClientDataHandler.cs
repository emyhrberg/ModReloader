using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.IO;
using static XPT.Core.Audio.MP3Sharp.Decoding.Decoder;

namespace SquidTestingMod.Helpers
{


    internal static class ClientDataHandler
    {
        //Function that handles writing info that shoud survive modlreload
        static int _mode = (int)ClientModes.FreshClient;
        static int _playerId = 0;
        static int _worldId = 0;

        //static Dictionary<int, ClientData> ClientsData;

        public static ClientModes Mode { get { return (ClientModes)_mode; } set { _mode = ((int)value); } }
        public static int PlayerId { get { return _playerId; } set { _playerId = value; } }
        public static int WorldId { get { return _worldId; } set { _worldId = value; } }
        public static void WriteData()
        {
            if (Main.dedServ)
            {
                return;
            }

            Log.Info("Writing Data");
            Main.instance.Window.Title = $"{_mode}, {_playerId}, {_worldId}";
        }

        public static void ReadData()
        {
            if (Main.dedServ)
            {
                return;
            }

            Log.Info("Reading Data");
            if (!string.IsNullOrEmpty(Main.instance.Window.Title))
            {
                string[] list = Main.instance.Window.Title.Split(", ");
                Array.Resize(ref list, 3);

                bool succesfulParsing = true;
                succesfulParsing &= int.TryParse(list[0], out _mode);
                succesfulParsing &= int.TryParse(list[1], out _playerId);
                succesfulParsing &= int.TryParse(list[2], out _worldId);

                if (!succesfulParsing)
                {
                    Log.Error("Unable to parce client data from title");
                }


            }
            Main.instance.Window.Title = "SquidTestingMod: this is hard";
        }
    }
}
