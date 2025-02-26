using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Panels;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel containing options to modify player behaviour like God,Fast,Build,etc.
    /// </summary>
    public class DebugPanel : RightParentPanel
    {
        public DebugPanel() : base("Debug")
        {
            // Get instances
            HitboxSystem hitboxSystem = ModContent.GetInstance<HitboxSystem>();
            DrawUISystem drawUISystem = ModContent.GetInstance<DrawUISystem>();

            // Add debug options
            AddHeader("Hitboxes");
            AddOnOffOption(hitboxSystem.ToggleHitboxes, "Hitboxes Off", "Show hitboxes of all entities");
            AddPadding();
            AddHeader("UI");
            AddOnOffOption(drawUISystem.ToggleUIDebugDrawing, "UIElements Off", "Show all UI elements from mods");
            AddOnOffOption(SpawnDebugPanel, "Create DebugPanel 30x30", "Create a debug panel at 30,30");
            AddPadding();
            AddHeader("Logs");
            AddOnOffOption(Log.OpenClientLog, "Open client.log", "Open the client.log file");
        }

        private void SpawnDebugPanel()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys.mainState.Append(new CustomDebugPanel(30, 30));
        }
    }
}