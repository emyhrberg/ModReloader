using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ModReloader.Helpers
{
    public static class UIElementSettingsJson
    {
        private static readonly string fileName = "UIElementSettings.json";
        private static Dictionary<string, object> _elementSettings = [];

        /// <summary> Initialize element settings levels from file </summary>
        public static void Initialize()
        {
            _elementSettings = ReadElementSettingsFromFile();
        }

        public static T TryGetValue<T>(string settingName, T defaultValue)
        {
            if (TryGetValue(settingName, out T result))
            {
                    return result;
            }
            else
            {
                return defaultValue;
            }
        }

        public static bool TryGetValue<T>(string settingName, out T result)
        {
            if (_elementSettings.TryGetValue(settingName, out object value))
            {
                if(value is T cast)
                {
                    result = (T)value;
                    return true;
                }
                result = default;
                return false;
            }
            else
            {
                result = default;
                return false;
            }
            
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
                        new JsonSerializerSettings { }
                    ) ?? [];
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read element settings: {ex.Message}");
            }
            return [];
        }

        public static void WriteValue<T>(string settingName, T value)
        {
            _elementSettings[settingName] = value;
        }

        public static void Save()
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