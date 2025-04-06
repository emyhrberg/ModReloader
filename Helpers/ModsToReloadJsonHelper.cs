using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;

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
            string filePath = Path.Combine(Main.SavePath, "ModHelper", "ModsToReload.json");
            try
            {
                string json = JsonConvert.SerializeObject(modsToReload, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write ModsToReload to file: {ex.Message}");
            }
        }

        public static List<string> ReadModsToReload()
        {
            string filePath = Path.Combine(Main.SavePath, "ModHelper", "ModsToReload.json");
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    List<string> data = JsonConvert.DeserializeObject<List<string>>(json);
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
