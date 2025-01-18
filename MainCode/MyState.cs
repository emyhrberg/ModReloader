using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.UI;
using Terraria.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Reflection;
using System;
using System.Threading.Tasks;

namespace SkipSelect.MainCode
{
    public class MyState : UIState
    {

        // Variables
        private bool Visible;

        public override void OnInitialize()
        {
            Config config = ModContent.GetInstance<Config>();
            if (config.EnableRefresh)
            {
                Asset<Texture2D> buttonRefreshTexture = ModContent.Request<Texture2D>("SkipSelect/MainCode/ButtonRefresh");
                MyHoverButton buttonRefresh = new(buttonRefreshTexture, "Refresh (Go to Develop Mods)");
                buttonRefresh.Width.Set(100f, 0f);
                buttonRefresh.Height.Set(100f, 0f);
                buttonRefresh.Top.Set(10f, 0f);
                buttonRefresh.HAlign = 0.5f;
                buttonRefresh.OnLeftClick += CloseButtonClicked;
                Append(buttonRefresh);
            }
        }

        private async void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            ModContent.GetInstance<SkipSelect>().Logger.Warn("Saving and quitting...");

            // Save and quit
            WorldGen.SaveAndQuit();

            // Wait for save to complete (adjust timing if necessary)
            await Task.Delay(3000);

            // Navigate to Develop Mods
            NavigateToDevelopMods();
        }

        private void NavigateToDevelopMods()
        {
            try
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn("Attempting to navigate to Develop Mods...");

                // Access the Interface type
                Assembly tModLoaderAssembly = typeof(Main).Assembly;
                Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");

                if (interfaceType == null)
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("Interface class not found.");
                    return;
                }

                // Get the modSources instance (Develop Mods menu)
                FieldInfo modSourcesField = interfaceType.GetField("modSources", BindingFlags.NonPublic | BindingFlags.Static);
                object modSourcesInstance = modSourcesField?.GetValue(null);

                if (modSourcesInstance == null)
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("modSources instance not found.");
                    return;
                }

                // Get the modSourcesID
                FieldInfo modSourcesIDField = interfaceType.GetField("modSourcesID", BindingFlags.NonPublic | BindingFlags.Static);
                int modSourcesID = (int)(modSourcesIDField?.GetValue(null) ?? -1);

                if (modSourcesID == -1)
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("modSourcesID not found.");
                    return;
                }

                // Set Main.menuMode to modSourcesID
                Main.menuMode = modSourcesID;

                ModContent.GetInstance<SkipSelect>().Logger.Warn($"Successfully navigated to Develop Mods (MenuMode: {modSourcesID}).");
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<SkipSelect>().Logger.Error($"Error navigating to Develop Mods: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void DebugInterfaceMembers()
        {
            Assembly tModLoaderAssembly = typeof(Main).Assembly;
            Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");

            if (interfaceType == null)
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn("Interface class not found.");
                return;
            }

            // Log all fields
            foreach (var field in interfaceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn($"Field: {field.Name} ({field.FieldType.Name})");
            }

            // Log all properties
            foreach (var property in interfaceType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn($"Property: {property.Name} ({property.PropertyType.Name})");
            }
        }
    }
}
