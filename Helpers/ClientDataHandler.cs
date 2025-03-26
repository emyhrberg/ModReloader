using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Terraria;
using Terraria.WorldBuilding;

namespace ErkysModdingUtilities.Helpers
{


    public class ClientDataJson
    {
        public int ProccesID { get; set; }
        // This is needed to serialize enums as strings
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ClientModes ClientMode { get; set; }
        public int PlayerID { get; set; }
        public int WorldID { get; set; }
    }

    public static class ClientDataHandler
    {
        //Function that handles writing info that shoud survive modlreload
        public static ClientModes ClientMode = ClientModes.FreshClient;
        public static int PlayerID = 0;
        public static int WorldID = 0;

        private static string GetFolderPath()
        {
            try
            {
                string path = Path.Combine(Main.SavePath, "ErkysModdingUtilities");
                Directory.CreateDirectory(path); // Ensure the directory exists
                path = Path.Combine(path, "ErkysModdingUtilities.json");
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

            var listJson = GetListFromJson();

            LockingFile(path, (reader, writer) =>
            {
                Log.Info("Writing Data");

                // Use the strongly typed class:
                var data = new ClientDataJson
                {
                    ProccesID = Utilities.ProccesId,
                    ClientMode = ClientMode,
                    PlayerID = PlayerID,
                    WorldID = WorldID
                };

                var existingData = listJson.FirstOrDefault(d => d.ProccesID == Utilities.ProccesId);

                if (existingData != null)
                {
                    // Update the existing entry
                    existingData.ClientMode = data.ClientMode;
                    existingData.PlayerID = data.PlayerID;
                    existingData.WorldID = data.WorldID;
                }
                else
                {
                    // Add a new entry
                    listJson.Add(data);
                }


                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(listJson, jsonOptions);
                writer.BaseStream.SetLength(0);  // Clears the file
                writer.BaseStream.Seek(0, SeekOrigin.Begin); // Move to start
                writer.Write(jsonString);
                writer.Flush();
            });


        }

        public static void ReadData()
        {
            if (Main.dedServ)
                return;

            Log.Info("Reading Data");
            var listJson = GetListFromJson();
            foreach (var data in listJson)
            {
                if (data.ProccesID == Utilities.ProccesId)
                {
                    ClientMode = data.ClientMode;
                    PlayerID = data.PlayerID;
                    WorldID = data.WorldID;
                    break;
                }
            }

        }

        private static List<ClientDataJson> GetListFromJson()
        {
            List<ClientDataJson> listJson = new List<ClientDataJson>();

            string path = GetFolderPath();
            if (path == null)
                return listJson;

            try
            {
                string jsonString = null;
                LockingFile(path, (reader, writer) =>
                {
                    jsonString = reader.ReadToEnd();
                });
                if (string.IsNullOrEmpty(jsonString))
                {
                    listJson = new();
                }
                else
                {
                    listJson = JsonSerializer.Deserialize<List<ClientDataJson>>(jsonString);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error reading or deserializing data: " + ex.Message);
            }
            return listJson;
        }

        private static void LockingFile(string filePath, Action<StreamReader, StreamWriter> action)
        {
            int retryDelay = 200; // 200ms delay between retries
            int maxAttempts = 20; // Maximum retries before giving up
            int attempts = 0;

            while (true)
            {
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    using (StreamReader reader = new StreamReader(fs, new UTF8Encoding(false)))
                    using (StreamWriter writer = new StreamWriter(fs, new UTF8Encoding(false)))
                    {
                        Log.Info($"File {Path.GetFileName(filePath)} is locked by {Utilities.ProccesId} process. Editing...");
                        fs.Seek(0, SeekOrigin.Begin);
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);

                        action?.Invoke(reader, writer);

                        Log.Info($"File {Path.GetFileName(filePath)} editing complete by {Utilities.ProccesId} process");
                        break;
                    }
                }
                catch (IOException)
                {
                    attempts++;
                    if (attempts >= maxAttempts)
                    {
                        Log.Info("Timeout: Unable to access file.");
                        break;
                    }

                    Log.Info($"File is in use. Retrying {attempts} time...");
                    Thread.Sleep(retryDelay); // Wait before retrying
                }
            }
        }

    }
}
