using System.Collections.Generic;
using System.Threading.Tasks;
using ErkysModdingUtilities.Common.Configs;
using ErkysModdingUtilities.Common.Players;
using ErkysModdingUtilities.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using static ErkysModdingUtilities.UI.Elements.Option;

namespace ErkysModdingUtilities.UI.Elements
{
    /// <summary>
    /// A panel containing options to modify player behavior like God, Noclip, etc.
    /// </summary>
    public class PlayerPanel : OptionPanel
    {
        public List<Option> cheatOptions = new();
        private Option toggleAll;

        public PlayerPanel() : base(title: "Player", scrollbarEnabled: true)
        {
            AddPadding(5);
            AddHeader("Player", hover: "Modify player abilities");

            toggleAll = AddOption("Toggle All", ToggleAll, "Toggle all player abilities on/off");

            // Automatically create an option for each cheat
            foreach (var cheat in PlayerCheatManager.Cheats)
            {
                var option = AddOption(
                    text: cheat.Name,
                    leftClick: cheat.Toggle,
                    hover: cheat.Description
                );
                cheatOptions.Add(option);
            }
            AddSlider(
                title: "Mine Radius",
                min: 1,
                max: 50,
                defaultValue: 3,
                onValueChanged: value => MineAura.mineRange = (int)value,
                increment: 1,
                hover: "Mine all tiles around you when moving (not MP-supported)",
                textSize: 0.9f
            );
            AddPadding();

            AddHeader("Actions");
            ActionOption clear = new(ClearInventory, "Clear Inventory", "Clears your inventory except favorited items");
            uiList.Add(clear);
            ActionOption reveal = new(revealMap, "Reveal Map", "The world map becomes completely explored for this character permanently");
            uiList.Add(reveal);
        }

        private void revealMap()
        {
            // Ensure it's only running on the client side
            if (Main.netMode == NetmodeID.Server)
                return;

            // task because its computationally expensive to  fully reveal the map and run on the main thread
            Task.Run(() =>
            {
                byte brightness = (byte)MathHelper.Clamp(255f * (100 / 100f), 1f, 255f);

                for (int i = 0; i < Main.maxTilesX; i++)
                {
                    for (int j = 0; j < Main.maxTilesY; j++)
                    {
                        if (WorldGen.InWorld(i, j, 0))
                        {
                            Main.Map.Update(i, j, brightness);
                        }
                    }
                }

                Main.refreshMap = true;
                if (Conf.LogToChat) Main.NewText("Map fully revealed!", 255, 255, 255);
            });
        }

        private void ClearInventory()
        {
            // start at 10 to skip the hotbar
            for (int i = 10; i < Main.LocalPlayer.inventory.Length; i++)
            {
                Item item = Main.LocalPlayer.inventory[i];
                if (!item.favorited)
                {
                    item.TurnToAir(false);
                }
            }
            if (Conf.LogToChat) Main.NewText("Inventory cleared", byte.MaxValue, byte.MaxValue, byte.MaxValue);
        }

        private void ToggleAll()
        {
            // Decide whether to enable or disable everything
            bool anyOff = PlayerCheatManager.Cheats.Exists(c => c.GetValue() == false);
            // If at least one is off, we enable them all; if all are on, we disable them all
            bool newVal = anyOff;
            PlayerCheatManager.SetAllCheats(newVal);

            // Update each option’s UI text
            State newState = newVal ? State.Enabled : State.Disabled;
            foreach (Option option in cheatOptions)
            {
                option.SetState(newState);
            }
            // Set itself
            toggleAll.SetState(newState);
        }
    }
}
