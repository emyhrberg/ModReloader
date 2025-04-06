using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using Terraria;

namespace ModHelper.Helpers
{
    public static class ClientDataHandler
    {
        //Function that handles writing info that shoud survive modlreload

        private static bool flagIsReaded = false; // Used to check if the values are readed

        public static ClientMode ClientMode
        {
            get
            {
                CheckIfNeedsToReadValues();
                return field;
            }
            set;
        } = ClientMode.FreshClient;
        public static int PlayerID // default to invalid player ID
        {
            get
            {
                CheckIfNeedsToReadValues();
                return field;
            }
            set;
        } = -1;
        public static int WorldID // default to invalid world ID
        {
            get
            {
                CheckIfNeedsToReadValues();
                return field;
            }
            set;
        } = -1;

        private static void CheckIfNeedsToReadValues()
        {
            if (!flagIsReaded)
            {
                ReadData();
                flagIsReaded = true;
            }
        }

        private static string GetFolderPath()
        {
            try
            {
                string path = Path.Combine(Main.SavePath, "ModHelper");
                Directory.CreateDirectory(path); // Ensure the directory exists
                path = Path.Combine(path, "ClientDataHandler.json");
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

            Log.Info("Writing ClientData");

            LockingFile(path, (reader, writer) =>
            {

                var listJson = GetListFromJson(reader);

                var data = new ClientDataJson
                {
                    ProcessID = Utilities.ProcessID,
                    ClientMode = ClientMode,
                    PlayerID = PlayerID,
                    WorldID = WorldID
                };

                var existingData = listJson.FirstOrDefault(d => d.ProcessID == Utilities.ProcessID);

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

                WriteListToFile(listJson, writer);
            });
        }

        private static void WriteListToFile(List<ClientDataJson> listJson, StreamWriter writer)
        {
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(listJson, jsonOptions);
            writer.BaseStream.SetLength(0);  // Clears the file
            writer.BaseStream.Seek(0, SeekOrigin.Begin); // Move to start
            writer.Write(jsonString);
            writer.Flush();
        }

        public static void ReadData()
        {
            if (Main.dedServ)
                return;

            string path = GetFolderPath();
            if (path == null)
                return;

            Log.Info("Reading ClientData");

            LockingFile(path, (reader, writer) =>
            {
                var listJson = GetListFromJson(reader);

                var index = listJson.Select((data, index) => new { data, index = index + 1 })
                .Where(pair => (pair.data.ProcessID == Utilities.ProcessID))
                .Select((pair) => pair.index)
                .FirstOrDefault() - 1;

                if (index >= 0)
                {
                    ClientMode = listJson[index].ClientMode;
                    PlayerID = listJson[index].PlayerID;
                    WorldID = listJson[index].WorldID;
                    // Clear data after reading it
                    listJson.RemoveAt(index);
                }

                WriteListToFile(listJson, writer);
            });
        }

        private static List<ClientDataJson> GetListFromJson(StreamReader reader)
        {
            List<ClientDataJson> listJson = new List<ClientDataJson>();
            string jsonString = reader.ReadToEnd();

            Log.Info($"Reading from file: " + jsonString);

            if (string.IsNullOrEmpty(jsonString))
            {
                listJson = [];
            }
            else
            {
                listJson = JsonSerializer.Deserialize<List<ClientDataJson>>(jsonString);
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
                        Log.Info($"File {Path.GetFileName(filePath)} is locked by {Utilities.ProcessID} process. Editing...");
                        fs.Seek(0, SeekOrigin.Begin);
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);

                        action?.Invoke(reader, writer);

                        Log.Info($"File {Path.GetFileName(filePath)} editing complete by {Utilities.ProcessID} process");
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
