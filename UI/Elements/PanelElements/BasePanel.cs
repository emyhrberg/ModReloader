using System;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using ModReloader.UI.Elements.ButtonElements;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements
{
    /// <summary>
    /// A parent panel
    /// for Player, Debug, World panels
    /// </summary>
    public abstract class BasePanel : UIPanel
    {
        // Variables
        // 35 is the customtitlepanel height
        // 12 is minus the padding of a panel
        protected float currentTop = 35 - 12;

        public UIList uiList;
        protected UIScrollbar scrollbar;
        public ResizeIcon resizeButton;

        private readonly int panelPadding = 12;

        // Panel values
        protected bool Active = false; // default to false
        public string Header = "";
        public TitlePanel TitlePanel;

        // Dragging
        public bool dragging;
        public Vector2 dragOffset;
        public const float DragThreshold = 3f; // very low threshold for dragging
        public Vector2 mouseDownPos;


        // Its associated button. Used so CloseButtonPanel can close the correct panel
        // And update its state of "open" panel for a button which is highlighted.
        public BaseButton AssociatedButton { get; set; } = null;

        #region Constructor
        public BasePanel(string header, bool scrollbarEnabled = true)
        {
            // panel settings
            Width.Set(350, 0);
            Height.Set(460, 0);
            Top.Set(-70, 0);
            Left.Set(-20, 0);
            VAlign = 1.0f;
            HAlign = 1.0f;
            BackgroundColor = ColorHelper.SuperDarkBluePanel;

            Header = header;

            // Create all content in the panel
            TitlePanel = new(header);
            CloseButtonPanel closeButtonPanel = new();

            // Add all content in the panel
            Append(TitlePanel);
            Append(closeButtonPanel);

            // Create a new list
            uiList = new UIList
            {
                MaxWidth = { Percent = 1f, Pixels = panelPadding * 2 },
                Width = { Percent = 1f, Pixels = panelPadding * 2 },
                MaxHeight = { Percent = 1f, Pixels = -20 },
                Height = { Percent = 1f, Pixels = -20 },
                HAlign = 0.5f,
                VAlign = 0f,
                Top = { Pixels = 24 },
                Left = { Pixels = 0 },
                ListPadding = 0f, // 0 or 5f
                ManualSortMethod = (e) => { }
            };

            // Create a new scrollbar
            if (scrollbarEnabled)
            {
                scrollbar = new()
                {
                    // MaxHeight = something // may be needed at some point
                    Height = { Percent = 1f, Pixels = -35 - 12 - 35 }, // -35 for header, -12 for padding, -35 for resize icon
                    HAlign = 1f,
                    VAlign = 0f,
                    Left = { Pixels = 5 }, // scrollbar has 20 width
                    Top = { Pixels = 35 + 12 },
                };
            }


            // Set the scrollbar to the list
            Append(uiList);

            if (scrollbarEnabled) uiList.SetScrollbar(scrollbar);
            if (scrollbarEnabled) Append(scrollbar);

            // Resize
            resizeButton = new(Ass.Resize);
            resizeButton.OnDragY += offsetY =>
            {
                float oldHeight = Height.Pixels;
                float newHeight = oldHeight + offsetY;
                float maxHeight = 180f;

                // Clamp max height
                if (newHeight > 1000f || newHeight < maxHeight)
                {
                    return;
                }

                // Resize: Set new heights!
                Height.Set(newHeight, 0f);
                uiList.Height.Set(newHeight - 35, 0f);

                // Set new top offsets
                float topOffset = newHeight - oldHeight;
                Top.Pixels += topOffset;

                Recalculate();
            };
            Append(resizeButton);
        }
        #endregion

        #region add stuff

        public UIElement AddPadding(float padding)
        {
            // Create a basic UIElement to act as a spacer instead of using HeaderElement
            UIElement paddingElement = new();
            paddingElement.Height.Set(padding, 0f);
            paddingElement.Width.Set(0, 1f);
            uiList.Add(paddingElement);
            return paddingElement;
        }

        protected HeaderElement AddHeader(string title, Action onLeftClick = null, string hover = "")
        {
            HeaderElement headerElement = new(title, hover);
            headerElement.OnLeftClick += (mouseEvent, element) => onLeftClick?.Invoke();
            uiList.Add(headerElement);
            return headerElement;
        }
        protected SliderPanel AddSlider(string title, float min, float max, float defaultValue, Action<float> onValueChanged = null, float? increment = null, float textSize = 1f, string hover = "", Action leftClickText = null, Action rightClickText = null, Func<float, string> valueFormatter = null)
        {
            SliderPanel sliderPanel = new(title, min, max, defaultValue, onValueChanged, increment, textSize, hover, leftClickText, rightClickText, valueFormatter);
            uiList.Add(sliderPanel);
            AddPadding(3);
            return sliderPanel;
        }

        protected OptionElement AddOption(string text, Action leftClick, string hover = "", Action rightClick = null, float padding = 3f)
        {
            OptionElement option = new(leftClick, text, hover);
            uiList.Add(option);
            AddPadding(padding);
            return option;
        }

        protected ActionOption AddAction(Action leftClick, string text, string hover, Action rightClick = null, float textSize = 0.4f, float padding = 5f)
        {
            ActionOption actionOption = new(leftClick, text, hover, rightClick);
            uiList.Add(actionOption);
            AddPadding(padding);
            return actionOption;
        }

        #endregion

        public bool GetActive() => Active;
        public bool SetActive(bool active) => Active = active;

        #region update

        public override bool ContainsPoint(Vector2 point)
        {
            // If the panel isn�t active, pretend it�s not there for hit-testing.
            return Active && base.ContainsPoint(point);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            if (SliderBase.IsAnySliderLocked)
            {
                dragging = false;
                return;
            }

            base.Update(gameTime);

            // If resize dragging.
            if (resizeButton != null && resizeButton.draggingResize)
            {
                return;
            }

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (dragging)
            {
                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), mouseDownPos);
                if (dragDistance > DragThreshold)
                {
                    Left.Set(Main.mouseX - dragOffset.X, 0f);
                    Top.Set(Main.mouseY - dragOffset.Y, 0f);
                    Recalculate();
                }
            }
        }

        #endregion

        #region dragging

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            // don't drag if we are scrolling
            if (scrollbar != null && scrollbar.ContainsPoint(evt.MousePosition))
                return;

            // Bring this panel to the front by reordering in the parent.
            if (Parent is UIElement parent)
            {
                parent.RemoveChild(this);
                parent.Append(this);
            }

            // start dragging
            mouseDownPos = evt.MousePosition;
            base.LeftMouseDown(evt);
            dragging = true;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false; // stop dragging
            Recalculate();
        }

        #endregion

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;

            // first draw everything in the panel
            base.Draw(spriteBatch);
        }
    }
}
