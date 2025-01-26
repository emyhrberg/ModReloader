using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SkipSelect.Core.System
{
    public class GodModeSystem : ModSystem
    {
        public ModKeybind ToggleGodModeKeybind;

        public override void Load()
        {
            // Register the keybind for toggling God Mode
            ToggleGodModeKeybind = KeybindLoader.RegisterKeybind(Mod, "Toggle God Mode", "H");
        }

        public override void Unload()
        {
            ToggleGodModeKeybind = null;
        }

        public override void PostUpdateInput()
        {
            // Check if the keybind is pressed and toggle God Mode
            if (ToggleGodModeKeybind != null && ToggleGodModeKeybind.JustPressed)
            {
                Main.LocalPlayer.GetModPlayer<GodModePlayer>().ToggleGodMode();
            }
        }

        public override void PostDrawInterface(SpriteBatch sb)
        {
            drawGodText(sb);
        }

        private void drawGodText(SpriteBatch spriteBatch)
        {
            var godMode = Main.LocalPlayer.GetModPlayer<GodModePlayer>();

            if (godMode.IsGodModeEnabled)
            {
                StringBuilder text = new("God (H)");
                Vector2 position = Main.LocalPlayer.Center.ToScreenPosition() + new Vector2(-32, 20); // Below the player
                var font = FontAssets.MouseText.Value;
                spriteBatch.DrawString(font, text, position, Color.Orange);
            }
        }
    }
}