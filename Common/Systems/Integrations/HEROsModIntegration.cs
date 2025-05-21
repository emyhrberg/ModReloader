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
            string ReloadPermission = LocalizationHelper.GetText("ReloadButton.Text");
            string ReloadPermissionDisplay = LocalizationHelper.GetText("ReloadButton.HoverText", string.Join(", ", Conf.C.ModsToReload));

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
                /* onClick action:      */ (Action)(async () => await ReloadUtilities.SinglePlayerReload()),
                /* onPermissionChanged: */ (Action<bool>)(hasPerm =>
                                           {
                                               if (!hasPerm)
                                                   Main.NewText("⛔You lost permission to reload mods!", Color.OrangeRed);
                                           }),
                /* tooltipFunc:         */ () =>
                {
                    if (ReloadUtilities.IsModsToReloadEmpty)
                        return LocalizationHelper.GetText("ReloadButton.HoverDescNoMods");

                    string modsToReload = string.Join(", ", Conf.C.ModsToReload);
                    Log.Info($"Reloading mods for singleplayer Heros: {modsToReload}");
                    return LocalizationHelper.GetText("ReloadButton.HoverText", modsToReload);
                }
            );
            Log.Info("HEROsMod reload button registered successfully.");
        }

        private static void RegisterReloadMPButton(Mod herosMod)
        {
            string ReloadPermission = LocalizationHelper.GetText("ReloadMPButton.Text");
            string ReloadPermissionDisplay = Helpers.LocalizationHelper.GetText("ReloadMPButton.HoverText", string.Join(", ", Conf.C.ModsToReload));

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
                /* onClick action:      */ (Action)(async () => await ReloadUtilities.MultiPlayerMainReload()),
                /* onPermissionChanged: */ (Action<bool>)(hasPerm =>
                                           {
                                               if (!hasPerm)
                                                   Main.NewText("⛔ You lost permission to reload MP mods!", Color.OrangeRed);
                                           }),
                /* tooltipFunc:         */ () =>
                {
                    if (ReloadUtilities.IsModsToReloadEmpty)
                        return LocalizationHelper.GetText("ReloadMPButton.HoverDescNoMods");

                    string modsToReload = string.Join(", ", Conf.C.ModsToReload);
                    Log.Info($"Reloading mods for multiplayer Heros: {modsToReload}");
                    return LocalizationHelper.GetText("ReloadMPButton.HoverText", modsToReload);
                }
            );
            Log.Info("HEROsMod reloadMP button registered successfully.");
        }

        private static void RegisterModsButton(Mod herosMod)
        {
            // const string Perm = "ToggleModMenu";
            string Perm = LocalizationHelper.GetText("ModsButton.Text");
            string PermDisplay = LocalizationHelper.GetText("ModsButton.HoverDesc");
            herosMod.Call("AddPermission", Perm, PermDisplay);

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
                () => sys.mainState.modsPanel.GetActive() ? LocalizationHelper.GetText("ModsButton.HoverTooltipOn") : LocalizationHelper.GetText("ModsButton.HoverTooltipOff")
            );
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // 3.  Button that toggles the UI-element demo panel
        // ──────────────────────────────────────────────────────────────────────────────
        private static void RegisterUIButton(Mod herosMod)
        {
            string Perm = LocalizationHelper.GetText("ModsButton.Text");
            string PermDisplay = LocalizationHelper.GetText("UIElementButton.HoverDescBase");

            // const string Perm = "ToggleUIPanel";
            herosMod.Call("AddPermission", Perm, PermDisplay);

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
                (Func<string>)(() => sys.mainState.uiElementPanel.GetActive() ? LocalizationHelper.GetText("UIElementButton.HoverTooltipOn") : LocalizationHelper.GetText("UIElementButton.HoverTooltipOff"))
            );
        }

        // ──────────────────────────────────────────────────────────────────────────────
        // 4.  Button that toggles the in-game log panel
        // ──────────────────────────────────────────────────────────────────────────────
        private static void RegisterLogButton(Mod herosMod)
        {
            // const string Perm = "ToggleLogPanel";
            string Perm = LocalizationHelper.GetText("LogButton.Text");
            string PermDisplay = LocalizationHelper.GetText("LogButton.HoverDescBase");
            herosMod.Call("AddPermission", Perm, PermDisplay);

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
                    sys.mainState.logPanel.GetActive() ? LocalizationHelper.GetText("LogButton.HoverTooltipOn") : LocalizationHelper.GetText("LogButton.HoverTooltipOff"))
            );
        }
    }
}