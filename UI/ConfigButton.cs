using System;
using System.Linq;
using System.Reflection;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ConfigButton : BaseButton
    {

        public ConfigButton(Asset<Texture2D> texture, string hoverText)
            : base(texture, hoverText)
        {
        }

        public void HandleClick(UIMouseEvent evt, UIElement listeningElement)
        {
            try
            {
                // Get the UIModConfig instance
                var modConfigUI = ModContent.GetInstance<Mod>().GetType().Assembly
                    .GetTypes()
                    .FirstOrDefault(t => t.Name == "UIModConfig");

                if (modConfigUI == null)
                {
                    Log.Warn("UIModConfig class not found.");
                    return;
                }

                // Get the SetMod method
                var setModMethod = modConfigUI.GetMethod("SetMod", BindingFlags.Instance | BindingFlags.NonPublic);

                if (setModMethod == null)
                {
                    Log.Warn("SetMod method not found.");
                    return;
                }

                // Create an instance of UIModConfig
                var modConfigInstance = Activator.CreateInstance(modConfigUI);

                // Get the mod instance
                var modInstance = ModContent.GetInstance<SquidTestingMod>();

                // Invoke the SetMod method
                setModMethod.Invoke(modConfigInstance, new object[] { modInstance });

                // Set the UI state to the mod config UI
                Main.MenuUI.SetState((UIState)modConfigInstance);

                Log.Info("Opened config for SquidTestingMod.");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to open config: {ex.Message}");
            }
        }
    }
}