using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Players;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;

namespace SquidTestingMod.UI.Elements
{
    /// <summary>
    /// A panel containing options to modify player behavior like God, Noclip, etc.
    /// </summary>
    public class PlayerPanel : OptionPanel
    {
        // Keep references to the options for updating them later
        public Dictionary<string, OnOffOption> options = [];

        public PlayerPanel() : base(title: "Player", scrollbarEnabled: true)
        {
            Draggable = true;
            // === ABILITIES ===
            AddPadding(5);
            AddHeader("Abilities");
            options["god"] = new OnOffOption(PlayerCheatManager.ToggleGod, "God Off", "Makes you immortal");
            options["light"] = new OnOffOption(PlayerCheatManager.ToggleLightMode, "Light Off", "Light up the world around you");
            options["noclip"] = new OnOffOption(PlayerCheatManager.ToggleNoclip, "Noclip Off", "Fly through blocks\nHold shift/ctrl to go faster");
            options["killAura"] = new OnOffOption(PlayerCheatManager.ToggleKillAura, "Kill Aura Off", "Insta-kill all enemies that touch you");
            options["mineAura"] = new OnOffOption(PlayerCheatManager.ToggleMineAura, "Mine Aura Off", "Mine tiles around you (not MP-supported)");
            SliderOption mineRadius = new(
                title: "Mine Radius",
                min: 1,
                max: 50,
                defaultValue: 0,
                onValueChanged: value => MineAura.mineRange = (int)value,
                increment: 1,
                hover: "Mine all tiles around you when moving (not MP-supported)"
            );
            // add to uilist
            uiList.Add(options["god"]);
            uiList.Add(options["light"]);
            uiList.Add(options["noclip"]);
            uiList.Add(options["killAura"]);
            uiList.Add(options["mineAura"]);
            uiList.Add(mineRadius);
            AddPadding();

            // === BUILD ===
            AddHeader("Build");
            options["buildAnywhere"] = new OnOffOption(PlayerCheatManager.ToggleBuildAnywhere, "Build Anywhere Off", "Place blocks and walls anywhere in the air");
            options["buildFaster"] = new OnOffOption(PlayerCheatManager.ToggleBuildFaster, "Build Faster Off", "Place tiles and walls faster, also mine faster");
            uiList.Add(options["buildAnywhere"]);
            uiList.Add(options["buildFaster"]);
            AddPadding();

            // === TOGGLE ALL ===
            AddHeader("Toggle All");
            options["all"] = new OnOffOption(ToggleAll, "Off", "Toggle all player abilities on/off");
            uiList.Add(options["all"]);
            AddPadding();

            // === BUTTON OPTIONS ===
            AddHeader("Options");
            OnOffOption clearInv = new(clearInventory, "Clear Inventory", "Clears your inventory except favorited items");
            OnOffOption revealMapOption = new(revealMap, "Reveal Map", "The world map becomes completely explored for this character permanently");
            uiList.Add(clearInv);
            uiList.Add(revealMapOption);
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
                Main.NewText("Map fully revealed!", 255, 255, 255);
            });
        }

        private void clearInventory()
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
            Main.NewText("Inventory cleared", byte.MaxValue, byte.MaxValue, byte.MaxValue);
        }

        public void ToggleAll()
        {
            // Decide if we want to turn everything on or off
            bool turnOn = !PlayerCheatManager.IsAnyCheatEnabled;

            // Set all cheats to the same state
            PlayerCheatManager.SetAllCheats(turnOn);

            // Update the text of each OnOffOption
            // (These keys match exactly what we used when inserting into the dictionary.)
            RefreshCheatTexts();
        }

        // In PlayerPanel.cs, add a method:
        public void RefreshCheatTexts()
        {
            options["god"].UpdateText(PlayerCheatManager.God ? "God On" : "God Off");
            options["noclip"].UpdateText(PlayerCheatManager.Noclip ? "Noclip On" : "Noclip Off");
            options["light"].UpdateText(PlayerCheatManager.LightMode ? "Light On" : "Light Off");
            options["killAura"].UpdateText(PlayerCheatManager.KillAura ? "Kill Aura On" : "Kill Aura Off");
            options["mineAura"].UpdateText(PlayerCheatManager.MineAura ? "Mine Aura On" : "Mine Aura Off");
            options["buildAnywhere"].UpdateText(PlayerCheatManager.BuildAnywhere ? "Build Anywhere On" : "Build Anywhere Off");
            options["buildFaster"].UpdateText(PlayerCheatManager.BuildFaster ? "Build Faster On" : "Build Faster Off");
            options["all"].UpdateText(PlayerCheatManager.IsAnyCheatEnabled ? "Off" : "On");
        }
    }
}
