using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        protected UIList uiList;
        protected UIScrollbar scrollbar;
        protected bool scrollbarEnabled = true;

        private int panelPadding = 12;

        public OptionPanel(string title, bool scrollbarEnabled = true) : base(title)
        {
            // panel settings
            Height.Set(460f, 0f);
            Top.Set(-70, 0f);
            Left.Set(-20, 0f);
            BackgroundColor = ColorHelper.DarkBluePanel;

            Draggable = true; // maybe change this later when u fix sliders

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
                    Height = { Percent = 1f, Pixels = -35 - 12 },
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
            OptionElement option = new(leftClick, text, hover, rightClick);
            uiList.Add(option);
            AddPadding(padding);
            return option;
        }

        protected ActionOption AddAction(string text, Action leftClick, string hover, Action rightClick = null, float textSize = 0.4f, float padding = 5f)
        {
            ActionOption actionOption = new(leftClick, text, hover, rightClick);
            uiList.Add(actionOption);
            AddPadding(padding);
            return actionOption;
        }

        protected HeaderElement AddHeader(string title, Action onLeftClick = null, string hover = "", Color color = default, float HAlign = 0.5f)
        {
            HeaderElement headerElement = new(title, hover, color, HAlign);
            headerElement.OnLeftClick += (mouseEvent, element) => onLeftClick?.Invoke();
            uiList.Add(headerElement);
            return headerElement;
        }

        protected Searchbox addSearchbox(float padding = 3f)
        {
            Searchbox searchbox = new("Type to search");
            uiList.Add(searchbox);
            AddPadding(padding);
            return searchbox;
        }

        /// <summary>
        /// Add padding to the panel with a blank header with the given panel element height
        /// </summary>
        protected HeaderElement AddPadding(float padding = 20f)
        {
            // Create a blank UIElement to act as a spacer.
            HeaderElement paddingElement = new("");
            paddingElement.Height.Set(padding, 0f);
            paddingElement.Width.Set(0, 1f);
            uiList.Add(paddingElement);
            return paddingElement;
        }

        public override void Update(GameTime gameTime)
        {
            // test
            //uiList.Width.Set(0, 1);

            base.Update(gameTime);

            // If the inventory is open, move the panel to the left by 350 pixels
            bool inventoryOpen = Main.playerInventory;

            // if (inventoryOpen)
            // {
            //     Left.Set(-225, 0f);
            // }
            // else
            // {
            //     Left.Set(-20, 0f);
            // }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (scrollbar != null && scrollbar.ContainsPoint(evt.MousePosition))
                return;

            if (!Active)
                return;

            // this drag code needs to be here because we override some stuff above.
            // so OptionPanel needs to have its own LeftMouseDown method
            // and call base.LeftMouseDown(evt) to make it work properly.
            mouseDownPos = evt.MousePosition;
            base.LeftMouseDown(evt);
            dragging = true;
            IsDragging = false;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
        }

        // Build the last ago by every second, adding the time


        public override void Draw(SpriteBatch spriteBatch)
        {
            // first draw everything in the panel
            base.Draw(spriteBatch);

            // last, draw the hover texture
            foreach (var element in uiList._items.ToList())
            {
                if (element is ModElement modElement)
                {
                    var icon = modElement.modIcon;
                    if (icon != null && icon.IsHovered && icon.updatedTex != null)
                    {
                        Vector2 mousePos = new(Main.mouseX - icon.Width.Pixels * 4, Main.mouseY - icon.Height.Pixels * 2);
                        spriteBatch.Draw(icon.updatedTex, mousePos, Color.White);
                    }
                }
                else if (element is ModSourcesElement modSourcesElement)
                {
                    var icon = modSourcesElement.modIcon;
                    if (icon != null && icon.IsHovered && icon.tex != null)
                    {
                        Vector2 mousePos = new(Main.mouseX - icon.Width.Pixels * 4, Main.mouseY - icon.Height.Pixels * 2);
                        spriteBatch.Draw(icon.tex, mousePos, Color.White);

                        // Determine the color based on the time ago
                        TimeSpan timeAgo = DateTime.Now - icon.lastModified;
                        Color timeColor = timeAgo.TotalSeconds < 60 ? new Color(5, 230, 55) :
                                          timeAgo.TotalMinutes < 60 ? new Color(5, 230, 55) :
                                          timeAgo.TotalHours < 24 ? Color.Orange :
                                          Color.Red;

                        string builtAgo = ConvertLastModifiedToTimeAgo(icon.lastModified);

                        if (!string.IsNullOrEmpty(builtAgo))
                        {
                            Utils.DrawBorderString(
                                spriteBatch,
                                text: $"Built {builtAgo}",
                                new Vector2(mousePos.X + icon.Width.Pixels, mousePos.Y - 10),
                                timeColor,
                                scale: 1.0f,
                                0.5f,
                                0.5f
                            );
                        }
                    }
                }
            }
        }

        private static string ConvertLastModifiedToTimeAgo(DateTime lastModified)
        {
            TimeSpan timeAgo = DateTime.Now - lastModified;
            if (timeAgo.TotalSeconds < 60)
            {
                return $"{timeAgo.Seconds} seconds ago";
            }
            else if (timeAgo.TotalMinutes < 2)
            {
                return $"{timeAgo.Minutes} minute ago";
            }
            else if (timeAgo.TotalMinutes < 60)
            {
                return $"{timeAgo.Minutes} minutes ago";
            }
            else if (timeAgo.TotalHours < 2)
            {
                return $"{timeAgo.Hours} hour ago";
            }
            else if (timeAgo.TotalHours < 24)
            {
                return $"{timeAgo.Hours} hours ago";
            }
            else if (timeAgo.TotalDays < 2)
            {
                return $"{timeAgo.Days} day ago";
            }
            else
            {
                return $"{timeAgo.Days} days ago";
            }
        }

        #region Reset position
        // When we click on a button, we toggle the active state of the panel.
        // This method is called to reset the position of the panel when it is toggled (when the panel is shown again).
        public override bool SetActive(bool active)
        {
            if (active)
            {
                // Reset panel position
                Top.Set(-70, 0f);
                Left.Set(-20, 0f);
                Recalculate();
            }

            Active = active;
            return Active;
        }
        #endregion
    }
}
