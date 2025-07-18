using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.UI.Elements.MainMenuElements;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.Common.Systems.Hooks.MainMenu;

internal sealed class MainMenuState : UIState
{
    // Colors
    private static readonly Color Normal = new(173, 173, 198);
    private static readonly Color Hover = new(237, 246, 255);

    // Position
    private float verticalSpacing = 23f;
    private float currentY = 15f;

    // Elements
    private TooltipPanel tooltipPanel;
    private UIText tooltipText;

    #region constructor
    public MainMenuState()
    {
        // Null checks
        if (Conf.C is null || !Conf.C.ShowMainMenuInfo)
            return;

        // Extra spacing if other big menu mods are loaded
        if (ModLoader.HasMod("TerrariaOverhaul") || ModLoader.HasMod("Terramon"))
        {
            currentY += 205f;
            verticalSpacing = 20f; // tighter spacing to try to fit with terramon lol
        }
        if (ModLoader.HasMod("CompatChecker"))
            currentY += 30f; // move everything down a bit to fit compat checker elements at the top

        // Helpers
        string headerModName = $"{ModContent.GetInstance<ModReloader>().DisplayName} v{ModContent.GetInstance<ModReloader>().Version}";
        string reloadHoverMods = ReloadUtilities.IsModsToReloadEmpty ? "No mods selected" : string.Join(",", Conf.C.ModsToReload);

        // Add Mod Reloader section
        AddOption(headerModName, tooltip: () => Loc.Get("MainMenu.WelcomeTooltip"), isHeader: true);
        AddOption(Loc.Get("MainMenu.OpenConfigText"), () => Conf.C.Open(), () => Loc.Get("MainMenu.OpenConfigTooltip"));
        AddOption(Loc.Get("MainMenu.ReloadText"), async () => await ReloadUtilities.SinglePlayerReload(), () => Loc.Get("MainMenu.ReloadTooltip", $"[c/FFFF00:{reloadHoverMods}] "));
        AddOption(string.Empty);

        // Add options section
        AddOption(Loc.Get("MainMenu.OptionsHeader"), () => Loc.Get("MainMenu.OptionsTooltip"), isHeader: true);
        AddOption(Loc.Get("MainMenu.StartServerText"), MainMenuActions.StartServer, () => Loc.Get("MainMenu.StartServerTooltip"));
        AddOption(Loc.Get("MainMenu.StartClientText"), MainMenuActions.StartClient, () => Loc.Get("MainMenu.StartClientTooltip"));
        AddOption(Loc.Get("MainMenu.OpenLogText"), Log.OpenClientLog, () => Loc.Get("MainMenu.OpenLogTooltip", $"[c/FFFF00:{Path.GetFileName(Logging.LogPath)}]"));
        AddOption(Loc.Get("MainMenu.ClearLogText"), Log.ClearClientLog, () => Loc.Get("MainMenu.ClearLogTooltip", $"[c/FFFF00:{Path.GetFileName(Logging.LogPath)}]"));
        AddOption(string.Empty);

        // Add singleplayer section
        // Load players and worlds for tooltips
        Main.LoadPlayers();
        int playerIdx = Conf.C.Player;
        if (playerIdx < 0 || playerIdx >= Main.PlayerList.Count) playerIdx = 0;
        string playerName = Main.PlayerList.Count > 0 ? Main.PlayerList[playerIdx].Name : "";

        Main.LoadWorlds();
        int worldIdx = Conf.C.World;
        if (worldIdx < 0 || worldIdx >= Main.WorldList.Count) worldIdx = 0;
        string worldName = Main.WorldList.Count > 0 ? Main.WorldList[worldIdx].Name : "";

        // Add singleplayer section
        AddOption(Loc.Get("MainMenu.SingleplayerHeader"), tooltip: () => Loc.Get("MainMenu.SingleplayerTooltip"), isHeader: true);
        AddOption(
            Loc.Get("MainMenu.JoinSingleplayerText"),
            () =>
            {
                ClientDataJsonHelper.ClientMode = ClientMode.SinglePlayer;
                ClientDataJsonHelper.PlayerPath = null;
                ClientDataJsonHelper.WorldPath = null;
                AutoloadPlayerInWorldSystem.EnterSingleplayerWorld();
            },
            () =>
            {
                if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(worldName))
                    return Loc.Get("MainMenu.JoinSingleplayerTooltipNoData");
                return Loc.Get("MainMenu.JoinSingleplayerTooltip", $"[c/FFFF00:{playerName}]", $"[c/FFFF00:{worldName}]");
            }
        );
        AddOption(string.Empty);

        // Add multiplayer section
        AddOption(Loc.Get("MainMenu.MultiplayerHeader"), tooltip: () => Loc.Get("MainMenu.MultiplayerTooltip"), isHeader: true);
        AddOption(Loc.Get("MainMenu.HostMultiplayerText"), AutoloadPlayerInWorldSystem.HostMultiplayerWorld, () => Loc.Get("MainMenu.HostMultiplayerTooltip"));
        AddOption(Loc.Get("MainMenu.JoinMultiplayerText"), AutoloadPlayerInWorldSystem.EnterMultiplayerWorld, () => Loc.Get("MainMenu.JoinMultiplayerTooltip"));
        AddOption(string.Empty);

        // Add world section
        AddOption(Loc.Get("MainMenu.WorldHeader"), tooltip: () => Loc.Get("MainMenu.WorldTooltip"), isHeader: true);
        AddOption(
            Loc.Get("MainMenu.CreateNewWorld"),
            () => MainMenuActions.CreateNewWorld(MainMenuActions.GetNextAvailableTestWorldName()),
            () => Loc.Get(
            "MainMenu.CreateNewWorldTooltip",
            $"[c/FFFF00:{MainMenuActions.GetNextAvailableTestWorldName()}]",
            $"[c/FFFF00:{Conf.C.CreateTestWorldSize}]",
            $"[c/FFFF00:{Conf.C.CreateTestWorldDifficulty}]"
            )
        );

        // Add tooltip panel
        tooltipPanel = new();
        tooltipPanel.BorderColor = new Color(33, 43, 79) * 0.8f;
        tooltipPanel.BackgroundColor = new Color(73, 94, 171);
        tooltipPanel.Left.Set(15 - 3, 0f); // -3 panel padding on each side is 6/2 = 3
        tooltipPanel.Top.Set(currentY + 10, 0f);
        tooltipPanel.Width.Set(310f, 0f);
        tooltipPanel.Height.Set(68f, 0f);

        // Add tooltip text
        tooltipText = new UIText(string.Empty, 0.9f);
        tooltipText.Left.Set(0f, 0f);
        tooltipText.TextOriginX = 0;
        tooltipText.TextOriginY = 0;
        tooltipPanel.Append(tooltipText);
        Append(tooltipPanel);
    }
    #endregion

    /// <summary>
    /// Helper method to add a menu option.
    /// </summary>
    private void AddOption(string text,
                            Action action = null,
                            Func<string> tooltip = null,
                            bool isHeader = false)
    {
        float size = isHeader ? 1.15f : 1f;
        var label = new UIText(text, size)
        {
            TextColor = isHeader ? Hover : Normal
        };

        label.Left.Set(15f, 0f);
        label.Top.Set(currentY, 0f);
        currentY += verticalSpacing;

        label.OnMouseOver += (_, _) =>
        {
            if (!isHeader)
                label.TextColor = Hover;
            if (tooltip != null)
            {
                tooltipText.SetText(tooltip());
                tooltipPanel.Hidden = false;
            }
        };
        label.OnMouseOut += (_, _) =>
        {
            if (!isHeader)
                label.TextColor = Normal;
            tooltipText.SetText(string.Empty);
            tooltipPanel.Hidden = true;
        };

        if (action != null)
            label.OnLeftClick += (_, _) => action.Invoke();

        Append(label);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
    }
}
