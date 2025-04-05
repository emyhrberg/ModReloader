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
            AddAction("Start Client", () => Main.NewText("Main: " + Main.SavePath), hover: "Start an additional tModLoader client");
            AddAction("Start Server", null, hover: "Start a tModLoader server");
        }
    }
}
