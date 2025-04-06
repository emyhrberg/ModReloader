using System;
using System.Diagnostics;
using System.IO;
using ModHelper.Helpers;
using Terraria;

namespace ModHelper.UI.Elements
{
    /// <summary>
    /// A panel containing options to modify player behavior like God, Noclip, etc.
    /// </summary>
    public class OptionsPanel : OptionPanel
    {
        public OptionsPanel() : base(title: "Options", scrollbarEnabled: true)
        {
            // AddHeader("Options", null, hover: "Debug and testing.");
            AddAction("Open Log", Log.OpenClientLog, hover: "Open the log file");
            AddAction("Clear Log", Log.ClearClientLog, hover: "Clear the log file");
            AddAction("Start Client", StartClient, hover: "Start an additional tModLoader client");
            AddAction("Start Server", null, hover: "Start a tModLoader server");
            //AddAction("Open enabled.json", Log.OpenEnabledJson);
        }

        private static void StartClient()
        {
            try
            {
                string steamPath = Log.GetSteamPath();
                string startGameFileName = Path.Combine(steamPath, "start-tModLoader.bat");
                if (!File.Exists(startGameFileName))
                {
                    Log.Error("Failed to find start-tModLoader.bat file.");
                    return;
                }

                // create a process
                ProcessStartInfo process = new(startGameFileName)
                {
                    UseShellExecute = true,
                };

                // start the process
                Process gameProcess = Process.Start(process);
                Log.Info("Game process started with ID: " + gameProcess.Id + " and name: " + gameProcess.ProcessName);
                Main.NewText("Started tModLoader client");
            }
            catch (Exception e)
            {
                Log.Error("Failed to start game process (start-tModLoader.bat failed to launch): " + e.Message);
                return;
            }
        }
    }
}
