//using Microsoft.Xna.Framework.Graphics;
//using ReLogic.Content;
//using Terraria.UI;
//using Terraria.Audio;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;
//using System.Reflection;
//using System;
//using System.Threading.Tasks;
//using SkipSelect.MainCode.Other;
//using Microsoft.Xna.Framework;

//namespace SkipSelect.MainCode.UI
//{
//    public class MyState : UIState
//    {

//        // Variables
//        private bool Visible;

//        public override void OnInitialize()
//        {
//            initButton();
//        }

//        private void initButton()
//        {
//            Config config = ModContent.GetInstance<Config>();
//            if (config.EnableRefresh)
//            {
//                Asset<Texture2D> buttonRefreshTexture = ModContent.Request<Texture2D>("SkipSelect/MainCode/Assets/ButtonRefresh");
//                MyHoverButton buttonRefresh = new(buttonRefreshTexture, "Refresh (Go to Develop Mods)");
//                buttonRefresh.Width.Set(100f, 0f);
//                buttonRefresh.Height.Set(100f, 0f);
//                buttonRefresh.Top.Set(10f, 0f);
//                buttonRefresh.HAlign = 0.5f;
//                buttonRefresh.OnLeftClick += RefreshButtonClicked;
//                Append(buttonRefresh);
//            }
//        }

//        private async void RefreshButtonClicked(UIMouseEvent evt, UIElement listeningElement)
//        {
//            // init config
//            Config config = ModContent.GetInstance<Config>();
//            int wait = config.WaitingTime;
//            int w = 1000;
//            SoundEngine.PlaySound(SoundID.MenuClose);
//            ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("Saving and quitting...");

//            // 1) Save and quit
//            WorldGen.JustQuit();

//            // 2) Navigate to Develop Mods
//            object inst = NavigateToDevelopMods();
//            await Task.Delay(w);

//            // 3) Interact with Develop Mods menu
//            InteractWithModMenu(inst);
//        }

//        private void InteractWithModMenu(object modSourcesInstance)
//        {
//            try
//            {
//                ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("Inspecting Develop Mods menu...");

//                // Access _items field
//                FieldInfo itemsField = modSourcesInstance.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
//                var items = itemsField.GetValue(modSourcesInstance) as System.Collections.IEnumerable;

//                foreach (var item in items)
//                {
//                    if (item.GetType().Name == "UIModSourceItem")
//                    {
//                        ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("Found UIModSourceItem.");
//                        InspectAndClickBuildReload(item); // Inspect child elements and click Build + Reload
//                        return;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                ModContent.GetInstance<SkipSelectSimplified>().Logger.Error($"Error interacting with Develop Mods menu: {ex.Message}\n{ex.StackTrace}");
//            }
//        }

//        private void InspectBuildReloadButton(object button)
//        {
//            try
//            {
//                ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("Inspecting 'Build + Reload' button fields and properties...");

//                // Inspect fields
//                foreach (var field in button.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
//                {
//                    ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn($"Field: {field.Name}, Type: {field.FieldType.Name}, Value: {field.GetValue(button)}");
//                }

//                // Inspect properties
//                foreach (var property in button.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
//                {
//                    try
//                    {
//                        ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn($"Property: {property.Name}, Type: {property.PropertyType.Name}, Value: {property.GetValue(button)}");
//                    }
//                    catch
//                    {
//                        ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn($"Property: {property.Name}, Type: {property.PropertyType.Name}, Value: <Inaccessible>");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                ModContent.GetInstance<SkipSelectSimplified>().Logger.Error($"Error inspecting 'Build + Reload' button: {ex.Message}\n{ex.StackTrace}");
//            }
//        }

//        private void InspectAndClickBuildReload(object modItem)
//        {
//            try
//            {
//                // Access the Children property
//                Type modItemType = modItem.GetType();
//                PropertyInfo childrenProperty = modItemType.GetProperty("Children", BindingFlags.Public | BindingFlags.Instance);
//                var children = childrenProperty.GetValue(modItem) as System.Collections.IEnumerable;

//                // Iterate over children to find the Build + Reload button
//                foreach (var child in children)
//                {
//                    // Check if the child is a UIAutoScaleTextTextPanel
//                    if (child.GetType().Name.Contains("UIAutoScaleTextTextPanel"))
//                    {
//                        // Attempt to find its text
//                        FieldInfo textField = child.GetType().GetField("_text", BindingFlags.NonPublic | BindingFlags.Instance);
//                        string buttonText = textField?.GetValue(child) as string;


//                        if (buttonText == "Build + Reload")
//                        {
//                            ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("Found 'Build + Reload' button.");
//                            InspectBuildReloadButton(buttonText);

//                            // Locate OnLeftClick handler
//                            FieldInfo onClickField = child.GetType().GetField("OnLeftClick", BindingFlags.NonPublic | BindingFlags.Instance);
//                            if (onClickField != null)
//                            {
//                                var onClickHandler = onClickField.GetValue(child);
//                                ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("Located OnLeftClick handler for 'Build + Reload' button.");

//                                // Attempt to invoke the handler
//                                if (onClickHandler != null)
//                                {
//                                    MethodInfo invokeMethod = onClickHandler.GetType().GetMethod("Invoke");
//                                    if (invokeMethod != null)
//                                    {
//                                        ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("Invoking 'Build + Reload' button...");
//                                        invokeMethod.Invoke(onClickHandler, new object[] { null, null });
//                                        ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("'Build + Reload' button invoked successfully.");
//                                        return;
//                                    }
//                                    else
//                                    {
//                                        ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("Invoke method not found for OnLeftClick handler.");
//                                    }
//                                }
//                                else
//                                {
//                                    ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("OnLeftClick handler is null.");
//                                }
//                            }
//                            else
//                            {
//                                ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("OnLeftClick handler field not found for 'Build + Reload' button.");
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                ModContent.GetInstance<SkipSelectSimplified>().Logger.Error($"Error inspecting and clicking Build + Reload: {ex.Message}\n{ex.StackTrace}");
//            }
//        }

//        private Object NavigateToDevelopMods()
//        {
//            try
//            {
//                ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("Attempting to navigate to Develop Mods...");

//                // Access the Interface type
//                Assembly tModLoaderAssembly = typeof(Main).Assembly;
//                Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");

//                if (interfaceType == null)
//                {
//                    ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("Interface class not found.");
//                    return null;
//                }

//                // Get the modSources instance (Develop Mods menu)
//                FieldInfo modSourcesField = interfaceType.GetField("modSources", BindingFlags.NonPublic | BindingFlags.Static);
//                object modSourcesInstance = modSourcesField?.GetValue(null);

//                if (modSourcesInstance == null)
//                {
//                    ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("modSources instance not found.");
//                    return null;
//                }

//                // Get the modSourcesID
//                FieldInfo modSourcesIDField = interfaceType.GetField("modSourcesID", BindingFlags.NonPublic | BindingFlags.Static);
//                int modSourcesID = (int)(modSourcesIDField?.GetValue(null) ?? -1);

//                if (modSourcesID == -1)
//                {
//                    ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn("modSourcesID not found.");
//                    return null;
//                }

//                // Set Main.menuMode to modSourcesID
//                Main.menuMode = modSourcesID;

//                ModContent.GetInstance<SkipSelectSimplified>().Logger.Warn($"Successfully navigated to Develop Mods (MenuMode: {modSourcesID}).");

//                return modSourcesInstance;
//            }
//            catch (Exception ex)
//            {
//                ModContent.GetInstance<SkipSelectSimplified>().Logger.Error($"Error navigating to Develop Mods: {ex.Message}\n{ex.StackTrace}");
//                return null;
//            }
//        }
//    }
//}