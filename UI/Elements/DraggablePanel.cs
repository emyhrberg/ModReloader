using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ModHelper.UI.Buttons;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    /// <summary>
    /// A panel which can be dragged
    /// Also has a close button and a title
    /// </summary>
    public abstract class DraggablePanel : UIPanel
    {
        // Panel values
        protected bool prev_Active_state = false; // previous active state
        protected bool Active = false; // default to false
        protected const int padding = 12;
        public string Header = "";
        public CustomTitlePanel TitlePanel;

        // Dragging
        public bool Draggable = false;
        public bool IsDragging;
        protected bool dragging;
        protected Vector2 dragOffset;
        protected const float DragThreshold = 3f; // very low threshold for dragging
        protected Vector2 mouseDownPos;

        // Size
        protected static float PANEL_WIDTH = 350f;

        // Its associated button. Used so CloseButtonPanel can close the correct panel
        // And update its state of "open" panel for a button which is highlighted.
        public BaseButton AssociatedButton { get; set; } = null;

        #region Constructor
        public DraggablePanel(string header)
        {
            // Set some default panel properties 
            // Children will override this hopefully :)
            Width.Set(PANEL_WIDTH, 0f);
            Height.Set(460f, 0f);
            HAlign = 1.0f; // right aligned
            VAlign = 1.0f; // bottom aligned
            // BackgroundColor = ColorHelper.DarkBluePanel;
            Header = header;

            // Create all content in the panel
            TitlePanel = new(header);
            CloseButtonPanel closeButtonPanel = new();

            // Add all content in the panel
            Append(TitlePanel);
            Append(closeButtonPanel);
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (CustomSliderBase.IsAnySliderLocked)
            {
                IsDragging = false;
                return;
            }

            if (!Active)
                return;

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            base.Update(gameTime);

            if (Draggable & dragging)
            {
                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), mouseDownPos);
                if (dragDistance > DragThreshold)
                {
                    IsDragging = true;
                    Left.Set(Main.mouseX - dragOffset.X, 0f);
                    Top.Set(Main.mouseY - dragOffset.Y, 0f);
                    Recalculate();
                }
            }
            else
            {
                IsDragging = false;
            }
        }
        #endregion

        #region Dragging
        public override bool ContainsPoint(Vector2 point)
        {
            if (!Active)
                return false;

            // Make a hitbox of the width 1f and the height 25f
            // This is the title bar

            // Rectangle hitbox = new((int)Left.Pixels, (int)Top.Pixels, (int)Width.Pixels, 25);
            // return hitbox.Contains(point.ToPoint());

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
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            IsDragging = false;
            Recalculate();
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (IsDragging)
                return;
            base.LeftClick(evt);
        }
        #endregion

        #region Toggle Visibility
        // also see update() for more visibility toggling
        // we modify both update() and draw() when active is false
        public bool GetActive() => Active;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;
            base.Draw(spriteBatch);
        }
        #endregion

        #region Reset position
        // When we click on a button, we toggle the active state of the panel.
        // This method is called to reset the position of the panel when it is toggled (when the panel is shown again).
        public virtual bool SetActive(bool active)
        {
            /// Implemented in child classes
            /// <see cref="SpawnerPanel"/> 
            /// <see cref="OptionPanel"/> 
            Log.Info($"{(active ? "Open" : "Close")} {Header} panel");
            AssociatedButton.ParentActive = false;
            return Active = active;
        }
        #endregion
    }
}