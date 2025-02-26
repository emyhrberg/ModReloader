using System;
using Microsoft.Xna.Framework;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using XPT.Core.Audio.MP3Sharp.Decoding;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A parent panel
    /// for Player, Debug, World panels
    /// </summary>
    public abstract class RightParentPanel : DraggablePanel
    {
        // Variables
        // 35 is the customtitlepanel height
        // 12 is minus the padding of a panel
        protected float currentTop = 35 - 12;

        private UIList uiList;
        private UIScrollbar scrollbar;

        public RightParentPanel(string header) : base(header)
        {
            Draggable = true;

            // Create a new list
            uiList = new()
            {
                Width = { Percent = 1f },
                Height = { Percent = 1f, Pixels = -35 - 12 },
                HAlign = 0.5f,
                VAlign = 0f,
                Top = { Pixels = 35 + 12 },
                ListPadding = 0f,
                ManualSortMethod = (e) => { }
            };

            // Create a new scrollbar
            scrollbar = new()
            {
                Height = { Percent = 1f, Pixels = -35 - 12 },
                HAlign = 1f,
                VAlign = 0f,
                Left = { Pixels = 5 }, // scrollbar has 20 width
                Top = { Pixels = 35 + 12 }
            };

            // Set the scrollbar to the list
            uiList.SetScrollbar(scrollbar);
            Append(uiList);
            Append(scrollbar);
        }

        protected HeaderElement AddHeader(string title)
        {
            HeaderElement headerElement = new(title);
            uiList.Add(headerElement);
            return headerElement;
        }

        /// <summary>
        /// Add padding to the panel with a blank header with the given panel element height
        /// </summary>
        protected HeaderElement AddPadding(float padding = 15f)
        {
            // Create a blank UIElement to act as a spacer.
            HeaderElement paddingElement = new("");
            paddingElement.Height.Set(padding, 0f);
            // Optionally, you can set the width to fill the list.
            paddingElement.Width.Set(0, 1f);
            uiList.Add(paddingElement);
            return paddingElement;
        }

        protected OnOffOption AddOnOffOption(Action onClick, string title, string hoverText = "")
        {
            // Create a new option panel
            OnOffOption onOffPanel = new(title, hoverText);
            onOffPanel.OnLeftClick += (mouseEvent, element) => onClick?.Invoke();

            // Add the option to the ui list
            uiList.Add(onOffPanel);

            // Add the panel to the player panel
            return onOffPanel;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If the mouse is over your panel, capture mouse input.
            if (this.ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}
