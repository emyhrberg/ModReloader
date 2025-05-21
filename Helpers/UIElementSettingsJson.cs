using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModReloader.Helpers
{
    public static class UIElementSettingsJson
    {
        private static readonly string fileName = "UIElementSettings.json";
        private static JObject _elementSettings = [];

        /// <summary> Initialize element settings levels from file </summary>
        public static void Initialize()
        {
            ReadElementSettingsFromFile();
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
            if (_elementSettings.TryGetValue(settingName, out JToken value))
            {
                if (TryConvertToken(value, out T castedValue))
                {
                    result = castedValue;
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

        private static void ReadElementSettingsFromFile()
        {
            _elementSettings = [];
            string filePath = Utilities.GetModReloaderFolderPath(fileName);
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var jObject = JsonConvert.DeserializeObject<JObject>(json);
                    if (jObject != null)
                    {
                        _elementSettings = jObject;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read element settings: {ex.Message}");
            }
        }

        public static void WriteValue<T>(string settingName, T value)
        {
            _elementSettings[settingName] = JToken.FromObject(value);
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

        public static bool TryConvertToken<T>(JToken token, out T result)
        {
            result = default;

            if (token == null || token.Type == JTokenType.Null)
                return false;
            try
            {
                JValue temp = (JValue)token;
                result = (T)Convert.ChangeType(temp, typeof(T));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}