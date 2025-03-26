using System;
using System.Reflection;
using EliteTestingMod.Common.Configs;
using EliteTestingMod.Helpers;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Terraria.ModLoader;

namespace EliteTestingMod.UI.Elements
{
    public class LogPanel : OptionPanel
    {
        private bool log = true;
        private SliderPanel logLevelSlider;

        public LogPanel() : base(title: "Log", scrollbarEnabled: true)
        {
            AddPadding(5);
            AddHeader(title: "Log Path",
                onLeftClick: Log.OpenLogFolder,
                hover: "Click to open the folder at Steam/steamapps/common/tModLoader/tModLoader-Logs");

            ActionOption clearClient = new(Log.ClearClientLog, "Clear client.log", "Clear the client.log file");
            ActionOption openClient = new(Log.OpenClientLog, "Open client.log", "Click to open client.log");
            AddPadding(5);
            uiList.Add(openClient);
            AddPadding(5);
            uiList.Add(clearClient);
            AddPadding(5);

            logLevelSlider = AddSlider(
                title: "Log Level: All",
                min: 0,
                max: 5,
                defaultValue: 5,
                onValueChanged: SetLogLevel,
                increment: 1,
                textSize: 0.8f,
                hover: "Set the log level for the client.log file"
            );

            Option log = AddOption("Log", ToggleClientLogging, "Enable or disable all logging to client.log");
            log.SetState(Option.State.Enabled);
            Option clearOnReload = AddOption("Clear Log On Reload", ClearClientOnReload, "Clear the client.log file when reloading");
            clearOnReload.SetState(Conf.C.ClearClientLogOnReload ? Option.State.Enabled : Option.State.Disabled);
            AddPadding();

            AddHeader(title: "Game Path",
                onLeftClick: Log.OpenEnabledJsonFolder,
                hover: "Click to open the folder at Documents/My Games/Terraria/ModLoader");
            ActionOption openEnabled = new(Log.OpenEnabledJson, "Open enabled.json", "This is a json file that shows a list of all your currently enabled mods", Log.OpenEnabledJsonFolder);
            AddPadding(5);
            uiList.Add(openEnabled);
            AddPadding();
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

        private void SetLogLevel(float value)
        {
            Logger logger = GetLogger();
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

            // Update slider text
            logLevelSlider.UpdateText($"Log Level: {level.ToString()}");
        }

        private static Logger GetLogger()
        {
            PropertyInfo tmlProp = typeof(Logging).GetProperty("tML", BindingFlags.Static | BindingFlags.NonPublic);
            if (tmlProp == null)
                return null;

            ILog tmlLogger = (ILog)tmlProp.GetValue(null);
            if (tmlLogger == null)
                return null;

            return tmlLogger.Logger as Logger;
        }

        private void ClearClientOnReload()
        {
            Conf.C.ClearClientLogOnReload = !Conf.C.ClearClientLogOnReload;
            Conf.ForceSaveConfig(Conf.C);
        }

        private void ToggleClientLogging()
        {
            Logger logger = GetLogger();
            if (logger == null)
                return;

            log = !log;

            if (log)
            {
                logger.Level = Level.All;
                logger.Repository.Threshold = Level.All;
            }
            else
            {
                logger.Level = Level.Off;
                logger.Repository.Threshold = Level.Off;
            }
        }
    }
}