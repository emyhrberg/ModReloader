using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader;
using System.Reflection;
using System;
using System.Threading.Tasks;
using SquidTestingMod.src;
using log4net.Core;
using log4net;

namespace SquidTestingMod.UI
{
    public class RefreshUIState : UIState
    {
        // variables
        private bool IsRefreshButtonVisible = true;
        RefreshUIHoverButton buttonRefresh;

        // logger
        ILog logger;

        public override void OnInitialize()
        {
            InitButton();
            logger = ModContent.GetInstance<SquidTestingMod>().Logger;
        }

        private void InitButton()
        {
            Config config = ModContent.GetInstance<Config>();
            if (config.EnableRefreshButton)
            {
                Asset<Texture2D> buttonRefreshTexture = ModContent.Request<Texture2D>("SquidTestingMod/Assets/ButtonRefresh");
                RefreshUIHoverButton buttonRefresh = new(buttonRefreshTexture, "Refresh (Go to Develop Mods)");
                buttonRefresh.Width.Set(100f, 0f); // only changes the size of the clickable area, not actual size of the button
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

            // await Task.Delay(5000);

            // 3) Click Build & Reload
            // FindBuildReload(inst);
            // await Task.Delay(5000);

            if (c.InvokeBuildAndReload)
            {
                int wait2 = c.WaitingTime;
                await Task.Delay(wait2);
                BuildReload(inst);
            }
        }

        private void BuildReload(object modSourcesInstance)
        {
            // Get the _items field from modSourcesInstance and take the first UIModSourceItem.
            var items = (System.Collections.IEnumerable)modSourcesInstance
                .GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(modSourcesInstance);
            object modSourceItem = null;
            foreach (var item in items)
            {
                if (item.GetType().Name == "UIModSourceItem")
                {
                    modSourceItem = item;
                    break;
                }
            }
            // Invoke the internal BuildAndReload method on the UIModSourceItem.
            var method = modSourceItem.GetType().GetMethod("BuildAndReload", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(modSourceItem, [null, null]);
        }

        private void FindBuildReload(object inst)
        {
            // Find the Build + Reload button
            var items = (System.Collections.IEnumerable)inst.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(inst);
            object buildReloadButton = null;
            foreach (var item in items)
            {
                if (item.GetType().Name == "UIModSourceItem")
                {
                    var children = (System.Collections.IEnumerable)item.GetType().GetProperty("Children", BindingFlags.Public | BindingFlags.Instance).GetValue(item);
                    foreach (var child in children)
                    {
                        if (child.GetType().Name.Contains("UIAutoScaleTextTextPanel"))
                        {
                            if ((string)child.GetType().GetField("_text", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(child) == "Build + Reload")
                            {
                                buildReloadButton = child;
                                break;
                            }
                        }
                    }
                }
                if (buildReloadButton != null) break;
            }
            logger.Info("Found 'Build + Reload' button.");
        }


        private object NavigateToDevelopMods()
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