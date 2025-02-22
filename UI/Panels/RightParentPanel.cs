using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Panels;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A parent panel
    /// for Player, Debug, World panels
    /// </summary>
    public abstract class RightParentPanel : DraggablePanel
    {
        // Variables
        protected const float ItemPadding = 3f;
        protected float currentTop = 35 + ItemPadding;

        public RightParentPanel(string header) : base(header)
        {
        }

        protected OptionPanel AddOptionPanel(string title, string description, bool checkBox, Color color, Action onClick)
        {
            // Create a new option panel
            OptionPanel optionPanel = new(title, description, checkBox, color);

            // Set the action to be performed when the panel is clicked.
            // If onClickAction is null, log a message instead.
            optionPanel.OnLeftClick += (mouseEvent, element) =>
            {
                if (onClick != null)
                    onClick();
                else
                    Log.Info("No left click for " + title + " button");
            };

            // Set the position of the panel
            optionPanel.Top.Set(currentTop, 0f);
            currentTop += 30 + ItemPadding; // Increase with the height of the OptionPanel + ItemPadding

            // Add the panel to the player panel
            Append(optionPanel);
            return optionPanel;
        }

    }
}
