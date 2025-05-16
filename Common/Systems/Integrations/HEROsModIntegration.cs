using System;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Configs;
using ModReloader.Helpers;
using ModReloader.UI.Elements.PanelElements.ModElements;
using ReLogic.Content;
using Terraria.UI;

namespace ModReloader.Common.Systems.Integrations
{
    [JITWhenModsEnabled("HEROsMod")]
    public class HerosModIntegration : ModSystem
    {
        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("HEROsMod", out Mod herosMod) && !Main.dedServ)
            {
                RegisterReloadButton(herosMod);
                RegisterReloadMPButton(herosMod);
                RegisterModsButton(herosMod);
                RegisterUIButton(herosMod);
                RegisterLogButton(herosMod);
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
                /* texture:             */ Ass.ButtonReloadSPHeros,
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

        private static void RegisterReloadMPButton(Mod herosMod)
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
                /* texture:             */ Ass.ButtonReloadMP,
                /* onClick action:      */ (Action)(async () =>
                                           {
                                               await ReloadUtilities.MultiPlayerMainReload();
                                           }),
                /* onPermissionChanged: */ (Action<bool>)(hasPerm =>
                                           {
                                               if (!hasPerm)
                                                   Main.NewText("⛔ You lost permission to reload mods!", Color.OrangeRed);
                                           }),
                /* tooltipFunc:         */ (Func<string>)(() => $"Reload {string.Join(", ", Conf.C.ModsToReload)}")
            );
            Log.Info("HEROsMod reloadMP button registered successfully.");
        }

        private static void RegisterModsButton(Mod herosMod)
        {
            const string Perm = "ToggleModMenu";
            herosMod.Call("AddPermission", Perm, "Enable or disable mods");

            // grab the panel instance once so both the click‑action and tooltip can use it
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            Asset<Texture2D> tex = Ass.ButtonModsHeros;

            herosMod.Call(
                "AddSimpleButton",
                Perm,
                tex,

                // on‑click: toggle visibility and bring to front
                () =>
                {
                    ModsPanel panel = sys.mainState.modsPanel;
                    bool nowOpen = !panel.GetActive();
                    panel.SetActive(nowOpen);

                    if (panel.Parent is UIElement parent)
                    {
                        panel.Remove(); parent.Append(panel);
                    }
                },

                // permission change callback
                (Action<bool>)(hasPerm =>
                {
                    if (!hasPerm) Main.NewText("⛔ You lost permission to open mods panel!", Color.OrangeRed);
                }),

                // tooltip: reflect current state
                () => sys.mainState.modsPanel.GetActive() ? "Close mod list" : "Open mod list"
            );
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // 3.  Button that toggles the UI-element demo panel
        // ──────────────────────────────────────────────────────────────────────────────
        private static void RegisterUIButton(Mod herosMod)
        {
            const string Perm = "ToggleUIPanel";
            herosMod.Call("AddPermission", Perm, "Show or hide the UI-Elements panel");

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            Asset<Texture2D> tex = Ass.ButtonUIHeros;

            herosMod.Call(
                "AddSimpleButton",
                Perm,
                tex,
                (Action)(() =>                       // click
                {
                    var panel = sys.mainState.uiElementPanel;
                    bool opened = !panel.GetActive();
                    panel.SetActive(opened);

                    if (panel.Parent is UIElement p) { panel.Remove(); p.Append(panel); }
                }),
                (Action<bool>)(hasPerm =>            // permission lost
                {
                    if (!hasPerm)
                        Main.NewText("⛔ You lost permission to use the UI-panel button!", Color.OrangeRed);
                }),
                (Func<string>)(() =>                 // tooltip
                    sys.mainState.uiElementPanel.GetActive()
                    ? "Close UI-Elements panel"
                    : "Open UI-Elements panel")
            );
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // 4.  Button that toggles the in-game log panel
        // ──────────────────────────────────────────────────────────────────────────────
        private static void RegisterLogButton(Mod herosMod)
        {
            const string Perm = "ToggleLogPanel";
            herosMod.Call("AddPermission", Perm, "Open or close the log panel");

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            Asset<Texture2D> tex = Ass.ButtonLogHeros;

            herosMod.Call(
                "AddSimpleButton",
                Perm,
                tex,
                (Action)(() =>
                {
                    var panel = sys.mainState.logPanel;
                    bool opened = !panel.GetActive();
                    panel.SetActive(opened);

                    if (panel.Parent is UIElement p) { panel.Remove(); p.Append(panel); }
                }),
                (Action<bool>)(hasPerm =>
                {
                    if (!hasPerm)
                        Main.NewText("⛔ You lost permission to open the log panel!", Color.OrangeRed);
                }),
                (Func<string>)(() =>
                    sys.mainState.logPanel.GetActive()
                    ? "Close log panel"
                    : "Open log panel")
            );
        }
    }
}