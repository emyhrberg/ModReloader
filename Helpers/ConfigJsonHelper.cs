using System;
using System.IO;
using Microsoft.Xna.Framework;
using ModHelper.Helpers;
using Newtonsoft.Json;
using Terraria;

namespace ModHelper.Helpers
{
    public static class ConfigJsonHelper
    {
        /// <summary>
        /// A helper class to manage configuration settings for the mod.
        /// Writes a json file of some settings to the mod folder.
        /// Reads the json file to apply some settings to the mod.
        /// Example: Conf.C.ButtonsPosition
        /// </summary>

        public static void WriteButtonsPosition(Vector2 buttonsPosition)
        {
            string filePath = Path.Combine(Main.SavePath, "ModHelper", "ButtonsPosition.json");
            try
            {
                string json = JsonConvert.SerializeObject(buttonsPosition, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write to config file: {ex.Message}");
            }
        }

        public static Vector2 ReadButtonsPosition()
        {
            string filePath = Path.Combine(Main.SavePath, "ModHelper", "ButtonsPosition.json");
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    Vector2 data = JsonConvert.DeserializeObject<Vector2>(json);
                    // Use default if deserialization returns zero vector (or you could check here further)
                    if (data == default)
                    {
                        return new Vector2(0.5f, 1.0f);
                    }
                    return data;
                }
                else
                {
                    Log.Error("Config file not found.");
                    return new Vector2(0.5f, 1.0f);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read from config file: {ex.Message}");
                return new Vector2(0.5f, 1.0f);
            }
        }
    }
}
