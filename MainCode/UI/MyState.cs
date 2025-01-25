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
using SkipSelect.MainCode.Other;
using Microsoft.Xna.Framework;

namespace SkipSelect.MainCode.UI
{
    public class MyState : UIState
    {

        // variables
        private bool IsRefreshButtonVisible = true;
        MyHoverButton buttonRefresh;

        public override void OnInitialize()
        {
            initButton();
        }

        private void initButton()
        {
            Config config = ModContent.GetInstance<Config>();
            if (config.EnableRefresh)
            {
                Asset<Texture2D> buttonRefreshTexture = ModContent.Request<Texture2D>("SkipSelect/MainCode/Assets/ButtonRefresh");
                MyHoverButton buttonRefresh = new(buttonRefreshTexture, "Refresh (Go to Develop Mods)");
                buttonRefresh.Width.Set(100f, 0f); // only change the size of the clickable area, not actual size of the button
                buttonRefresh.Height.Set(100f, 0f);
                buttonRefresh.Top.Set(50f, 0f);
                buttonRefresh.HAlign = 0.3f;
                buttonRefresh.OnLeftClick += RefreshButtonClicked;
                Append(buttonRefresh);
            }
        }

        public void ToggleRefreshButton()
        {
            IsRefreshButtonVisible = !IsRefreshButtonVisible;

            if (IsRefreshButtonVisible)
            {
                buttonRefresh.Deactivate();
            }
            else
            {
                buttonRefresh.Activate();
            }
        }

        private async void RefreshButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);

            // 1) Save and quit or just quit.
            Config c = ModContent.GetInstance<Config>();
            if (c.SaveWorld)
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn("Saving and quitting...");
                WorldGen.SaveAndQuit();
                int wait = c.WaitingTime; // 1000 = 1 second
                await Task.Delay(wait);
            }
            else
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn("Just quitting...");
                WorldGen.JustQuit();
            }

            // 2) Navigate to Develop Mods
            object inst = NavigateToDevelopMods();
        }

        private Object NavigateToDevelopMods()
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
                    return null;
                }

                // Get the modSources instance (Develop Mods menu)
                FieldInfo modSourcesField = interfaceType.GetField("modSources", BindingFlags.NonPublic | BindingFlags.Static);
                object modSourcesInstance = modSourcesField?.GetValue(null);

                if (modSourcesInstance == null)
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("modSources instance not found.");
                    return null;
                }

                // Get the modSourcesID
                FieldInfo modSourcesIDField = interfaceType.GetField("modSourcesID", BindingFlags.NonPublic | BindingFlags.Static);
                int modSourcesID = (int)(modSourcesIDField?.GetValue(null) ?? -1);

                if (modSourcesID == -1)
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("modSourcesID not found.");
                    return null;
                }

                // Set Main.menuMode to modSourcesID
                Main.menuMode = modSourcesID;

                ModContent.GetInstance<SkipSelect>().Logger.Warn($"Successfully navigated to Develop Mods (MenuMode: {modSourcesID}).");

                return modSourcesInstance;
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<SkipSelect>().Logger.Error($"Error navigating to Develop Mods: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }
    }
}