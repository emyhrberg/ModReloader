using System.Collections.Generic;
using SquidTestingMod.Common.Players;
using SquidTestingMod.Common.Systems;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel containing options to modify player behaviour like God, Fast, Build, etc.
    /// </summary>
    public class LogPanel : RightParentPanel
    {
        public LogPanel() : base(title: "Log", scrollbarEnabled: false)
        {
            // Player options
            AddHeader("Abilities");
        }
    }
}