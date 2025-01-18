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
        public override void OnInitialize()
        {
            // Load button texture
            Asset<Texture2D> buttonRefreshTexture = ModContent.Request<Texture2D>("SkipSelect/MainCode/ButtonRefresh");

            // Create the button
            MyHoverButton buttonRefresh = new(buttonRefreshTexture, "Refresh (Go to Develop Mods)");

            // Set button size and position
            buttonRefresh.Width.Set(100f, 0f);
            buttonRefresh.Height.Set(100f, 0f);
            buttonRefresh.Top.Set(10f, 0f);
            buttonRefresh.Left.Set(10f, 0f);
            buttonRefresh.HAlign = 0.5f;

            // Add click event
            buttonRefresh.OnLeftClick += CloseButtonClicked;

            // Add the button to the UIState
            Append(buttonRefresh);
        }


        private async void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            ModContent.GetInstance<SkipSelect>().Logger.Warn("Saving and quitting...");

            // Save and quit
            WorldGen.SaveAndQuit();

            // Wait for save to complete (adjust timing if necessary)
            await Task.Delay(3000);

            // Navigate to Mod Browser
            NavigateToModBrowser();
        }

        private void NavigateToModBrowser()
        {
            try
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn("Attempting to navigate to Mod Browser...");

                // Access the Interface type
                Assembly tModLoaderAssembly = typeof(Main).Assembly;
                Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");

                if (interfaceType == null)
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("Interface class not found.");
                    return;
                }

                DebugInterfaceMembers();

                // Get the modBrowser instance
                FieldInfo modBrowserField = interfaceType.GetField("modBrowser", BindingFlags.NonPublic | BindingFlags.Static);
                object modBrowserInstance = modBrowserField?.GetValue(null);

                if (modBrowserInstance == null)
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("modBrowser instance not found.");
                    return;
                }

                // Get the modBrowserID
                FieldInfo modBrowserIDField = interfaceType.GetField("modBrowserID", BindingFlags.NonPublic | BindingFlags.Static);
                int modBrowserID = (int)(modBrowserIDField?.GetValue(null) ?? -1);

                if (modBrowserID == -1)
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("modBrowserID not found.");
                    return;
                }

                // Set Main.menuMode to modBrowserID
                Main.menuMode = modBrowserID;

                ModContent.GetInstance<SkipSelect>().Logger.Warn($"Successfully navigated to Mod Browser (MenuMode: {modBrowserID}).");
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<SkipSelect>().Logger.Error($"Error navigating to Mod Browser: {ex.Message}\n{ex.StackTrace}");
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
