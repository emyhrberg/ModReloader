using SquidTestingMod.Common.Systems;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel containing options to modify player behaviour like God, Fast, Build, etc.
    /// </summary>
    public class PlayerPanel : RightParentPanel
    {
        public PlayerPanel() : base("Player")
        {
            AddHeader("Player Modes");
            AddOnOffOption(PlayerCheats.ToggleGodMode, "God Mode Off", "Makes you immortal");
            AddOnOffOption(PlayerCheats.ToggleNoClip, "Noclip Mode Off", "Move through blocks.");
            AddOnOffOption(PlayerCheats.ToggleTeleportMode, "Teleport Mode Off", "Right click to teleport to the mouse position");
            AddOnOffOption(PlayerCheats.ToggleFastMode, "Fast Mode Off", "Increases speed and acceleration");
            AddOnOffOption(PlayerCheats.ToggleBuildMode, "Build Mode Off", "Infinite range, instant mining and more");
            AddOnOffOption(PlayerCheats.ToggleLightMode, "Light Mode Off", "Increases light around the player");

            AddHeader("Test1");
            AddPadding();
            AddHeader("Test2");
            AddPadding();
            AddHeader("Test3");
            AddPadding();
            AddHeader("Test4");
            AddPadding();
            AddHeader("Test5");
            AddPadding();
            AddHeader("Test6");
            AddPadding();
            AddHeader("Test7");
            AddPadding();
            AddHeader("Test8");
            AddPadding();
            AddHeader("Test9");
            AddPadding();
            AddHeader("Test10");
        }
    }
}
