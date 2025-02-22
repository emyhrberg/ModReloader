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

            // Add option panels
            AddOptionPanel(
                title: "Hitboxes",
                description: "Show hitboxes of all entities",
                checkBox: true,
                color: Color.BlueViolet,
                onClick: hitboxSystem.ToggleHitboxes);

            AddOptionPanel(
                title: "UIElements",
                description: "Show all UI elements from mods",
                checkBox: true,
                color: Color.BlueViolet,
                onClick: drawUISystem.ToggleUIDebugDrawing);

            AddOptionPanel(
                title: "Open client.log",
                description: "Open the client.log file",
                checkBox: false,
                color: Color.Red,
                onClick: Log.OpenClientLog);

            AddOptionPanel(
                title: "Create DebugPanel 30x30",
                description: "Spawn a custom debug panel",
                checkBox: false,
                color: Color.Red,
                onClick: () =>
                {
                    MainSystem sys = ModContent.GetInstance<MainSystem>();
                    sys.mainState.Append(new CustomDebugPanel(30, 30));
                });
        }
    }
}