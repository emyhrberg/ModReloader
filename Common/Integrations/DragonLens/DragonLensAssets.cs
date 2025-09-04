using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace ModReloader.Common.Integrations.DragonLens
{
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    internal static class DragonLensAssets
    {
        public static class Misc
        {
            public static Asset<Texture2D> GlowAlpha = ModContent.Request<Texture2D>("DragonLens/Assets/Misc/GlowAlpha");
        }

    }
}
