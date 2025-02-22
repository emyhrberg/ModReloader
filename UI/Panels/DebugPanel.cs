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
    public class DebugPanel : UIPanel
    {
        // Panel values
        private bool Active = false; // true for visible and update, false for not visible and not update
        private const int padding = 12;
        private const int W = 350; // Width of the panel
        private const int H = 500; // Height of the panel

        // Colors
        private Color lightBlue = new(63, 82, 151);
        private Color darkBlue = new(73, 85, 186);

        // UI Elements
        private UIPanel CloseButtonPanel;
        private UIPanel TitlePanel;
        private UIText HeaderText;

        #region Constructor
        public DebugPanel()
        {
            // Set the panel properties
            Width.Set(W, 0f);
            Height.Set(H, 0f);
            HAlign = 1.0f;
            VAlign = 1.0f;
            BackgroundColor = lightBlue;

            // Create all content in the panel
            TitlePanel = new CustomTitlePanel(padding: padding, bgColor: darkBlue, height: 35);
            HeaderText = new UIText(text: "Debug", textScale: 0.4f, large: true);
            CloseButtonPanel = new CloseButtonPanel();

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
            elementsOption.Top.Set(70 + padding, 0f);
            clientLogOption.Top.Set(105 + padding, 0f);
            spawnOption.Top.Set(140 + padding, 0f);

            // Add all content in the panel
            Append(TitlePanel);
            Append(HeaderText);
            Append(CloseButtonPanel);
            Append(hitboxOption);
            Append(elementsOption);
            Append(clientLogOption);
            Append(spawnOption);
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Active = false;
                Main.playerInventory = false;
                return;
            }

            base.Update(gameTime);

            if (dragging)
            {
                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), mouseDownPos);
                if (dragDistance > DragThreshold)
                {
                    IsDragging = true;
                    Left.Set(Main.mouseX - dragOffset.X, 0f);
                    Top.Set(Main.mouseY - dragOffset.Y, 0f);
                    Recalculate();
                    Main.LocalPlayer.mouseInterface = true;
                }
            }
            else
            {
                IsDragging = false;
            }

            if (dragging || ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
        #endregion

        #region Dragging
        public bool IsDragging;
        private bool dragging;
        private Vector2 dragOffset;
        private const float DragThreshold = 3f; // very low threshold for dragging
        private Vector2 mouseDownPos;

        public override bool ContainsPoint(Vector2 point)
        {
            if (!Active)
                return false;

            return base.ContainsPoint(point);
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (!Active)
                return;

            mouseDownPos = evt.MousePosition;
            base.LeftMouseDown(evt);
            dragging = true;
            IsDragging = false;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
            Main.LocalPlayer.mouseInterface = true;
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            IsDragging = false;
            Main.LocalPlayer.mouseInterface = false;
            Recalculate();
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (IsDragging)
                return;
            base.LeftClick(evt);
            Main.LocalPlayer.mouseInterface = true;
        }
        #endregion

        #region Toggle Visibility
        // also see update() for more visibility toggling
        // we modify both update() and draw() when active is false
        public bool GetActive() => Active;
        public bool SetActive(bool active) => Active = active;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;
            base.Draw(spriteBatch);
        }
        #endregion
    }
}