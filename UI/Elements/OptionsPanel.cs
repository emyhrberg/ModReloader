using ModHelper.Helpers;

namespace ModHelper.UI.Elements
{
    /// <summary>
    /// A panel containing options to modify player behavior like God, Noclip, etc.
    /// </summary>
    public class OptionsPanel : OptionPanel
    {
        public OptionsPanel() : base(title: "Options", scrollbarEnabled: true)
        {
            AddHeader("Options", null, hover: "Debug and testing.");

            AddAction("Clear Log", Log.ClearClientLog, hover: "Clear the log file.");
            AddAction("Start Client", null, hover: "Start an additional tModLoader client.");
            AddAction("Start Server", null, hover: "Start a tModLoader server.");
        }
    }
}
