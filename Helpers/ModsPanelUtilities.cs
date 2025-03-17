using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Helpers
{
    public class ModsPanelUtilities : ModSystem
    {
        public static List<Asset<Texture2D>> ModIcons = [];

        public override void PostSetupContent()
        {
            foreach (Mod mod in ModLoader.Mods)
            {
                Log.Info("Loading icon for " + mod.Name);
                if (mod.FileExists("icon.png"))
                {
                    Asset<Texture2D> icon = mod.Assets.Request<Texture2D>("icon", AssetRequestMode.ImmediateLoad);
                    ModIcons.Add(icon);
                    Log.Info("Loaded icon for " + mod.Name);
                }
            }
        }

        private void QueueLoadModIcon(ModItem modItem, string iconPath)
        {
            if (File.Exists(iconPath))
            {
                Task.Run(() =>
                {
                    try
                    {
                        // Queue loading the icon on the main thread
                        Main.QueueMainThreadAction(() =>
                        {
                            try
                            {
                                using (FileStream stream = File.OpenRead(iconPath))
                                {
                                    // Create the texture on the main thread
                                    Texture2D iconTexture = Texture2D.FromStream(Main.graphics.GraphicsDevice, stream);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Error loading icon on main thread for");
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Failed to queue icon loading for");
                    }
                });
            }
        }
    }
}