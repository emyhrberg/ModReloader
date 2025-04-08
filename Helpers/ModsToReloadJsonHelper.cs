using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ModHelper.Helpers
{
    public static class ModsToReloadJsonHelper
    {
        /// <summary>
        /// A helper class to manage configuration settings for the mod.
        /// Writes a json file of some settings to the mod folder.
        /// Reads the json file to apply some settings to the mod.
        /// Example: Conf.C.ButtonsPosition
        /// </summary>

        public static void WriteModsToReload(List<string> modsToReload)
        {
            // Remove duplicates before writing
            List<string> uniqueMods = modsToReload.Distinct().ToList();

            // If we removed duplicates, update the original list
            if (uniqueMods.Count != modsToReload.Count)
            {
                modsToReload.Clear();
                modsToReload.AddRange(uniqueMods);
                Log.Info($"Removed {modsToReload.Count - uniqueMods.Count} duplicate mod entries");
            }

            string filePath = Utilities.GetModHelperFolderPath("ModsToReload.json");
            try
            {
                Utilities.LockingFile(filePath, (reader, writer) =>
                {
                    string json = JsonConvert.SerializeObject(uniqueMods, Formatting.Indented);
                    writer.BaseStream.SetLength(0);  // Clears the file
                    writer.BaseStream.Seek(0, SeekOrigin.Begin); // Move to start
                    writer.Write(json);
                    writer.Flush();
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write ModsToReload to file: {ex.Message}");
            }
        }

        public static List<string> ReadModsToReload()
        {
            string filePath = Utilities.GetModHelperFolderPath("ModsToReload.json");
            try
            {
                if (File.Exists(filePath))
                {
                    List<string> data = default;
                    Utilities.LockingFile(filePath, (reader, writer) =>
                    {
                        string json = reader.ReadToEnd();
                        data = JsonConvert.DeserializeObject<List<string>>(json);
                    });
                    // Use default if deserialization returns zero vector (or you could check here further)
                    if (data == default)
                    {
                        return [];
                    }
                    return data;
                }
                else
                {
                    Log.Error("ModsToReload file not found.");
                    return [];
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read from ModsToReload file: {ex.Message}");
                return [];
            }
        }
    }
}
