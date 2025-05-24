using System;
using System.Threading.Tasks;
using ModReloader.Common.BuilderToggles;
using ModReloader.UI.Elements.ButtonElements;
using ModReloader.UI.Elements.PanelElements;
using Terraria.UI;

namespace ModReloader.Common.Systems.Integrations.HerosMod
{
    [JITWhenModsEnabled("HEROsMod")]
    public sealed class HerosModIntegration : ModSystem
    {
        private const string PermReloadSP = "ReloadSP";
        private const string PermReloadMP = "ReloadMP";
        private const string PermModsPanel = "ModsPanel";
        private const string PermUIElementPanel = "UIElementPanel";
        private const string PermLogPanel = "LogPanel";

        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("HEROsMod", out Mod herosMod) && !Main.dedServ)
            {
                RegisterReloadSPButton(herosMod);
                RegisterReloadMPButton(herosMod);
                RegisterModsButton(herosMod);
                RegisterUIButton(herosMod);
                RegisterLogButton(herosMod);
            }
        }

        private static void RegisterReloadSPButton(Mod herosMod)
        {
            herosMod.Call("AddPermission", PermReloadSP, "Reloads mods");
            herosMod.Call(
                "AddSimpleButton",
                PermReloadSP,
                Ass.ButtonReloadSPHeros,
                GuardedAsync(ReloadUtilities.SinglePlayerReload),
                (Action<bool>)(hasPerm =>
                {
                    if (!hasPerm)
                    {
                        Main.NewText($"⛔ You lost permission to use the {PermReloadSP} button!", ColorHelper.CalamityRed);
                    }
                }),
                (Func<string>)(() => GetReloadTooltip())
            );
        }

        private static void RegisterReloadMPButton(Mod herosMod)
        {
            herosMod.Call("AddPermission", PermReloadMP, Loc.Get("ReloadMPButton.HoverText", string.Join(", ", Conf.C.ModsToReload)));
            herosMod.Call(
                "AddSimpleButton",
                PermReloadMP,
                Ass.ButtonReloadMP,
                GuardedAsync(ReloadUtilities.MultiPlayerMainReload),
                (Action<bool>)(hasPerm => PermissionChanged(hasPerm, PermReloadMP)),
                (Func<string>)(() => GetReloadTooltip())
            );
        }

        private static void RegisterModsButton(Mod herosMod)
        {
            herosMod.Call("AddPermission", PermModsPanel, Loc.Get("ModsButton.HoverDesc"));
            herosMod.Call(
            "AddSimpleButton",
            PermModsPanel,
            Ass.ButtonModsHeros,
            () => TogglePanel(ModContent.GetInstance<MainSystem>().mainState.modsPanel),
            (Action<bool>)(hasPerm => PermissionChanged(hasPerm, PermModsPanel)),
            (Func<string>)(() => GetTooltip(ModContent.GetInstance<MainSystem>().mainState.modsPanel, "ModsButton"))
            );
        }

        private static void RegisterUIButton(Mod herosMod)
        {
            herosMod.Call("AddPermission", PermUIElementPanel, Loc.Get("UIElementButton.HoverDescBase"));
            herosMod.Call(
            "AddSimpleButton",
            PermUIElementPanel,
            Ass.ButtonUIHeros,
            () => TogglePanel(ModContent.GetInstance<MainSystem>().mainState.uiElementPanel),
            (Action<bool>)(hasPerm => PermissionChanged(hasPerm, PermUIElementPanel)),
            (Func<string>)(() => GetTooltip(ModContent.GetInstance<MainSystem>().mainState.uiElementPanel, "UIElementButton"))
            );
        }

        private static void RegisterLogButton(Mod herosMod)
        {
            herosMod.Call("AddPermission", PermLogPanel, Loc.Get("LogButton.HoverDescBase"));
            herosMod.Call(
            "AddSimpleButton",
            PermLogPanel,
            Ass.ButtonLogHeros,
            () => TogglePanel(ModContent.GetInstance<MainSystem>().mainState.logPanel),
            (Action<bool>)(hasPerm => PermissionChanged(hasPerm, PermLogPanel)),
            (Func<string>)(() => GetTooltip(ModContent.GetInstance<MainSystem>().mainState.logPanel, "LogButton"))
            );
        }

        #region Helpers

        // reload action helper
        private static Action GuardedAsync(Func<Task> taskFunc)
        {
            return async () =>
            {
                if (!BuilderToggleHelper.GetActive())
                {
                    LeftClickHelper.Notify();
                    return;
                }

                await taskFunc().ConfigureAwait(false);
            };
        }

        // reload tooltip helper
        private static string GetReloadTooltip()
        {
            if (ReloadUtilities.IsModsToReloadEmpty)
                return Loc.Get("ReloadButton.HoverDescNoMods");

            return Loc.Get("ReloadButton.HoverText", string.Join(", ", Conf.C.ModsToReload));
        }

        // toggle panel helper
        private static void TogglePanel(BasePanel panel)
        {
            if (!BuilderToggleHelper.GetActive())
            {
                LeftClickHelper.Notify();
                return;
            }
            bool nowOpen = !panel.GetActive();
            panel.SetActive(nowOpen);

            if (panel.Parent is UIElement parent)
            {
                panel.Remove();
                parent.Append(panel);
            }
        }

        // permission helper
        private static void PermissionChanged(bool hasPerm, string permissionName)
        {
            if (!hasPerm)
            {
                Main.NewText($"⛔ You lost permission to use the {permissionName} button!", ColorHelper.CalamityRed);
                Log.Info($"You lost permission for {permissionName} button. You cannot use it anymore.");
            }
            else
            {
                Main.NewText($"✅ You regained permission to use the {permissionName} button!", Color.LightGreen);
                Log.Info($"You regained permission for {permissionName} button. You can use it again.");
            }
        }

        // tooltip helper
        private static string GetTooltip(BasePanel panel, string key)
        {
            return panel.GetActive() ? Loc.Get($"{key}.HoverTooltipOn") : Loc.Get($"{key}.HoverTooltipOff");
        }

        #endregion
    }
}
