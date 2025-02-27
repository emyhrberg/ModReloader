using System;
using System.Linq;
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
        // Variables 
        SliderOption widthOption;
        SliderOption heightOption;

        public DebugPanel() : base(title: "Debug", scrollbarEnabled: false)
        {
            // Get instances
            HitboxSystem hitboxSystem = ModContent.GetInstance<HitboxSystem>();
            DrawUISystem drawUISystem = ModContent.GetInstance<DrawUISystem>();

            // Add debug options
            AddHeader("Hitboxes");
            AddOnOffOption(hitboxSystem.ToggleHitboxes, "Hitboxes Off", "Show hitboxes of all entities");
            AddPadding();

            AddHeader("UI");
            AddOnOffOption(drawUISystem.ToggleUIDebugDrawing, "UIElements Hitboxes Off", "Show all UI elements from mods");
            AddOnOffOption(drawUISystem.ToggleUIDebugSizeElementDrawing, "UIElements Size Text Off", "Show sizes of UI elements");
            AddOnOffOption(drawUISystem.PrintAllUIElements, "Print UIElements", "Prints all UI elements and dimensions to chat");
            AddPadding();

            AddHeader("Debug Panel");
            widthOption = AddSliderOption("Width", 0, 800, 100);
            heightOption = AddSliderOption("Height", 0, 800, 100);
            AddOnOffOption(SpawnDebugPanel, "Create DebugPanel", "Create a debug panel with the specified dimensions");
            AddOnOffOption(RemoveAllDebugPanels, "Remove All DebugPanel");
            AddPadding();

            AddHeader("Logs");
            AddOnOffOption(Log.OpenClientLog, "Open client.log", "Right click to open folder location", Log.OpenLogFolder);
            AddOnOffOption(Log.OpenEnabledJson, "Open enabled.json", "This file shows currently enabled mods\nRight click to open folder location", Log.OpenEnabledJsonFolder);
        }

        private void SpawnDebugPanel()
        {
            // get width and height from slider options
            int w = (int)Math.Round(widthOption.normalizedValue * 800);
            int h = (int)Math.Round(heightOption.normalizedValue * 800);
            Log.Info("Creating DebugPanel with dimensions: " + w + ", " + h);

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys.mainState.Append(new CustomDebugPanel(w, h));
        }

        private void RemoveAllDebugPanels()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;

            if (mainState.Children == null) return;

            // Remove all panels of type CustomDebugPanel
            foreach (var child in mainState.Children.ToList())
            {
                if (child is CustomDebugPanel)
                {
                    mainState?.RemoveChild(child);
                }
            }
        }
    }
}