using System.Reflection;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria.ModLoader;

namespace SquidTestingMod.UI.Elements
{
    public class LogPanel : OptionPanel
    {
        private bool log = false;

        public LogPanel() : base(title: "Log", scrollbarEnabled: true)
        {
            AddPadding(5);
            AddHeader("Client");
            OnOffOption openClient = new(Log.OpenClientLog, "Open client.log", "Left click to open client.log\nRight click to open folder location", Log.OpenLogFolder);
            OnOffOption clearClient = new(Log.ClearClientLog, "Clear client.log", "Clear the client.log file");
            OnOffOption clearClientOnReload = new(ClearClientOnReload, "Clear When Reload Off", "Clear the client.log file when reloading");
            OnOffOption logOption = new(ToggleLogging, "Log On", "Whether to log anything at all to client.log");
            uiList.Add(openClient);
            uiList.Add(clearClient);
            uiList.Add(clearClientOnReload);
            uiList.Add(logOption);
            AddPadding();

            AddHeader("Other Files");
            OnOffOption openEnabled = new(Log.OpenEnabledJson, "Open enabled.json", "Shows a list of currently enabled mods", Log.OpenEnabledJsonFolder);
            uiList.Add(openEnabled);
            AddPadding();

        }

        private Logger GetLogger()
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

        private void ToggleLogging()
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