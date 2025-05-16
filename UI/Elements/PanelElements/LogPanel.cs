using System;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using ModReloader.Helpers;

namespace ModReloader.UI.Elements.PanelElements
{
    public class LogPanel : BasePanel
    {
        public LogPanel() : base(header: "Log")
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
            AddPadding(20);

            AddHeader(title: "Log Level", hover: "Set the log level for each logger (0-5): Off, Error, Warn, Info, Debug, All");
            AddPadding(3);

            // Get all loggers and sort them
            ILog[] sortedLoggers = LogManager.GetCurrentLoggers()
                .OrderBy(log =>
                {
                    // Prioritize tML and your mod's name
                    if (log.Logger.Name == "tML") return 0;
                    if (log.Logger.Name.StartsWith(ModContent.GetInstance<ModReloader>().Name)) return 1;
                    return 2; // Other loggers
                })
                .ThenBy(log => log.Logger.Name) // Sort alphabetically for the rest
                .ToArray();

            // Add sliders for each logger
            foreach (var log in sortedLoggers)
            {
                // Truncate logger name if it's too long
                string loggerName = log.Logger.Name.Length > 13
                    ? log.Logger.Name.Substring(0, 10) + ".."
                    : log.Logger.Name;

                AddSlider(
                    title: loggerName,
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