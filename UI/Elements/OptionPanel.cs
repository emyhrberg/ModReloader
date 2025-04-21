using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    /// <summary>
    /// A parent panel
    /// for Player, Debug, World panels
    /// </summary>
    public abstract class OptionPanel : DraggablePanel
    {
        // Variables
        // 35 is the customtitlepanel height
        // 12 is minus the padding of a panel
        protected float currentTop = 35 - 12;

        public UIList uiList;
        protected UIScrollbar scrollbar;
        protected bool scrollbarEnabled = true;

        private int panelPadding = 12;

        public OptionPanel(string title, bool scrollbarEnabled = true) : base(title)
        {
            // panel settings
            Height.Set(460f, 0f);
            Top.Set(-70, 0f);
            Left.Set(-20, 0f);
            BackgroundColor = ColorHelper.SuperDarkBluePanel;

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


                // Clamp min height
                // if (newHeight < 200f)
                // newHeight = 200f;

                // Resize: Set new heights!
                Height.Set(newHeight, 0f);
                uiList.Height.Set(newHeight - 35, 0f);
                // scrollbar.Height.Set(newHeight - -35 - 12 - 35 - 35, 0f);

                // Set new top offsets
                float topOffset = newHeight - oldHeight;
                Top.Pixels += topOffset;
                // ItemsGrid.Top.Pixels -= topOffset;
                // Scrollbar.Top.Pixels -= topOffset;

                Recalculate();
            };
            Append(resizeButton);
        }

        public UIElement AddPadding(float padding = 20f)
        {
            // Create a basic UIElement to act as a spacer instead of using HeaderElement
            UIElement paddingElement = new();
            paddingElement.Height.Set(padding, 0f);
            paddingElement.Width.Set(0, 1f);
            uiList.Add(paddingElement);
            return paddingElement;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (scrollbar != null && scrollbar.ContainsPoint(evt.MousePosition))
                return;

            if (!Active)
                return;

            // Bring this panel to the front by reordering in the parent.
            if (this.Parent != null)
            {
                UIElement parent = this.Parent;
                parent.RemoveChild(this);
                parent.Append(this);
            }

            // this drag code needs to be here because we override some stuff above.
            // so OptionPanel needs to have its own LeftMouseDown method
            // and call base.LeftMouseDown(evt) to make it work properly.
            mouseDownPos = evt.MousePosition;
            base.LeftMouseDown(evt);
            dragging = true;
            IsDragging = false;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // first draw everything in the panel
            base.Draw(spriteBatch);

            // last, draw the hover texture. but only if conc.c.modsview is small
            // DONT DRAW !
            foreach (var element in uiList._items.ToList())
            {
                if (element is ModElement modElement && Conf.C.ShowIconsWhenHovering)
                {
                    var icon = modElement.modIcon;
                    if (icon != null && icon.IsHovered && icon.updatedTex != null)
                    {
                        Vector2 mousePos = new(Main.mouseX - icon.Width.Pixels * 4, Main.mouseY - icon.Height.Pixels * 2); // !!!!!!!!!
                        // spriteBatch.Draw(icon.updatedTex, mousePos, Color.White);
                    }
                }
                else if (element is ModSourcesElement modSourcesElement)
                {
                    return;
                    // do nothing for now since i updated these with bigger icons
                    var icon = modSourcesElement.modIcon;
                    if (icon != null && icon.IsHovered && icon.tex != null)
                    {
                        Vector2 mousePos = new(Main.mouseX - icon.Width.Pixels * 2, Main.mouseY - icon.Height.Pixels * 2);
                        spriteBatch.Draw(icon.tex, mousePos, Color.White);

                        // Determine the color based on the time ago
                        TimeSpan timeAgo = DateTime.Now - icon.lastModified;
                        Color timeColor = timeAgo.TotalSeconds < 60 ? new Color(5, 230, 55) :
                                          timeAgo.TotalMinutes < 60 ? new Color(5, 230, 55) :
                                          timeAgo.TotalHours < 24 ? Color.Orange :
                                          Color.Red;

                        // string builtAgo = ConvertLastModifiedToTimeAgo(icon.lastModified);

                        // if (!string.IsNullOrEmpty(builtAgo))
                        // {
                        // Utils.DrawBorderString(
                        // spriteBatch,
                        // text: $"Built {builtAgo}",
                        // new Vector2(mousePos.X + icon.Width.Pixels, mousePos.Y - 10),
                        // timeColor,
                        // scale: 1.0f,
                        // 0.5f,
                        // 0.5f
                        // );
                        // }    
                    }
                }
            }
        }

        #region Reset position
        // This method is called to reset the position of the panel when it is toggled (when the panel is shown again).
        public override bool SetActive(bool active)
        {
            if (active && Conf.C.ResetPanelPositionWhenToggling)
            {
                // Reset panel position
                Top.Set(-70, 0f);
                Left.Set(-20, 0f);

                if (this is ModSourcesPanel && Conf.C.AllowMultiplePanelsOpenSimultaneously)
                {
                    Left.Set(-20 - 350 - 20, 0f); // -20 for padding, -350 for the size of the panel, -20 for padding
                }

                Recalculate();
            }

            Active = active;
            return Active;
        }
        #endregion
    }
}
