using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Configs;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
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
            BackgroundColor = darkBlue * 1.0f; // modify opacity if u want here
            Height.Set(400, 0f);
            Top.Set(-70, 0f);
            Left.Set(-20, 0f);

            Draggable = false; // maybe change this later when u fix sliders

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
                    Top = { Pixels = 35 + 12 }
                };
            }


            // Set the scrollbar to the list
            Append(uiList);
            if (scrollbarEnabled) uiList.SetScrollbar(scrollbar);
            if (scrollbarEnabled) Append(scrollbar);
        }

        protected HeaderElement AddHeader(string title, Action onLeftClick = null, string hover = "")
        {
            HeaderElement headerElement = new(title, hover);
            headerElement.OnLeftClick += (mouseEvent, element) => onLeftClick?.Invoke();
            uiList.Add(headerElement);
            return headerElement;
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

            if (Conf.DraggablePanels)
            {
                return;
            }

            // If the inventory is open, move the panel to the left by 350 pixels
            bool inventoryOpen = Main.playerInventory;

            if (inventoryOpen)
            {
                Left.Set(-225, 0f);
            }
            else
            {
                Left.Set(-20, 0f);
            }

        }

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
                        Vector2 mousePos = new(Main.mouseX, Main.mouseY);
                        spriteBatch.Draw(icon.updatedTex, mousePos, Color.White);
                    }
                }
                else if (element is ModSourcesElement modSourcesElement)
                {
                    var icon = modSourcesElement.modIcon;
                    if (icon != null && icon.IsHovered && icon.tex != null)
                    {
                        Vector2 mousePos = new(Main.mouseX, Main.mouseY);
                        spriteBatch.Draw(icon.tex, mousePos, Color.White);
                    }
                }
            }
        }
    }
}
