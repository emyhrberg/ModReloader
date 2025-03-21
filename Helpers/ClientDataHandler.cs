using System;
using System.IO;
using System.Text.Json;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Helpers
{
    public enum ClientMode
    {
        FreshClient, // Client that just started
        SinglePlayer, // Client that is in singleplayer
        MPMain, // Client that is in multiplayer and is main
        MPMinor, // Client that is in multiplayer and is not main
    }

    internal static class ClientDataHandler
    {
        //Function that handles writing info that shoud survive modlreload
        static int _mode = (int)ClientMode.FreshClient;
        static int _playerId = 0;
        static int _worldId = 0;

        public static ClientMode Mode { get { return (ClientMode)_mode; } set { _mode = (int)value; } }
        public static int PlayerId { get { return _playerId; } set { _playerId = value; } }
        public static int WorldId { get { return _worldId; } set { _worldId = value; } }
        public static void WriteData()
        {
            if (Main.dedServ)
                return;

            Log.Info("Writing Data");
            Main.instance.Window.Title = $"{_mode}, {_playerId}, {_worldId}";

            string path = Path.Combine(Main.SavePath, "SquidTestingMod");
            Directory.CreateDirectory(path);
            path = Path.Combine(path, "SquidTestingMod.json");

            // Serialize to json
            // new array for players with entry of array: Mode, playerId, worldId
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(new int[] { _mode, _playerId, _worldId }, options);
            File.WriteAllText(path, jsonString);
        }

        public static void ReadData()
        {
            if (Main.dedServ)
                return;

            Log.Info("Reading Data");
            string path = Path.Combine(Main.SavePath, "SquidTestingMod");
            try
            {
                Directory.CreateDirectory(path); // Ensure the directory exists
                path = Path.Combine(path, "SquidTestingMod.json");
                Log.Info("save path: " + path);
            }
            catch (Exception ex)
            {
                Log.Error("Could not find part of the path: " + ex.Message);
                return;
            }

            // Read from json
            // Set the values of the mode, playerId, and worldId
            int mode = JsonSerializer.Deserialize<int[]>(File.ReadAllText(path))[0];
            int playerId = JsonSerializer.Deserialize<int[]>(File.ReadAllText(path))[1];
            int worldId = JsonSerializer.Deserialize<int[]>(File.ReadAllText(path))[2];

            Log.Info($"Mode: {mode}, PlayerId: {playerId}, WorldId: {worldId}");
            _mode = mode;
            _playerId = playerId;
            _worldId = worldId;
        }
    }
}
