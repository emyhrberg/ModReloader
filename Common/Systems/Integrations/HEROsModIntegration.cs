using System;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using ModHelper.Common.Configs;
using Terraria.UI;
using ModHelper.UI.Elements.ModElements;

namespace ModHelper.Common.Systems.Integrations
{
    [JITWhenModsEnabled("HEROsMod")]
    public class HerosModIntegration : ModSystem
    {
        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("HEROsMod", out Mod herosMod) && !Main.dedServ)
            {
                RegisterReloadButton(herosMod);
                RegisterModsButton(herosMod);
            }
        }

        private static void RegisterReloadButton(Mod herosMod)
        {
            const string ReloadPermission = "ReloadMods";
            const string ReloadPermissionDisplay = "Reload Selected Mods";

            // Register a permission so admins can gate this button
            herosMod.Call(
                "AddPermission",
                ReloadPermission,
                ReloadPermissionDisplay
            );

            // Add the button itself
            herosMod.Call(
                "AddSimpleButton",
                /* permissionName:      */ ReloadPermission,
                /* texture:             */ Ass.ButtonReloadSP,
                /* onClick action:      */ (Action)(async () =>
                                           {
                                               await ReloadUtilities.SinglePlayerReload();
                                           }),
                /* onPermissionChanged: */ (Action<bool>)(hasPerm =>
                                           {
                                               if (!hasPerm)
                                                   Main.NewText("⛔ You lost permission to reload mods!", Color.OrangeRed);
                                           }),
                /* tooltipFunc:         */ (Func<string>)(() => $"Reload {string.Join(", ", Conf.C.ModsToReload)}")
            );
            Log.Info("HEROsMod reload button registered successfully.");
        }

        private static void RegisterModsButton(Mod herosMod)
        {
            const string Perm = "ToggleModMenu";
            herosMod.Call("AddPermission", Perm, "Enable or disable mods");

            // grab the panel instance once so both the click‑action and tooltip can use it
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            Asset<Texture2D> tex = Ass.ButtonMods;

            herosMod.Call(
                "AddSimpleButton",
                Perm,
                tex,

                // on‑click: toggle visibility and bring to front
                (Action)(() =>
                {
                    ModsPanel panel = sys.mainState.modsPanel;
                    bool nowOpen = !panel.GetActive();
                    panel.SetActive(nowOpen);

                    if (panel.Parent is UIElement parent) { 
                        panel.Remove(); parent.Append(panel); 
                    }
                }),

                // permission change callback
                (Action<bool>)(hasPerm =>
                {
                    if (!hasPerm) Main.NewText("⛔ You lost permission to open mods panel!", Color.OrangeRed);
                }),

                // tooltip: reflect current state
                (Func<string>)(() => sys.mainState.modsPanel.GetActive() ? "Close mod menu" : "Open mod menu")
            );
        }
    }
}