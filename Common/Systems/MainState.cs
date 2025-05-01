using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ModHelper.UI;
using ModHelper.UI.AbstractElements;
using ModHelper.UI.Elements;
using ModHelper.UI.Elements.DebugElements;
using ModHelper.UI.ModElements;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.Common.Systems
{
    public class MainState : UIState
    {
        // Buttons
        public ModsButton modsButton;
        public ModSourcesButton modSourcesButton;
        public ReloadSPButton reloadSPButton;
        public ReloadMPButton reloadMPButton;

        // Panels
        public ModsPanel modsPanel;
        public ModSourcesPanel modSourcesPanel;

        // More
        public bool Active = true; // flag to toggle all buttons on/off using the toggle button
        public float ButtonSize = 70f;
        public float TextSize = 0.9f;
        public float offset = 0; // START offset for first button position relative to center

        // Lists
        public List<BaseButton> AllButtons = [];
        public List<BasePanel> AllPanels = [];

        #region Constructor
        public MainState()
        {
            offset = -ButtonSize * 4;

            // Add buttons
            modsButton = AddButton<ModsButton>(Ass.ButtonMods, "Mods", "Manage Mods", hoverTextDescription: "Toggle mods on or off");
            modsPanel = AddPanel<ModsPanel>();
            modsButton.AssociatedPanel = modsPanel;
            modsPanel.AssociatedButton = modsButton;

            modSourcesButton = AddButton<ModSourcesButton>(Ass.ButtonModSources, "My Mods", "Mod Sources", hoverTextDescription: "Manage mod sources and reload");
            modSourcesPanel = AddPanel<ModSourcesPanel>();
            modSourcesButton.AssociatedPanel = modSourcesPanel;
            modSourcesPanel.AssociatedButton = modSourcesButton;

            

            string reloadHoverMods;
            if (Conf.C.ModsToReload == "")
                reloadHoverMods = "No mods selected";
            else
                reloadHoverMods = string.Join(",", Conf.C.ModsToReload);

            if (Main.netMode == NetmodeID.SinglePlayer)
                reloadSPButton = AddButton<ReloadSPButton>(Ass.ButtonReloadSP, buttonText: "Reload", hoverText: "Reload", hoverTextDescription: reloadHoverMods);

            if (Main.netMode == NetmodeID.MultiplayerClient)
                reloadMPButton = AddButton<ReloadMPButton>(Ass.ButtonReloadMP, buttonText: "Reload", hoverText: "Reload", hoverTextDescription: reloadHoverMods);

            if (Conf.C.AddMainMenu)
            {
                // Add debug text to the main state
                DebugText debugText = new("");
                Append(debugText);

                string logFileName = Path.GetFileName(Logging.LogPath);
                DebugAction openLog = new("Open log, ", $"Open {logFileName}", Conf.C.OpenLogType == "File" ? Log.OpenClientLog : Log.OpenLogFolder);
                DebugAction clearLog = new("Clear log", $"Clear {logFileName}", Log.ClearClientLog, left: 81f);
                Append(openLog);
                Append(clearLog);
            }
        }
        #endregion

        private T AddPanel<T>() where T : BasePanel, new()
        {
            // Create a new panel using reflection
            T panel = new();

            // Add to appropriate list
            AllPanels.Add(panel);

            // Add to MainState
            Append(panel);
            return panel;
        }

        private T AddButton<T>(Asset<Texture2D> spritesheet = null, string buttonText = null, string hoverText = null, string hoverTextDescription = "") where T : BaseButton
        {
            // Create a new button using reflection
            T button = (T)Activator.CreateInstance(typeof(T), spritesheet, buttonText, hoverText, hoverTextDescription);

            // Button dimensions
            float size = ButtonSize;
            button.Width.Set(size, 0f);
            button.Height.Set(size, 0f);
            button.MaxWidth = new StyleDimension(size, 0);
            button.MaxHeight = new StyleDimension(size, 0);
            button.MinWidth = new StyleDimension(size, 0);
            button.MinHeight = new StyleDimension(size, 0);

            // set x pos with offset
            button.Left.Set(pixels: offset, precent: 0f);

            // set text scale
            button.TextScale = TextSize;

            // custom left pos. override default
            // convert vector2 to valign and halign
            // Read the VAlign and HAlign from the config
            Vector2 buttonsPosition = new(0.5f, 1.0f);
            button.VAlign = buttonsPosition.Y;
            button.HAlign = buttonsPosition.X;

            button.Recalculate();

            // increase offset for next button, except MPbutton
            offset += ButtonSize;

            // Add the button to the list of all buttons and append it to the MainState
            AllButtons.Add(button);
            Append(button);

            return button;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;

            // draw everything in the main state
            base.Draw(spriteBatch);

            // last, draw the tooltips
            // this is to avoid the tooltips being drawn over the buttons
            foreach (var button in AllButtons)
            {
                if (button.IsMouseHovering && button.HoverText != null)
                {
                    // Draw the tooltip
                    button.DrawTooltipPanel(button.HoverText, button.HoverTextDescription); // Draw the tooltip panel
                }
            }
        }
    }
}