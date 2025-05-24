using System;
using System.Threading.Tasks;
using ModReloader.Common.BuilderToggles;
using ModReloader.UI.Elements.PanelElements;
using Terraria.UI;

namespace ModReloader.Common.Systems.Integrations.HerosMod
{
    [JITWhenModsEnabled("HEROsMod")]
    public class HerosModIntegration : ModSystem
    {
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
            const string Perm = "ReloadSP";

            herosMod.Call("AddPermission", Perm, Loc.Get("ReloadButton.HoverText", string.Join(", ", Conf.C.ModsToReload)));
            herosMod.Call(
                "AddSimpleButton",
                Perm,
                Ass.ButtonReloadSPHeros,
                GuardedAsync(ReloadUtilities.SinglePlayerReload),
                (Action<bool>)(hasPerm => PermissionChanged(hasPerm, Perm)),
                (Func<string>)(() => GetReloadTooltip())
            );
        }

        private static void RegisterReloadMPButton(Mod herosMod)
        {
            const string Perm = "ReloadMP";

            herosMod.Call("AddPermission", Perm, Loc.Get("ReloadMPButton.HoverText", string.Join(", ", Conf.C.ModsToReload)));
            herosMod.Call(
                "AddSimpleButton",
                Perm,
                Ass.ButtonReloadMP,
                GuardedAsync(ReloadUtilities.MultiPlayerMainReload),
                (Action<bool>)(hasPerm => PermissionChanged(hasPerm, Perm)),
                (Func<string>)(() => GetReloadTooltip())
            );
        }

        private static void RegisterModsButton(Mod herosMod)
        {
            const string Perm = "ModsPanel";

            herosMod.Call("AddPermission", Perm, Loc.Get("ModsButton.HoverDesc"));
            herosMod.Call(
            "AddSimpleButton",
            Perm,
            Ass.ButtonModsHeros,
            () => TogglePanel(ModContent.GetInstance<MainSystem>().mainState.modsPanel),
            (Action<bool>)(hasPerm => PermissionChanged(hasPerm, "ModsPanel")),
            (Func<string>)(() => GetTooltip(ModContent.GetInstance<MainSystem>().mainState.modsPanel, "ModsButton"))
            );
        }

        private static void RegisterUIButton(Mod herosMod)
        {
            const string Perm = "UIElementPanel";

            herosMod.Call("AddPermission", Perm, Loc.Get("UIElementButton.HoverDescBase"));
            herosMod.Call(
            "AddSimpleButton",
            Perm,
            Ass.ButtonUIHeros,
            () => TogglePanel(ModContent.GetInstance<MainSystem>().mainState.uiElementPanel),
            (Action<bool>)(hasPerm => PermissionChanged(hasPerm, "UIElementPanel")),
            (Func<string>)(() => GetTooltip(ModContent.GetInstance<MainSystem>().mainState.uiElementPanel, "UIElementButton"))
            );
        }

        private static void RegisterLogButton(Mod herosMod)
        {
            const string Perm = "LogPanel";

            herosMod.Call("AddPermission", Perm, Loc.Get("LogButton.HoverDescBase"));
            herosMod.Call(
            "AddSimpleButton",
            Perm,
            Ass.ButtonLogHeros,
            () => TogglePanel(ModContent.GetInstance<MainSystem>().mainState.logPanel),
            (Action<bool>)(hasPerm => PermissionChanged(hasPerm, "LogPanel")),
            (Func<string>)(() => GetTooltip(ModContent.GetInstance<MainSystem>().mainState.logPanel, "LogButton"))
            );
        }

        #region  helpers

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

            string modsToReload = string.Join(", ", Conf.C.ModsToReload);
            return Loc.Get("ReloadButton.HoverText", modsToReload);
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
                Main.NewText($"⛔ You lost permission to use the {permissionName} button!", Color.OrangeRed);
                Log.Info($"Permission for {permissionName} button was lost. Please check HEROsMod permissions.");
            }
            else
            {
                Main.NewText($"✅ You regained permission to use the {permissionName} button!", Color.LightGreen);
                Log.Info($"Permission for {permissionName} button was granted. You can now use it.");
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