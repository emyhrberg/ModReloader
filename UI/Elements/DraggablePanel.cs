using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    /// <summary>
    /// A panel which can be dragged
    /// Also has a close button and a title
    /// </summary>
    public abstract class DraggablePanel : UIPanel
    {
        // Panel values
        protected bool Active = false; // draw and update when true
        protected const int padding = 12;
        protected Color darkBlue = new(73, 85, 186);
        public string Header = "";
        public CustomTitlePanel TitlePanel;

        // Dragging
        public bool Draggable = false;
        public bool IsDragging;
        private bool dragging;
        private Vector2 dragOffset;
        private const float DragThreshold = 3f; // very low threshold for dragging
        private Vector2 mouseDownPos;

        #region Constructor
        public DraggablePanel(string header)
        {
            // Set some default panel properties 
            // Children will override this hopefully :)
            Width.Set(350, 0f);
            Height.Set(530, 0f);
            HAlign = 1.0f; // right aligned
            VAlign = 1.0f; // bottom aligned
            BackgroundColor = darkBlue;
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
            // if (IsMouseHovering)
            // {
            //     Main.LocalPlayer.mouseInterface = true;
            // }

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