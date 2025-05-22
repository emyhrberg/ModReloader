using System;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.BuilderToggles;
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
            string ReloadPermission = Loc.Get("ReloadButton.Text");
            string ReloadPermissionDisplay = Loc.Get("ReloadButton.HoverText", string.Join(", ", Conf.C.ModsToReload));

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
                    if (!BuilderToggleHelper.GetActive())
                    {
                        LeftClickHelper.Notify();
                        return;
                    }
                    await ReloadUtilities.SinglePlayerReload();
                }),
                /* onPermissionChanged: */ (Action<bool>)(hasPerm =>
                                           {
                                               if (!hasPerm)
                                                   Main.NewText("⛔You lost permission to reload mods!", Color.OrangeRed);
                                           }),
                /* tooltipFunc:         */ () =>
                {
                    if (ReloadUtilities.IsModsToReloadEmpty)
                        return Loc.Get("ReloadButton.HoverDescNoMods");

                    string modsToReload = string.Join(", ", Conf.C.ModsToReload);
                    Log.Info($"Reloading mods for singleplayer Heros: {modsToReload}");
                    return Loc.Get("ReloadButton.HoverText", modsToReload);
                }
            );
            Log.Info("HEROsMod reload button registered successfully.");
        }

        private static void RegisterReloadMPButton(Mod herosMod)
        {
            string ReloadPermission = Loc.Get("ReloadMPButton.Text");
            string ReloadPermissionDisplay = Helpers.Loc.Get("ReloadMPButton.HoverText", string.Join(", ", Conf.C.ModsToReload));

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
                                               if (!BuilderToggleHelper.GetActive())
                                               {
                                                   LeftClickHelper.Notify();
                                                   return;
                                               }
                                               await ReloadUtilities.MultiPlayerMainReload();
                                           }),
                /* onPermissionChanged: */ (Action<bool>)(hasPerm =>
                                           {
                                               if (!hasPerm)
                                                   Main.NewText("⛔ You lost permission to reload MP mods!", Color.OrangeRed);
                                           }),
                /* tooltipFunc:         */ () =>
                {
                    if (ReloadUtilities.IsModsToReloadEmpty)
                        return Loc.Get("ReloadMPButton.HoverDescNoMods");

                    string modsToReload = string.Join(", ", Conf.C.ModsToReload);
                    Log.Info($"Reloading mods for multiplayer Heros: {modsToReload}");
                    return Loc.Get("ReloadMPButton.HoverText", modsToReload);
                }
            );
            Log.Info("HEROsMod reloadMP button registered successfully.");
        }

        private static void RegisterModsButton(Mod herosMod)
        {
            // const string Perm = "ToggleModMenu";
            string Perm = Loc.Get("ModsButton.Text");
            string PermDisplay = Loc.Get("ModsButton.HoverDesc");
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
                    if (!BuilderToggleHelper.GetActive())
                    {
                        LeftClickHelper.Notify();
                        return;
                    }
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
                () => sys.mainState.modsPanel.GetActive() ? Loc.Get("ModsButton.HoverTooltipOn") : Loc.Get("ModsButton.HoverTooltipOff")
            );
        }

        private static void RegisterUIButton(Mod herosMod)
        {
            string Perm = Loc.Get("ModsButton.Text");
            string PermDisplay = Loc.Get("UIElementButton.HoverDescBase");

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
                    if (!BuilderToggleHelper.GetActive())
                    {
                        LeftClickHelper.Notify();
                        return;
                    }
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
                (Func<string>)(() => sys.mainState.uiElementPanel.GetActive() ? Loc.Get("UIElementButton.HoverTooltipOn") : Loc.Get("UIElementButton.HoverTooltipOff"))
            );
        }

        private static void RegisterLogButton(Mod herosMod)
        {
            // const string Perm = "ToggleLogPanel";
            string Perm = Loc.Get("LogButton.Text");
            string PermDisplay = Loc.Get("LogButton.HoverDescBase");
            herosMod.Call("AddPermission", Perm, PermDisplay);

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            Asset<Texture2D> tex = Ass.ButtonLogHeros;

            herosMod.Call(
                "AddSimpleButton",
                Perm,
                tex,
                (Action)(() =>
                {
                    if (!BuilderToggleHelper.GetActive())
                    {
                        LeftClickHelper.Notify();
                        return;
                    }
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
                    sys.mainState.logPanel.GetActive() ? Loc.Get("LogButton.HoverTooltipOn") : Loc.Get("LogButton.HoverTooltipOff"))
            );
        }
    }
}