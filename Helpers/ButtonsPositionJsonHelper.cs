using System;
using System.IO;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace ModHelper.Helpers
{
    public static class ButtonsPositionJsonHelper
    {
        /// <summary>
        /// A helper class to manage configuration settings for the mod.
        /// Writes a json file of some settings to the mod folder.
        /// Reads the json file to apply some settings to the mod.
        /// Example: Conf.C.ButtonsPosition
        /// </summary>

        public static void WriteButtonsPosition(Vector2 buttonsPosition)
        {
            string filePath = Utilities.GetModHelperFolderPath("ButtonsPosition.json");
            try
            {
                Utilities.LockingFile(filePath, (reader, writer) =>
                {
                    string json = JsonConvert.SerializeObject(buttonsPosition, Formatting.Indented);
                    writer.BaseStream.SetLength(0);  // Clears the file
                    writer.BaseStream.Seek(0, SeekOrigin.Begin); // Move to start
                    writer.Write(json);
                    writer.Flush();
                });
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write to config file: {ex.Message}");
            }
        }


        public static Vector2 ReadButtonsPosition()
        {
            string filePath = Utilities.GetModHelperFolderPath("ButtonsPosition.json");
            try
            {
                if (File.Exists(filePath))
                {
                    Vector2 data = default;
                    Utilities.LockingFile(filePath, (reader, writer) =>
                    {
                        string json = reader.ReadToEnd();
                        data = JsonConvert.DeserializeObject<Vector2>(json);
                    });
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
