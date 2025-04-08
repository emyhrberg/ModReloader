using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Terraria;

namespace ModHelper.Helpers
{
    public static class ClientDataJsonHelper
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

        public static void WriteData()
        {
            if (Main.dedServ)
                return;

            string path = Utilities.GetModHelperFolderPath("ClientDataHandler.json");
            if (path == null)
                return;

            Log.Info("Writing ClientData");
            Utilities.LockingFile(path, (reader, writer) =>
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

            string path = Utilities.GetModHelperFolderPath("ClientDataHandler.json");
            if (path == null)
                return;

            Log.Info("Reading ClientData");
            Utilities.LockingFile(path, (reader, writer) =>
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
    }
}
