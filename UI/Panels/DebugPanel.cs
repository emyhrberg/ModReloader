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
    public class DebugPanel : DraggablePanel
    {
        public DebugPanel() : base("Debug")
        {
            // Hitboxes
            HitboxSystem hitboxSystem = ModContent.GetInstance<HitboxSystem>();
            OptionPanel hitboxOption = new("Hitboxes", "Show hitboxes of all entities", true, Color.BlueViolet);
            hitboxOption.OnLeftClick += (a, b) => hitboxSystem.ToggleHitboxes();

            // elementsCheckbox
            DrawUISystem drawUISystem = ModContent.GetInstance<DrawUISystem>();
            OptionPanel elementsOption = new("UIElements", "Show all UI elements from mods", true, Color.Green);
            elementsOption.OnLeftClick += (a, b) => drawUISystem.ToggleUIDebugDrawing();

            // Open client.log panel button
            OptionPanel clientLogOption = new("Open client.log", "Open the client.log file", false, Color.Orange);
            clientLogOption.OnLeftClick += (a, b) => Log.OpenClientLog();

            // TODO more buttons here for debugging
            OptionPanel spawnOption = new("Create DebugPanel 30x30", "Spawn a custom debug panel", false, Color.Red);
            spawnOption.OnLeftClick += (a, b) => Append(new CustomDebugPanel(30, 30));
            // Set option positions
            hitboxOption.Top.Set(35 + padding, 0f);
            elementsOption.Top.Set(35 + 65 + padding, 0f);
            clientLogOption.Top.Set(35 + 65 * 2 + padding, 0f);
            spawnOption.Top.Set(35 + 65 * 3 + padding, 0f);

            // Add all content in the panel
            Append(hitboxOption);
            Append(elementsOption);
            Append(clientLogOption);
            Append(spawnOption);
        }
    }
}