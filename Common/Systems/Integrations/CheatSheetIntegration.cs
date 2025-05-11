using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CheatSheet;
using System;
using ModHelper.Helpers;
using ModHelper.Common.Configs;
using Terraria.UI;
using ModHelper.UI.Elements.PanelElements.ModElements;

namespace ModHelper.Common.Systems.Integrations
{
    [JITWhenModsEnabled("CheatSheet")]
    public class CheatSheetIntegration : ModSystem
    {
        public override void PostSetupContent()
        {
            // Only run if CheatSheet is loaded and we’re on client
            if (ModLoader.TryGetMod("CheatSheet", out Mod csMod) && !Main.dedServ)
            {
                RegisterReloadButton();
                RegisterModsButton();
            }
        }

        private void RegisterReloadButton()
        {
            // Load the button icon
            Asset<Texture2D> reloadTex = Ass.ButtonReloadSP;

            // Build the mods‐to‐reload string
            string modsToReload = string.Join(", ", Conf.C.ModsToReload);

            // Register the button: click runs your reload, tooltip shows the list
            CheatSheetInterface.RegisterButton(
                texture: reloadTex,
                buttonClickedAction: async () => await ReloadUtilities.SinglePlayerReload(),
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
                    if (!sys.mainState.AreButtonsShowing) return;
                    ModsPanel panel = sys.mainState.modsPanel;

                    bool nowOpen = !panel.GetActive();
                    panel.SetActive(nowOpen);

                    if (nowOpen && panel.Parent is UIElement parent)
                    {
                        panel.Remove();
                        parent.Append(panel);          // move to top layer
                    }

                    // (optional) highlight your in‑game button
                    sys.mainState.modsButton.ParentActive = nowOpen;
                },

                // Tooltip – reflect current state
                tooltip: () => sys.mainState.modsPanel.GetActive() ? "Close mod menu" : "Open mod menu"
            );
        }
    }
}
