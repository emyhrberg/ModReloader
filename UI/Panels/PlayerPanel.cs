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
    /// A panel containing options to modify player behaviour like God,Fast,Build,etc.
    /// </summary>
    public class PlayerPanel : DraggablePanel
    {
        public PlayerPanel() : base("Player")
        {
            // Godmode
            OptionPanel godOption = new("God Mode", "Makes you invincible to all damage", true, Color.BlueViolet);
            godOption.OnLeftClick += (a, b) => PlayerCheats.ToggleGodMode();

            // Fastmode
            OptionPanel fastOption = new("Fast Mode", "Increases player speed", true, Color.Green);
            fastOption.OnLeftClick += (a, b) => PlayerCheats.ToggleFastMode();

            // Buildmode
            OptionPanel buildOption = new("Build Mode", "Infinite range, instant mining and more", true, Color.Orange);
            buildOption.OnLeftClick += (a, b) => PlayerCheats.ToggleBuildMode();

            // No Clip
            OptionPanel noClipOption = new("Noclip Mode", "Move through blocks. Hold shift to go faster, hold ctrl to go slower", true, Color.Red);
            noClipOption.OnLeftClick += (a, b) => PlayerCheats.ToggleNoClip();

            // Set checkbox positions
            godOption.Top.Set(35 + padding, 0f);
            fastOption.Top.Set(35 + 65 + padding, 0f);
            buildOption.Top.Set(35 + 65 * 2 + padding, 0f);
            noClipOption.Top.Set(35 + 65 * 3 + padding, 0f);

            // Add all content in the panel
            Append(godOption);
            Append(fastOption);
            Append(buildOption);
            Append(noClipOption);
        }
    }
}