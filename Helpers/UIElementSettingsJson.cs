using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ModReloader.Helpers
{
    public class UIElementSettingsJsonSystem : ModSystem
    {
        public override void Load()
        {
            base.Load();

            UIElementSettingsJson.Initialize();
        }
    }

    public static class UIElementSettingsJson
    {
        private static readonly string fileName = "UIElementSettings.json";
        private static Dictionary<string, object> _elementSettings = [];

        /// <summary> Initialize element settings levels from file </summary>
        public static void Initialize()
        {
            _elementSettings = ReadElementSettingsFromFile();
        }

        /// <summary> Update a setting's value and save to file </summary>
        public static void UpdateElementValue(string settingName, object value)
        {
            _elementSettings[settingName] = value;
            WriteElementSettingsToFile();
        }

        /// <summary> Read log levels from the JSON file </summary>
        public static Dictionary<string, object> ReadElementSettings()
        {
            return new Dictionary<string, object>(_elementSettings);
        }

        private static Dictionary<string, object> ReadElementSettingsFromFile()
        {
            string filePath = Utilities.GetModReloaderFolderPath(fileName);
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(
                        json,
                        new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Decimal }
                    ) ?? [];
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read element settings: {ex.Message}");
            }
            return [];
        }

        private static void WriteElementSettingsToFile()
        {
            string filePath = Utilities.GetModReloaderFolderPath(fileName);
            try
            {
                string json = JsonConvert.SerializeObject(_elementSettings, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save element settings: {ex.Message}");
            }
        }
    }
}