using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Players;
using SquidTestingMod.Helpers;
using Terraria;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel containing options to modify player behavior like God, Noclip, etc.
    /// </summary>
    public class PlayerPanel : RightParentPanel
    {
        // Keep references to the options for updating them later
        private Dictionary<string, OnOffOption> options = [];

        // Slider for Max Life
        private SliderOption maxLifeSlider;

        public PlayerPanel() : base(title: "Player", scrollbarEnabled: false)
        {
            // === STATS ===
            float currentHP = MaxLife.maxLife != 0 ? MaxLife.maxLife : Main.LocalPlayer.statLifeMax2;
            currentHP = MathHelper.Clamp(currentHP, 1, 1000);
            maxLifeSlider = AddSliderOption(
                title: "Extra Life",
                min: 1,
                max: 1000,
                defaultValue: currentHP,    // pass 500
                onValueChanged: OnLifeMaxChanged,
                increment: 20,
                textSize: 1
            );
            AddPadding();

            // === ABILITIES ===
            AddHeader("Abilities");
            options["god"] = AddOnOffOption(PlayerCheatManager.ToggleGod, "God Off", "Makes you immortal");
            options["noclip"] = AddOnOffOption(PlayerCheatManager.ToggleNoclip, "Noclip Off", "Fly through blocks\nHold shift/ctrl to go faster");
            options["teleport"] = AddOnOffOption(PlayerCheatManager.ToggleTeleportMode, "Teleport Mode Off", "Right click to teleport to the mouse position");
            options["light"] = AddOnOffOption(PlayerCheatManager.ToggleLightMode, "Light Aura Off", "Light up the world around you");
            options["killAura"] = AddOnOffOption(PlayerCheatManager.ToggleKillAura, "Kill Aura Off", "Kill enemies around you");
            options["mineAura"] = AddOnOffOption(PlayerCheatManager.ToggleMineAura, "Mine Aura Off", "Mine tiles around you");
            AddSliderOption(
                title: "Mine Radius",
                min: 1,
                max: 50,
                defaultValue: 1,
                onValueChanged: value => MineAura.mineRange = (int)value,
                increment: 1,
                textSize: 0.8f
            );

            AddPadding();

            // === BUILD ===
            AddHeader("Build");
            options["placeAnywhere"] = AddOnOffOption(PlayerCheatManager.TogglePlaceAnywhere, "Place Anywhere Off", "Place blocks and walls anywhere in the air");
            options["placeFaster"] = AddOnOffOption(PlayerCheatManager.TogglePlaceFaster, "Place Faster Off", "Place tiles and walls faster");
            options["mineFaster"] = AddOnOffOption(PlayerCheatManager.ToggleMineFaster, "Mine Faster Off", "Mine tiles and walls faster");
            AddPadding();

            // === TOGGLE ALL ===
            AddHeader("Toggle All");
            options["all"] = AddOnOffOption(ToggleAll, "Off", "Toggle all player cheats on/off");
            AddPadding();
        }

        private void ToggleAll()
        {
            // Decide if we want to turn everything on or off
            bool turnOn = !PlayerCheatManager.IsAnyCheatEnabled;

            // Set all cheats to the same state
            PlayerCheatManager.SetAllCheats(turnOn);

            // Update the text of each OnOffOption
            // (These keys match exactly what we used when inserting into the dictionary.)
            options["god"].UpdateText(turnOn ? "God On" : "God Off");
            options["noclip"].UpdateText(turnOn ? "Noclip On" : "Noclip Off");
            options["teleport"].UpdateText(turnOn ? "Teleport Mode On" : "Teleport Mode Off");
            options["light"].UpdateText(turnOn ? "Light Aura On" : "Light Aura Off");
            options["killAura"].UpdateText(turnOn ? "Kill Aura On" : "Kill Aura Off");
            options["mineAura"].UpdateText(turnOn ? "Mine Aura On" : "Mine Aura Off");
            options["placeAnywhere"].UpdateText(turnOn ? "Place Anywhere On" : "Place Anywhere Off");
            options["placeFaster"].UpdateText(turnOn ? "Place Faster On" : "Place Faster Off");
            options["mineFaster"].UpdateText(turnOn ? "Mine Faster On" : "Mine Faster Off");

            // Finally, update the toggle-all button itself
            options["all"].UpdateText(turnOn ? "Off" : "On");
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
            maxLifeSlider.SetValue(fraction);
            base.Update(gameTime);
        }
    }
}
