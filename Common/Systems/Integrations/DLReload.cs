using DragonLens.Core.Systems.ThemeSystem;
using DragonLens.Core.Systems.ToolSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Systems.Integrations
{
    [ExtendsFromMod("DragonLens")]
    public class DLReload : Tool
    {
        public override string IconKey => "Reload";

        public override string DisplayName => "Reload";

        public override string Description => $"Reloads {string.Join(", ", Conf.C.ModsToReload)}";

        public override async void OnActivate()
        {
            await ReloadUtilities.SinglePlayerReload();
        }

        public override void DrawIcon(SpriteBatch spriteBatch, Rectangle position)
        {
            Texture2D icon = ThemeHandler.GetIcon("Reload");

            float scale = 1;

            if (icon.Width > position.Width || icon.Height > position.Height)
                scale = icon.Width > icon.Height ? position.Width / icon.Width : position.Height / icon.Height;

            spriteBatch.Draw(icon, position.Center(), null, Color.White, 0, icon.Size() / 2f, scale, 0, 0);
        }
    }
}
