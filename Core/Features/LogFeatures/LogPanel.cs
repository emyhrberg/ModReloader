using System;
using System.IO;
using System.Linq;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using ModReloader.UI.Shared;

namespace ModReloader.Core.Features.LogFeatures
{
    public class LogPanel : BasePanel
    {
        public LogPanel() : base(header: "Log")
        {
            AddPadding(5);
            AddHeader(
                title: Loc.Get("LogPanel.LogTitle"),
                hover: Loc.Get("LogPanel.LogTitleHover"),
                onLeftClick: Log.OpenLogFolder
            );

            AddPadding(3f);
            string clientPath = Path.GetFileName(Logging.LogPath);

            AddAction(Log.OpenClientLog, Loc.Get("LogPanel.OpenClientLog"), Loc.Get("LogPanel.OpenClientLogHover", clientPath));
            AddAction(Log.OpenServerLog, Loc.Get("LogPanel.OpenServerLog"), Loc.Get("LogPanel.OpenServerLogHover"));
            AddAction(Log.ClearClientLog, Loc.Get("LogPanel.ClearClientLog"), Loc.Get("LogPanel.ClearClientLogHover", clientPath));

            AddPadding(20);

            string headerHover = Loc.Get("LogPanel.LogLevelHeader");
            if (Conf.C.LogLevelPersistOnReloads)
                headerHover += "\n" + Loc.Get("LogPanel.LogLevelPersistOnReloads");

            AddHeader(title: Loc.Get("LogPanel.LogLevelTitle"), hover: headerHover);
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
                string loggerName = log.Logger.Name;

                // Get saved level or default to 5 (All)
                int defaultValue = 5;

                if (Conf.C.LogLevelPersistOnReloads)
                {
                    if (LogLevelSettingsJson.ReadLogLevels().TryGetValue(loggerName, out string savedLevel))
                    {
                        if (int.TryParse(savedLevel, out int parsedLevel))
                            defaultValue = parsedLevel;
                    }
                }

                // Truncate logger name for display
                string displayName;
                if (loggerName.Length > 13)
                    displayName = loggerName[..10] + "..";
                else
                    displayName = loggerName;

                AddSlider(
                    title: displayName,
                    min: 0,
                    max: 5,
                    defaultValue: defaultValue, // Set from saved value
                    onValueChanged: (value) => SetLogLevel(value, log.Logger as Logger),
                    increment: 1,
                    textSize: 0.8f,
                    hover: Loc.Get("LogPanel.SetLogLevelFor", loggerName),
                    valueFormatter: (value) => Loc.Get($"LogPanel.{(LogLevel)value}")
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

            // Save to JSON
            LogLevelSettingsJson.UpdateLogLevel(logger.Name, ((int)level).ToString());
        }

        [Obsolete("Old and unused reflection stuff")]
        private static Logger GetLogger(string loggerName = "tML")
        {
            //PropertyInfo tmlProp = typeof(Logging).GetProperty(loggerName, BindingFlags.Static | BindingFlags.NonPublic);
            //if (tmlProp == null)
            //    return null;

            //ILog tmlLogger = (ILog)tmlProp.GetValue(null);
            //if (tmlLogger == null)
            //    return null;

            //Logging.tML;

            return Logging.tML.Logger as Logger;
        }
    }
}