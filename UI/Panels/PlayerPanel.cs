using System.Collections.Generic;
using SquidTestingMod.Common.Players;
using SquidTestingMod.Common.Systems;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel containing options to modify player behaviour like God, Fast, Build, etc.
    /// </summary>
    public class PlayerPanel : RightParentPanel
    {
        // Keep references to the options for updating them later
        private Dictionary<string, OnOffOption> options = new Dictionary<string, OnOffOption>();

        public PlayerPanel() : base(title: "Player", scrollbarEnabled: false)
        {
            // Player options
            AddHeader("Player");
            options["god"] = AddOnOffOption(PlayerCheatManager.ToggleGod, "God Off", "Makes you immortal");
            options["noclip"] = AddOnOffOption(PlayerCheatManager.ToggleNoclip, "Noclip Off", "Disable gravity and fly through blocks\nHold shift to go faster");
            options["teleport"] = AddOnOffOption(PlayerCheatManager.ToggleTeleportMode, "Click To Teleport Off", "Right click to teleport to the mouse position");
            AddPadding();

            AddHeader("Build");
            options["useFaster"] = AddOnOffOption(PlayerCheatManager.ToggleUseFaster, "Use Faster Off", "Place blocks, walls, and mine faster");
            options["placeAnywhere"] = AddOnOffOption(PlayerCheatManager.TogglePlaceAnywhere, "Place Anywhere Off", "Place blocks and walls anywhere in the air");
            AddPadding();

            AddHeader("Misc");
            options["invisible"] = AddOnOffOption(PlayerCheatManager.ToggleInvisibleToEnemies, "Invisible To Enemies Off", "Enemies will not attack you");
            options["light"] = AddOnOffOption(PlayerCheatManager.ToggleLightMode, "Light Mode Off", "Light up the world around you");
            AddPadding();

            AddHeader("Toggle All");
            options["all"] = AddOnOffOption(ToggleAll, "Off", "Toggle all player cheats on/off \nExcept noclip");
            AddPadding();
        }

        private void ToggleAll()
        {
            // Determine if we should enable or disable all
            bool turnOn = !PlayerCheatManager.IsAnyCheatEnabled;

            // Set all to the same state
            PlayerCheatManager.SetAllCheats(turnOn);

            // Update UI text
            options["god"].UpdateText(turnOn ? "God On" : "God Off");
            // options["noclip"].UpdateText(turnOn ? "Noclip On" : "Noclip Off");
            options["teleport"].UpdateText(turnOn ? "Click To Teleport On" : "Click To Teleport Off");
            options["useFaster"].UpdateText(turnOn ? "Use Faster On" : "Use Faster Off");
            options["placeAnywhere"].UpdateText(turnOn ? "Place Anywhere On" : "Place Anywhere Off");
            options["invisible"].UpdateText(turnOn ? "Invisible To Enemies On" : "Invisible To Enemies Off");
            options["light"].UpdateText(turnOn ? "Light Mode On" : "Light Mode Off");
            options["all"].UpdateText(turnOn ? "On" : "Off");
        }
    }
}