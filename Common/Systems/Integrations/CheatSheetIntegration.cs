using CheatSheet;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Configs;
using ModReloader.Helpers;
using ModReloader.UI.Elements.PanelElements;
using ReLogic.Content;
using Terraria.UI;

namespace ModReloader.Common.Systems.Integrations
{
    [JITWhenModsEnabled("CheatSheet")]
    public class CheatSheetIntegration : ModSystem
    {
        public override void PostSetupContent()
        {
            // Only run if CheatSheet is loaded and we’re on client
            if (ModLoader.TryGetMod("CheatSheet", out Mod csMod) && !Main.dedServ)
            {
                // Check if we also have Heros. If so, just skip here since
                // we only want to register the buttons once (to Heros).
                // I hope this works, it may not work because of the order of the mods loaded.
                // I believe they are loaded in alphabetical order.
                // Update: It worked once during testing, so its fine for now.
                // Update 2: Maybe we should use sortAfter or sortBefore in build.txt to ensure mods load in the right order?
                if (ModLoader.TryGetMod("HEROsMod", out Mod herosMod))
                {
                    Log.Info("HEROsMod is loaded, skipping CheatSheet integration.");
                    return;
                }

                RegisterReloadButton();
                RegisterReloadMPButton();
                RegisterModsButton();
                RegisterUIButton();
                RegisterLogButton();
            }
        }

        private void RegisterReloadButton()
        {
            // Load the button icon
            Asset<Texture2D> reloadTex = Ass.ButtonReloadSPCS;

            // Build the mods‐to‐reload string
            string modsToReload = string.Join(", ", Conf.C.ModsToReload);

            // Register the button: click runs your reload, tooltip shows the list
            CheatSheetInterface.RegisterButton(
                texture: reloadTex,
                buttonClickedAction: async () => await ReloadUtilities.SinglePlayerReload(),
                tooltip: () => $"Reload {modsToReload}"
            );
        }

        private void RegisterReloadMPButton()
        {
            // Load the button icon
            Asset<Texture2D> reloadTex = Ass.ButtonReloadMP;

            // Build the mods‐to‐reload string
            string modsToReload = string.Join(", ", Conf.C.ModsToReload);

            // Register the button: click runs your reload, tooltip shows the list
            CheatSheetInterface.RegisterButton(
                texture: reloadTex,
                buttonClickedAction: async () => await ReloadUtilities.MultiPlayerMainReload(),
                tooltip: () => $"Reload {modsToReload}"
            );
        }

        private static void RegisterModsButton()
        {
            // Grab the panel once so both lambdas can see it
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            CheatSheetInterface.RegisterButton(
                texture: Ass.ButtonMods,

                // Click – toggle panel + bring to front if opening
                buttonClickedAction: () =>
                {
                    BasePanel panel = sys.mainState.modsPanel;

                    bool nowOpen = !panel.GetActive();
                    panel.SetActive(nowOpen);

                    if (nowOpen && panel.Parent is UIElement parent)
                    {
                        panel.Remove();
                        parent.Append(panel);          // move to top layer
                    }
                },
                tooltip: () => sys.mainState.modsPanel.GetActive() ? "Close mod list" : "Open mod list"
            );
        }

        private static void RegisterUIButton()
        {
            // Grab the panel once so both lambdas can see it
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            CheatSheetInterface.RegisterButton(
                texture: Ass.ButtonUICS,

                // Click – toggle panel + bring to front if opening
                buttonClickedAction: () =>
                {
                    BasePanel panel = sys.mainState.uiElementPanel;

                    bool nowOpen = !panel.GetActive();
                    panel.SetActive(nowOpen);

                    if (nowOpen && panel.Parent is UIElement parent)
                    {
                        panel.Remove();
                        parent.Append(panel);          // move to top layer
                    }
                },

                // Tooltip – reflect current state
                tooltip: () => sys.mainState.uiElementPanel.GetActive() ? "Close UIElement panel" : "Open UIElement panel"
            );
        }

        private static void RegisterLogButton()
        {
            // Grab the panel once so both lambdas can see it
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            CheatSheetInterface.RegisterButton(
                texture: Ass.ButtonLogCS,

                // Click – toggle panel + bring to front if opening
                buttonClickedAction: () =>
                {
                    BasePanel panel = sys.mainState.logPanel;

                    bool nowOpen = !panel.GetActive();
                    panel.SetActive(nowOpen);

                    if (nowOpen && panel.Parent is UIElement parent)
                    {
                        panel.Remove();
                        parent.Append(panel);          // move to top layer
                    }
                },
                tooltip: () => sys.mainState.logPanel.GetActive() ? "Close log panel" : "Open log panel"
            );
        }
    }
}
