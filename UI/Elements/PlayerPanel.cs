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

        // Slider for Max Life
        private SliderOption maxLifeSlider;

        public PlayerPanel() : base(title: "Player", scrollbarEnabled: true)
        {
            // === ABILITIES ===
            AddHeader("Abilities");
            options["god"] = AddOnOffOption(PlayerCheatManager.ToggleGod, "God Off", "Makes you immortal");
            options["noclip"] = AddOnOffOption(PlayerCheatManager.ToggleNoclip, "Noclip Off", "Fly through blocks\nHold shift/ctrl to go faster");
            options["teleport"] = AddOnOffOption(PlayerCheatManager.ToggleTeleportMode, "Teleport Mode Off", "Right click to teleport to the mouse position");
            options["enemiesIgnore"] = AddOnOffOption(PlayerCheatManager.ToggleEnemiesIgnore, "Enemies Ignore Off", "Enemies ignore you due to low player aggro");
            options["light"] = AddOnOffOption(PlayerCheatManager.ToggleLightMode, "Light Aura Off", "Light up the world around you");
            options["killAura"] = AddOnOffOption(PlayerCheatManager.ToggleKillAura, "Kill Aura Off", "Insta-kill all enemies that touch you");
            options["mineAura"] = AddOnOffOption(PlayerCheatManager.ToggleMineAura, "Mine Aura Off", "Mine tiles around you (not MP-supported)");
            SliderOption mineRadius = new(
                title: "Mine Radius",
                min: 1,
                max: 50,
                defaultValue: 0,
                onValueChanged: value => MineAura.mineRange = (int)value,
                increment: 1,
                hover: "Mine all tiles around you when moving (not MP-supported)"
            );
            uiList.Add(mineRadius);

            float currentHP = MaxLife.maxLife != 0 ? MaxLife.maxLife : Main.LocalPlayer.statLifeMax2;
            currentHP = MathHelper.Clamp(currentHP, 1, 1000);
            maxLifeSlider = new(
                title: "Extra Life",
                min: 1,
                max: 1000,
                defaultValue: currentHP,    // pass 500
                onValueChanged: OnLifeMaxChanged,
                increment: 20
            );
            uiList.Add(maxLifeSlider);

            AddPadding();

            // === BUILD ===
            AddHeader("Build");
            options["placeAnywhere"] = AddOnOffOption(PlayerCheatManager.TogglePlaceAnywhere, "Place Anywhere Off", "Place blocks and walls anywhere in the air");
            options["placeFaster"] = AddOnOffOption(PlayerCheatManager.TogglePlaceFaster, "Place Faster Off", "Place tiles and walls faster");
            options["mineFaster"] = AddOnOffOption(PlayerCheatManager.ToggleMineFaster, "Mine Faster Off", "Mine tiles and walls faster");
            AddPadding();

            // === TOGGLE ALL ===
            AddHeader("Toggle All");
            options["all"] = AddOnOffOption(ToggleAll, "Off", "Toggle all player abilities on/off");
            AddPadding();

            // === BUTTON OPTIONS ===
            AddHeader("Options");
            AddOnOffOption(clearInventory, "Clear Inventory", "Clears your inventory except favorited items");
            AddOnOffOption(revealMap, "Reveal Map", "The world map becomes completely explored for this character permanently");
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
            options["teleport"].UpdateText(PlayerCheatManager.TeleportMode ? "Teleport Mode On" : "Teleport Mode Off");
            options["enemiesIgnore"].UpdateText(PlayerCheatManager.LowAggro ? "Enemies Ignore On" : "Enemies Ignore Off");
            options["light"].UpdateText(PlayerCheatManager.LightMode ? "Light Aura On" : "Light Aura Off");
            options["killAura"].UpdateText(PlayerCheatManager.KillAura ? "Kill Aura On" : "Kill Aura Off");
            options["mineAura"].UpdateText(PlayerCheatManager.MineAura ? "Mine Aura On" : "Mine Aura Off");
            options["placeAnywhere"].UpdateText(PlayerCheatManager.PlaceAnywhere ? "Place Anywhere On" : "Place Anywhere Off");
            options["placeFaster"].UpdateText(PlayerCheatManager.PlaceFaster ? "Place Faster On" : "Place Faster Off");
            options["mineFaster"].UpdateText(PlayerCheatManager.MineFaster ? "Mine Faster On" : "Mine Faster Off");
            options["all"].UpdateText(PlayerCheatManager.IsAnyCheatEnabled ? "Off" : "On");
        }

        // Callback for the Max Life slider
        // This callback interprets the normalized slider value back into an absolute number (0–2000).
        private void OnLifeMaxChanged(float value)
        {
            // Update the static max life variable
            MaxLife.maxLife = (int)value;
            // Main.LocalPlayer.statLifeMax2 = MaxLife.maxLife;
        }

        // During Update, keep the slider in sync if the player's life is changed elsewhere.
        public override void Update(GameTime gameTime)
        {
            // Normalize the maxLife value from the range [1, 1000] to a fraction (0 to 1)
            float fraction = (MaxLife.maxLife - 1) / 999f;
            maxLifeSlider?.SetValue(fraction);
            base.Update(gameTime);
        }
    }
}
