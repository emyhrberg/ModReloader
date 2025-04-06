using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ModHelper.UI.Buttons;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI
{
    public class MainState : UIState
    {
        // Buttons
        public OptionsButton optionsButton;
        public UIElementButton uiButton;
        public ModsButton modsButton;
        public ModSourcesButton modSourcesButton;
        public ReloadSPButton reloadSPButton;
        public ReloadMPButton reloadMPButton;

        // Panels
        public OptionsPanel optionsPanel;
        public ModsPanel modsPanel;
        public UIElementPanel uiPanel;
        public ModSourcesPanel modSourcesPanel;
        public List<DraggablePanel> AllPanels = [];

        // Buttons
        public bool AreButtonsShowing = true; // flag to toggle all buttons on/off using the toggle button
        public float ButtonSize = 70f;
        public float TextSize = 0.9f;
        public float offset = 0; // START offset for first button position relative to center
        public List<BaseButton> AllButtons = [];

        private Dictionary<float, float> buttonTextSizes = new()
        {
            { 50f, 0.8f },
            { 55f, 0.8f },
            { 60f, 0.9f },
            { 65f, 0.9f },
            { 70f, 1f },
            { 75f, 1.15f },
            { 80f, 1.35f }
        };

        #region Constructor
        // MainState Constructor. This is where we create all the buttons and set up their positions.
        public MainState()
        {
            // Set buttonsize according to config
            Config c = ModContent.GetInstance<Config>();
            // if (c != null)
            // ButtonSize = Conf.C.Buttons.ButtonSize != 0 ? Conf.C.Buttons.ButtonSize : 70f;

            // Set text size
            if (buttonTextSizes.TryGetValue(ButtonSize, out float value))
            {
                // Log.Info($"Button size: {ButtonSize}, Text size: {value}");
                TextSize = value;
            }
            ButtonSize = 70f;

            // Set offset for first button position relative to center
            offset = -ButtonSize * 4;
            offset -= 20; // 20 is CUSTOM CUSTOM CUSTOM offset, see collapse also. this is to avoid the collapse button colliding with heros mod

            // Get client{x}.log
            string logPath = Path.GetFileName(Logging.LogPath);

            // Add buttons
            if (Conf.C.Buttons.ShowOptionsButton)
                optionsButton = AddButton<OptionsButton>(Ass.ButtonOptions, "Test", "Options");

            if (Conf.C.Buttons.ShowUIButton)
                uiButton = AddButton<UIElementButton>(Ass.ButtonUI, "UI", "UI Playground", hoverTextDescription: "Right click to toggle all UIElements");

            if (Conf.C.Buttons.ShowModsButton)
                modsButton = AddButton<ModsButton>(Ass.ButtonMods, "Mods", "Manage Mods", hoverTextDescription: "Manage mods to toggle");

            if (Conf.C.Buttons.ShowModSourcesButton)
            {
                modSourcesButton = AddButton<ModSourcesButton>(Ass.ButtonModSources, "Mods", "Mod Sources", hoverTextDescription: "View and edit mods to reload");
            }

            // Initialize reload SP and MP buttons

            string reloadHoverMods = Conf.C.ModToReload ?? "No mod selected";

            if (Conf.C.Buttons.ShowReloadButton)
                reloadSPButton = AddButton<ReloadSPButton>(Ass.ButtonReloadSP, buttonText: "Reload", hoverText: "Reload", hoverTextDescription: reloadHoverMods);

            if (Conf.C.Buttons.ShowReloadMPButton)
                reloadMPButton = AddButton<ReloadMPButton>(Ass.ButtonReloadMP, buttonText: "Reload", hoverText: "Reload", hoverTextDescription: reloadHoverMods);

            // Add the panels (invisible by default)
            optionsPanel = AddPanel<OptionsPanel>();
            modsPanel = AddPanel<ModsPanel>();
            uiPanel = AddPanel<UIElementPanel>();
            modSourcesPanel = AddPanel<ModSourcesPanel>();

            // Associate buttons with panels so we can highlight the buttons with open panels 
            // And close 
            modsPanel.AssociatedButton = modsButton;
            uiPanel.AssociatedButton = uiButton;
            optionsPanel.AssociatedButton = optionsButton;
            modSourcesPanel.AssociatedButton = modSourcesButton;

            // Temporary debug text for player name, who am I, and frame rate
            if (Conf.C.SizeDebugText != "Off")
            {
                DebugText debugText = new(text: "");
                Append(debugText);
            }
        }
        #endregion

        private T AddPanel<T>() where T : DraggablePanel, new()
        {
            // Create a new panel using reflection
            T panel = new();

            // Add to appropriate list
            AllPanels.Add(panel);

            // If the panel is already open, set it to active
            if (panel.GetActive())
            {
                panel.SetActive(true);
            }
            else
            {
                panel.SetActive(false);
            }

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
            Vector2 buttonsPosition = ButtonsPositionJsonHelper.ReadButtonsPosition();
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
            // draw everything in the main state
            base.Draw(spriteBatch);

            // last, draw the tooltips
            // this is to avoid the tooltips being drawn over the buttons
            foreach (var button in AllButtons)
            {
                if (button.IsMouseHovering && button.HoverText != null)
                {
                    // Draw the tooltip
                    DrawHelper.DrawTooltipPanel(button, button.HoverText, button.HoverTextDescription); // Draw the tooltip panel
                }
            }
        }

        #region Left/right dragging

        public bool dragging; // whether we are dragging or not
        private Vector2 dragStartPosition; // where the drag started
        public bool isClick = false; // used to separate clicking from dragging

        public override void RightMouseDown(UIMouseEvent evt)
        {
            base.RightMouseDown(evt);

            if (Conf.C.DragButtons == "Right")
            {
                DragStart();
            }
        }

        public override void RightMouseUp(UIMouseEvent evt)
        {
            base.RightMouseUp(evt);

            if (Conf.C.DragButtons == "Right")
            {
                DragEnd();
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            if (Conf.C.DragButtons == "Left")
            {
                DragStart();
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);

            if (Conf.C.DragButtons == "Left")
            {
                DragEnd();
            }
        }

        // Helper to get the rectangle that encompasses all buttons
        private Rectangle GetButtonAreaRectangle()
        {
            // Find min and max positions of all buttons
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (var button in AllButtons)
            {
                CalculatedStyle dimensions = button.GetDimensions();
                minX = Math.Min(minX, dimensions.X);
                minY = Math.Min(minY, dimensions.Y);
                maxX = Math.Max(maxX, dimensions.X + dimensions.Width);
                maxY = Math.Max(maxY, dimensions.Y + dimensions.Height);
            }

            return new Rectangle(
                (int)minX,
                (int)minY,
                (int)(maxX - minX),
                (int)(maxY - minY)
            );
        }

        // Check if the mouse is within the button area
        private bool IsMouseInButtonArea()
        {
            Rectangle buttonArea = GetButtonAreaRectangle();
            return buttonArea.Contains(Main.mouseX, Main.mouseY);
        }

        private void DragStart()
        {
            // Only start dragging if mouse is within button area
            if (IsMouseInButtonArea())
            {
                dragStartPosition = new Vector2(Main.mouseX, Main.mouseY);
                dragging = true;

                // Store initial alignments and offsets of all buttons
                foreach (var button in AllButtons)
                {
                    // Store tuple of: HAlign, VAlign, Left offset
                    button.Tag = new Tuple<float, float, float>(
                        button.HAlign,
                        button.VAlign,
                        button.Left.Pixels
                    );
                }
            }
        }

        private void DragEnd()
        {
            if (dragging)
            {
                dragging = false;

                // Save the new button position to config
                // Use the first button's alignment values
                Vector2 buttonsPosition = new Vector2(AllButtons[0].HAlign, AllButtons[0].VAlign);

                // Write the updated config to a JSON file.
                ButtonsPositionJsonHelper.WriteButtonsPosition(buttonsPosition);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (dragging)
            {
                // First check if we dragged far enough to consider it a drag
                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), dragStartPosition);
                if (dragDistance < 10f) // 10 pixels threshold
                {
                    isClick = true;
                    return;
                }
                isClick = false;

                // Calculate the drag delta as percentage of screen
                Vector2 mouseDelta = new Vector2(Main.mouseX, Main.mouseY) - dragStartPosition;
                Vector2 alignmentDelta = new Vector2(
                    mouseDelta.X / Main.screenWidth,
                    mouseDelta.Y / Main.screenHeight
                );

                // Move all buttons by adjusting alignment but preserve horizontal spacing
                foreach (var button in AllButtons)
                {
                    if (button.Tag is Tuple<float, float, float> initialData)
                    {
                        // Unpack the tuple
                        float initialHAlign = initialData.Item1;
                        float initialVAlign = initialData.Item2;
                        float initialLeftOffset = initialData.Item3;

                        // Calculate new alignment values with clamping to keep on screen
                        float newHAlign = Math.Clamp(initialHAlign + alignmentDelta.X, 0.16f, 1.05f);
                        float newVAlign = Math.Clamp(initialVAlign + alignmentDelta.Y, 0.0f, 0.99f);

                        // Apply the new alignment
                        button.HAlign = newHAlign;
                        button.VAlign = newVAlign;

                        // Keep the original horizontal offset
                        button.Left.Set(initialLeftOffset, 0);

                        button.Recalculate();
                    }
                }

                // Prevent using items while dragging
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        #endregion
    }
}