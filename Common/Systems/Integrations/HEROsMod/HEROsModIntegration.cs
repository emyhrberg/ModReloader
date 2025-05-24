using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.BuilderToggles;
using ModReloader.UI.Elements.PanelElements;
using ReLogic.Content;
using Terraria.UI;

namespace ModReloader.Common.Systems.Integrations.HerosMod
{
    [JITWhenModsEnabled("HEROsMod")]
    public sealed class HerosModIntegration : ModSystem
    {
        // Permission keys for HERO's Mod buttons.
        private const string PERM_RELOAD_SP = "ReloadSP";
        private const string PERM_RELOAD_MP = "ReloadMP";
        private const string PERM_OPEN_MODS = "OpenModsPanel";
        private const string PERM_OPEN_UI = "OpenUIPanel";
        private const string PERM_OPEN_LOG = "OpenLogPanel";

        public override void PostSetupContent()
        {
            if (Main.dedServ || !ModLoader.TryGetMod("HEROsMod", out var heros))
                return;

            RegisterButton(
                heros,
                PERM_RELOAD_SP,
                "ReloadButton.Text",
                Ass.ButtonReloadSPHeros,
                async () => await SafeRunAsync(ReloadUtilities.SinglePlayerReload),
                ReloadTooltip,
                "You lost permission to reload mods!",
                "You regained permission to reload mods!");

            RegisterButton(
                heros,
                PERM_RELOAD_MP,
                "ReloadMPButton.Text",
                Ass.ButtonReloadMP,
                async () => await SafeRunAsync(ReloadUtilities.MultiPlayerMainReload),
                ReloadMPTooltip,
                "You lost permission to reload MP mods!",
                "You regained permission to reload MP mods!");

            RegisterButton(
                heros,
                PERM_OPEN_MODS,
                "ModsButton.HoverDesc",
                Ass.ButtonModsHeros,
                TogglePanel(() => ModContent.GetInstance<MainSystem>().mainState.modsPanel),
                ModsTooltip,
                "You lost permission to open mods panel!",
                "You regained permission to open mods panel!");

            RegisterButton(
                heros,
                PERM_OPEN_UI,
                "UIElementButton.HoverDescBase",
                Ass.ButtonUIHeros,
                TogglePanel(() => ModContent.GetInstance<MainSystem>().mainState.uiElementPanel),
                UITooltip,
                "You lost permission to use the UI‑panel button!",
                "You regained permission to use the UI‑panel button!");

            RegisterButton(
                heros,
                PERM_OPEN_LOG,
                "LogButton.HoverDescBase",
                Ass.ButtonLogHeros,
                TogglePanel(() => ModContent.GetInstance<MainSystem>().mainState.logPanel),
                LogTooltip,
                "You lost permission to open the log panel!",
                "You regained permission to open the log panel!");
        }

        private static void RegisterButton(
            Mod heros,
            string permKey,
            string displayLocKey,
            Asset<Texture2D> icon,
            Action onClick,
            Func<string> tooltip,
            string loseMsg,
            string? gainMsg = null)
        {
            heros.Call("AddPermission", permKey, Loc.Get(displayLocKey));

            heros.Call(
                "AddSimpleButton",
                permKey,
                icon,
                onClick,
                (Action<bool>)(hasPerm =>
                {
                    if (!hasPerm)
                        Main.NewText(loseMsg, Color.OrangeRed);
                    else if (gainMsg != null)
                        Main.NewText(gainMsg, Color.LightGreen);
                }),
                tooltip);
        }

        private static async Task SafeRunAsync(Func<Task> job)
        {
            if (!BuilderToggleHelper.GetActive())
            {
                LeftClickHelper.Notify();
                return;
            }
            await job();
        }

        private static Action TogglePanel(Func<BasePanel> panelProvider) => () =>
        {
            if (!BuilderToggleHelper.GetActive())
            {
                LeftClickHelper.Notify();
                return;
            }
            BasePanel panel = panelProvider();
            bool open = !panel.GetActive();
            panel.SetActive(open);

            if (panel.Parent is UIElement parent)
            {
                panel.Remove();
                parent.Append(panel);
            }
        };

        #region Tooltips
        private static string ReloadTooltip() => ReloadUtilities.IsModsToReloadEmpty ?
            Loc.Get("ReloadButton.HoverDescNoMods") :
            Loc.Get("ReloadButton.HoverText", string.Join(", ", Conf.C.ModsToReload));

        private static string ReloadMPTooltip() => ReloadUtilities.IsModsToReloadEmpty ?
            Loc.Get("ReloadMPButton.HoverDescNoMods") :
            Loc.Get("ReloadMPButton.HoverText", string.Join(", ", Conf.C.ModsToReload));

        private static string ModsTooltip() => PanelTooltip(
            () => ModContent.GetInstance<MainSystem>().mainState.modsPanel,
            "ModsButton.HoverTooltipOn",
            "ModsButton.HoverTooltipOff");

        private static string UITooltip() => PanelTooltip(
            () => ModContent.GetInstance<MainSystem>().mainState.uiElementPanel,
            "UIElementButton.HoverTooltipOn",
            "UIElementButton.HoverTooltipOff");

        private static string LogTooltip() => PanelTooltip(
            () => ModContent.GetInstance<MainSystem>().mainState.logPanel,
            "LogButton.HoverTooltipOn",
            "LogButton.HoverTooltipOff");

        private static string PanelTooltip(Func<BasePanel> panelProvider, string onKey, string offKey)
            => panelProvider().GetActive() ? Loc.Get(onKey) : Loc.Get(offKey);
        #endregion
    }
}
