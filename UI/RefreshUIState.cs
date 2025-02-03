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
using SquidTestingMod.Core.System;

namespace SquidTestingMod.Core.UI
{
    public class RefreshUIState : UIState
    {

        // variables
        private bool IsRefreshButtonVisible = true;
        RefreshUIHoverButton buttonRefresh;

        public override void OnInitialize()
        {
            initButton();
        }

        private void initButton()
        {
            Config config = ModContent.GetInstance<Config>();
            if (config.EnableRefresh)
            {
                Asset<Texture2D> buttonRefreshTexture = ModContent.Request<Texture2D>("SquidTestingMod/Core/Assets/ButtonRefresh");
                RefreshUIHoverButton buttonRefresh = new(buttonRefreshTexture, "Refresh (Go to Develop Mods)");
                buttonRefresh.Width.Set(100f, 0f); // only change the size of the clickable area, not actual size of the button
                buttonRefresh.Height.Set(100f, 0f);
                buttonRefresh.HAlign = 0.5f; // slightly to the right
                buttonRefresh.VAlign = 0.7f; // bottom of the screen
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
            // SoundEngine.PlaySound(SoundID.MenuClose);

            // 1) Save and quit or just quit.
            Config c = ModContent.GetInstance<Config>();
            if (c.SaveWorld)
            {
                ModContent.GetInstance<SquidTestingMod>().Logger.Warn("Saving and quitting...");
                WorldGen.SaveAndQuit();
                int wait = c.WaitingTime; // 1000 = 1 second
                await Task.Delay(wait);
            }
            else
            {
                ModContent.GetInstance<SquidTestingMod>().Logger.Warn("Just quitting...");
                WorldGen.JustQuit();
            }

            // 2) Navigate to Develop Mods
            object inst = NavigateToDevelopMods();
        }

        private Object NavigateToDevelopMods()
        {
            try
            {
                ModContent.GetInstance<SquidTestingMod>().Logger.Warn("Attempting to navigate to Develop Mods...");

                // Access the Interface type
                Assembly tModLoaderAssembly = typeof(Main).Assembly;
                Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");

                // Get the modSources instance (Develop Mods menu)
                FieldInfo modSourcesField = interfaceType.GetField("modSources", BindingFlags.NonPublic | BindingFlags.Static);
                object modSourcesInstance = modSourcesField?.GetValue(null);

                // Get the modSourcesID
                FieldInfo modSourcesIDField = interfaceType.GetField("modSourcesID", BindingFlags.NonPublic | BindingFlags.Static);
                int modSourcesID = (int)(modSourcesIDField?.GetValue(null) ?? -1);

                // Set Main.menuMode to modSourcesID
                Main.menuMode = modSourcesID;

                ModContent.GetInstance<SquidTestingMod>().Logger.Warn($"Successfully navigated to Develop Mods (MenuMode: {modSourcesID}). | modSourcesInstance: {modSourcesInstance} (type: {modSourcesInstance.GetType()})");

                return modSourcesInstance;
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<SquidTestingMod>().Logger.Error($"Error navigating to Develop Mods: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }
    }
}