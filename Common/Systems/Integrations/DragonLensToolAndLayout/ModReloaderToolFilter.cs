using DragonLens.Content.GUI;
using DragonLens.Content.Tools;
using DragonLens.Core.Systems.ThemeSystem;
using DragonLens.Content.Filters;
using Microsoft.Xna.Framework.Graphics;

namespace ModReloader.Common.Systems.Integrations
{
    // A filter for the tool browser that shows only tools from a specific mod (Mod Reloader)
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class ModReloaderToolFilter : Filter
    {
        public Mod mod;

        public ModReloaderToolFilter(Mod mod) : base(null, "", n => FilterByMod(n, mod))
        {
            this.mod = mod;
            isModFilter = true;
        }

        public override string Name => mod.DisplayName;

        public override string Description => mod.DisplayName;

        public static bool FilterByMod(BrowserButton button, Mod mod)
        {
            if (button is ToolBrowserButton)
            {
                var tb = button as ToolBrowserButton;

                if (tb.tool.Mod != null && tb.tool.Mod == mod)
                    return false;
            }

            return true;
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle target)
        {
            Texture2D tex = null;

            string path = $"{mod.Name}/icon_small";

            if (mod.Name == "ModLoader")
                tex = ThemeHandler.GetIcon("Customize");
            else if (ModContent.HasAsset(path))
                tex = ModContent.Request<Texture2D>(path).Value;

            if (tex != null)
            {
                int widest = tex.Width > tex.Height ? tex.Width : tex.Height;
                spriteBatch.Draw(tex, target.Center.ToVector2(), null, Color.White, 0, tex.Size() / 2f, target.Width / (float)widest, 0, 0);
            }
            else
            {
                Utils.DrawBorderString(spriteBatch, mod.DisplayName[..2], target.Center.ToVector2(), Color.White, 1, 0.5f, 0.4f);
            }
        }
    }
}
