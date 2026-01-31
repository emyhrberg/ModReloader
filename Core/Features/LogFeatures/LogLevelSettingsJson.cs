using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ModReloader.Core.Features.LogFeatures
{
    public class LogLevelSystem : ModSystem
    {
        public override void Load()
        {
            base.Load();

            LogLevelSettingsJson.Initialize();
        }
    }

    public static class LogLevelSettingsJson
    {
        private static readonly string fileName = "LogLevels.json";
        private static Dictionary<string, string> _logLevels = [];

        /// <summary>
        /// Initialize log levels from file
        /// </summary>
        public static void Initialize()
        {
            _logLevels = ReadLogLevelsFromFile();
        }

        /// <summary>
        /// Update a logger's level and save to file
        /// </summary>
        public static void UpdateLogLevel(string loggerName, string level)
        {
            _logLevels[loggerName] = level;
            WriteLogLevelsToFile();
        }

        /// <summary>
        /// Read log levels from the JSON file
        /// </summary>
        public static Dictionary<string, string> ReadLogLevels()
        {
            return new Dictionary<string, string>(_logLevels);
        }

        private static Dictionary<string, string> ReadLogLevelsFromFile()
        {
            string filePath = Utilities.GetModReloaderFolderPath(fileName);
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                        ?? [];
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read log levels: {ex.Message}");
            }
            return new Dictionary<string, string>();
        }

        private static void WriteLogLevelsToFile()
        {
            string filePath = Utilities.GetModReloaderFolderPath(fileName);
            try
            {
                string json = JsonConvert.SerializeObject(_logLevels, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save log levels: {ex.Message}");
            }
        }
    }
}