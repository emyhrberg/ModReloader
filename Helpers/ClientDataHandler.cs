using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Terraria;

namespace SquidTestingMod.Helpers
{
    public enum ClientMode
    {
        FreshClient, // Client that just started
        SinglePlayer, // Client that is in singleplayer
        MPMain, // Client that is in multiplayer and is main
        MPMinor, // Client that is in multiplayer and is not main
    }

    public class ClientDataJson
    {
        // This is needed to serialize enums as strings
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ClientMode ClientMode { get; set; }
        public int PlayerID { get; set; }
        public int WorldID { get; set; }
    }

    public static class ClientDataHandler
    {
        //Function that handles writing info that shoud survive modlreload
        public static ClientMode ClientMode = ClientMode.FreshClient;
        public static int PlayerID = 0;
        public static int WorldID = 0;

        private static string GetFolderPath()
        {
            try
            {
                string path = Path.Combine(Main.SavePath, "SquidTestingMod");
                Directory.CreateDirectory(path); // Ensure the directory exists
                path = Path.Combine(path, "SquidTestingMod.json");
                Log.Info("Found save path: " + path);
                return path;
            }
            catch (Exception ex)
            {
                Log.Error("Could not find part of the path: " + ex.Message);
                return null;
            }
        }

        public static void WriteData()
        {
            if (Main.dedServ)
                return;

            string path = GetFolderPath();
            if (path == null)
                return;

            Log.Info("Writing Data");

            // Use the strongly typed class:
            var data = new ClientDataJson
            {
                ClientMode = ClientMode,
                PlayerID = PlayerID,
                WorldID = WorldID
            };

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(data, jsonOptions);
            File.WriteAllText(path, jsonString);
        }

        public static void ReadData()
        {
            if (Main.dedServ)
                return;

            Log.Info("Reading Data");
            string path = GetFolderPath();
            if (path == null)
                return;

            try
            {
                string jsonString = File.ReadAllText(path);
                var data = JsonSerializer.Deserialize<ClientDataJson>(jsonString);
                if (data != null)
                {
                    ClientMode = data.ClientMode;
                    PlayerID = data.PlayerID;
                    WorldID = data.WorldID;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error reading or deserializing data: " + ex.Message);
            }

        }
    }
}
