// public static mod system class that loads assets


using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace SquidTestingMod.src
{
    public class LoadAssets : ModSystem
    {
        public static Asset<Texture2D> PlayerGlow;

        public override void Load()
        {
            base.Load();
        }

        public override void PostSetupContent()
        {
            PlayerGlow = ModContent.Request<Texture2D>("SquidTestingMod/Assets/PlayerGlow");
        }
    }
}