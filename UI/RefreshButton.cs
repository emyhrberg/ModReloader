using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.src;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class RefreshButton : BaseButton
    {
        public RefreshButton(Asset<Texture2D> texture, string hoverText)
            : base(texture, hoverText)
        {
        }

        public async void HandleClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Config config = ModContent.GetInstance<Config>();

            if (config.SaveWorld)
            {
                ModContent.GetInstance<SquidTestingMod>().Logger.Warn("Saving and quitting...");
                WorldGen.SaveAndQuit();
                await Task.Delay(config.WaitingTime);
            }
            else
            {
                ModContent.GetInstance<SquidTestingMod>().Logger.Warn("Just quitting...");
                WorldGen.JustQuit();
            }

            object modSourcesInstance = NavigateToDevelopMods();

            if (config.InvokeBuildAndReload)
            {
                await Task.Delay(config.WaitingTime);
                BuildReload(modSourcesInstance);
            }
        }

        private void BuildReload(object modSourcesInstance)
        {
            var items = (System.Collections.IEnumerable)modSourcesInstance
                .GetType()
                .GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)
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

            var method = modSourceItem.GetType()
                .GetMethod("BuildAndReload", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(modSourceItem, new object[] { null, null });
        }

        private object NavigateToDevelopMods()
        {
            try
            {
                ModContent.GetInstance<SquidTestingMod>().Logger.Warn("Attempting to navigate to Develop Mods...");

                Assembly tModLoaderAssembly = typeof(Main).Assembly;
                Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");

                FieldInfo modSourcesField = interfaceType.GetField("modSources", BindingFlags.NonPublic | BindingFlags.Static);
                object modSourcesInstance = modSourcesField?.GetValue(null);

                FieldInfo modSourcesIDField = interfaceType.GetField("modSourcesID", BindingFlags.NonPublic | BindingFlags.Static);
                int modSourcesID = (int)(modSourcesIDField?.GetValue(null) ?? -1);

                Main.menuMode = modSourcesID;

                ModContent.GetInstance<SquidTestingMod>().Logger.Warn($"Successfully navigated to Develop Mods (MenuMode: {modSourcesID}).");

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
