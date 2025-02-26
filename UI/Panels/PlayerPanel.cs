using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Panels;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel containing options to modify player behaviour like God, Fast, Build, etc.
    /// </summary>
    public class PlayerPanel : RightParentPanel
    {
        public PlayerPanel() : base("Player")
        {
            // Add option panels
            AddOptionPanel(
                title: "God Mode",
                description: "Makes you immortal",
                checkBox: true,
                color: Color.BlueViolet,
                onClick: PlayerCheats.ToggleGodMode
            );
            AddOptionPanel(
                title: "Noclip Mode",
                description: "Move through blocks.",
                checkBox: true,
                color: Color.BlueViolet,
                onClick: PlayerCheats.ToggleNoClip
            );
            AddOptionPanel(
                title: "Light Mode",
                description: "Increases light around the player",
                checkBox: true,
                color: Color.BlueViolet,
                onClick: PlayerCheats.ToggleLightMode
            );
            AddOptionPanel(
                title: "Teleport Mode",
                description: "Right click to teleport to the mouse position",
                checkBox: true,
                color: Color.BlueViolet,
                onClick: PlayerCheats.ToggleTeleportMode
            );
            AddOptionPanel(
                title: "Fast Mode",
                description: "Increases speed and acceleration",
                checkBox: true,
                color: Color.BlueViolet,
                onClick: PlayerCheats.ToggleFastMode
            );
            AddOptionPanel(
                title: "Build Mode",
                description: "Infinite range, instant mining and more",
                checkBox: true,
                color: Color.BlueViolet,
                onClick: PlayerCheats.ToggleBuildMode
            );
        }
    }
}
