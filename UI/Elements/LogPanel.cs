using System;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ModHelper.UI.Elements.AbstractElements;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper.UI.Elements
{
    public class LogPanel : OptionPanel
    {
        public LogPanel() : base(title: "Log", scrollbarEnabled: true)
        {
            AddPadding(5);
            AddHeader(title: "Log",
                onLeftClick: Log.OpenLogFolder,
                hover: "Click to open the folder at Steam/steamapps/common/tModLoader/tModLoader-Logs");

            AddPadding(3f);
            string clientPath = Path.GetFileName(Logging.LogPath);
            AddAction(Log.OpenClientLog, "Open Client Log", $"Click to open {clientPath}");
            AddAction(Log.OpenServerLog, "Open Server Log", "Click to open server.log");
            AddAction(Log.ClearClientLog, "Clear Log", $"Clear the {clientPath} file");
            AddPadding();

            AddHeader(title: "Game Path",
                onLeftClick: Log.OpenEnabledJsonFolder,
                hover: "Click to open the folder at Documents/My Games/Terraria/ModLoader");
            ActionOption openEnabled = new(Log.OpenEnabledJson, "Open enabled.json", "This is a json file that shows a list of all your currently enabled mods", Log.OpenEnabledJsonFolder);
            AddPadding(5);
            uiList.Add(openEnabled);
            AddPadding();

            AddHeader(title: "Log Level", hover: "Set the log level for each logger (0-5): Off, Error, Warn, Info, Debug, All");

            // add all sliders for all loggers
            foreach (var log in LogManager.GetCurrentLoggers().OrderBy(l => l.Logger.Name))
            {
                AddSlider(
                    title: log.Logger.Name,
                    min: 0,
                    max: 5,
                    defaultValue: 5,
                    onValueChanged: (value) => SetLogLevel(value, log.Logger as Logger),
                    increment: 1,
                    textSize: 0.8f,
                    hover: $"Set the log level for {log.Logger.Name}",
                    valueFormatter: (value) => ((LogLevel)value).ToString()
                );
            }
        }

        [Flags]
        public enum LogLevel
        {
            Off = 0,
            Error = 1,
            Warn = 2,
            Info = 3,
            Debug = 4,
            All = 5
        }

        private void SetLogLevel(float value, Logger logger)
        {
            if (logger == null)
                return;

            // Convert value from 0-5 to LogLevel enum
            LogLevel level = (LogLevel)value;

            logger.Level = level switch
            {
                LogLevel.Error => Level.Error,
                LogLevel.Warn => Level.Warn,
                LogLevel.Info => Level.Info,
                LogLevel.Debug => Level.Debug,
                LogLevel.All => Level.All,
                _ => Level.Off
            };
        }

        private static Logger GetLogger(string loggerName = "tML")
        {
            PropertyInfo tmlProp = typeof(Logging).GetProperty(loggerName, BindingFlags.Static | BindingFlags.NonPublic);
            if (tmlProp == null)
                return null;

            ILog tmlLogger = (ILog)tmlProp.GetValue(null);
            if (tmlLogger == null)
                return null;

            return tmlLogger.Logger as Logger;
        }
    }
}