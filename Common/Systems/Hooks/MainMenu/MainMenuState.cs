﻿using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.UI.Elements.MainMenuElements;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.Common.Systems.Hooks.MainMenu;

internal sealed class MainMenuState : UIState
{
    // Elements
    private UIList mainMenuList;
    private TooltipPanel tooltipPanel;

    public MainMenuState()
    {
        // Null checks
        if (Conf.C is null || !Conf.C.ShowMainMenuInfo)
            return;

        // Set up UIList
        mainMenuList = new UIList
        {
            Width = { Pixels = 310f },
            Height = StyleDimension.Fill,
            ListPadding = 0f,
            Left = { Pixels = 15f },
            Top = { Pixels = 15f },
            ManualSortMethod = (e) => { }
        };

        // Extra spacing if other big menu mods are loaded
        if (ModLoader.HasMod("TerrariaOverhaul") || ModLoader.HasMod("Terramon"))
        {
            mainMenuList.Top.Pixels += 205f;
        }
        if (ModLoader.HasMod("CompatChecker"))
            mainMenuList.Top.Pixels += 30f;

        tooltipPanel = new TooltipPanel();

        // Add elements to the main menu list
        AddModReloaderSection(tooltipPanel);
        AddOptionsSection(tooltipPanel);
        AddSingleplayerSection(tooltipPanel);
        AddMultiplayerSection(tooltipPanel);
        AddWorldSection(tooltipPanel);

        Append(tooltipPanel);
        Append(mainMenuList);
    }

    private void AddModReloaderSection(TooltipPanel tooltipPanel)
    {
        // Helpers
        string headerModName = $"{ModContent.GetInstance<ModReloader>().DisplayName} v{ModContent.GetInstance<ModReloader>().Version}";
        string reloadHoverMods = ReloadUtilities.IsModsToReloadEmpty ? "No mods selected" : string.Join(",", Conf.C.ModsToReload);

        var headerElement = new HeaderMainMenuElement(headerModName, () => Loc.Get("MainMenu.WelcomeTooltip"), tooltipPanel);
        var configElement = new ActionMainMenuElement(
            () => Conf.C.Open(),
            Loc.Get("MainMenu.OpenConfigText"),
            () => Loc.Get("MainMenu.OpenConfigTooltip"),
            tooltipPanel
        );
        var reloadElement = new ActionMainMenuElement(
            async () => await ReloadUtilities.SinglePlayerReload(),
            Loc.Get("MainMenu.ReloadText"),
            () => Loc.Get("MainMenu.ReloadTooltip", $"[c/FFFF00:{reloadHoverMods}]"),
            tooltipPanel
        );
        var spacer = new SpacerMainMenuElement();
        mainMenuList.Add(headerElement);
        mainMenuList.Add(configElement);
        mainMenuList.Add(reloadElement);
        mainMenuList.Add(spacer);
    }

    private void AddOptionsSection(TooltipPanel tooltipPanel)
    {
        var optionsHeader = new HeaderMainMenuElement(Loc.Get("MainMenu.OptionsHeader"), () => Loc.Get("MainMenu.OptionsTooltip"), tooltipPanel);
        var startServerElement = new ActionMainMenuElement(
            MainMenuActions.StartServer,
            Loc.Get("MainMenu.StartServerText"),
            () => Loc.Get("MainMenu.StartServerTooltip"),
            tooltipPanel
        );
        var startClientElement = new ActionMainMenuElement(
            MainMenuActions.StartClient,
            Loc.Get("MainMenu.StartClientText"),
            () => Loc.Get("MainMenu.StartClientTooltip"),
            tooltipPanel
        );
        var openLogElement = new ActionMainMenuElement(
            Log.OpenClientLog,
            Loc.Get("MainMenu.OpenLogText"),
            () => Loc.Get("MainMenu.OpenLogTooltip", $"[c/FFFF00:{Path.GetFileName(Logging.LogPath)}]"),
            tooltipPanel
        );
        var clearLogElement = new ActionMainMenuElement(
            Log.ClearClientLog,
            Loc.Get("MainMenu.ClearLogText"),
            () => Loc.Get("MainMenu.ClearLogTooltip", $"[c/FFFF00:{Path.GetFileName(Logging.LogPath)}]"),
            tooltipPanel
        );
        var spacer = new SpacerMainMenuElement();
        mainMenuList.Add(optionsHeader);
        mainMenuList.Add(startServerElement);
        mainMenuList.Add(startClientElement);
        mainMenuList.Add(openLogElement);
        mainMenuList.Add(clearLogElement);
        mainMenuList.Add(spacer);
    }

    private void AddSingleplayerSection(TooltipPanel tooltipPanel)
    {
        // Load players and worlds for tooltips
        Main.LoadPlayers();
        int playerIdx = Conf.C.Player;
        if (playerIdx < 0 || playerIdx >= Main.PlayerList.Count) playerIdx = 0;
        string playerName = Main.PlayerList.Count > 0 ? Main.PlayerList[playerIdx].Name : "";

        Main.LoadWorlds();
        int worldIdx = Conf.C.World;
        if (worldIdx < 0 || worldIdx >= Main.WorldList.Count) worldIdx = 0;
        string worldName = Main.WorldList.Count > 0 ? Main.WorldList[worldIdx].Name : "";
        Log.Info("Loaded and found this many worlds in main menu: " + Main.WorldList.Count);

        var singleplayerHeader = new HeaderMainMenuElement(Loc.Get("MainMenu.SingleplayerHeader"), () => Loc.Get("MainMenu.SingleplayerTooltip"), tooltipPanel);
        var joinSingleplayer = new ActionMainMenuElement(
            () =>
            {
                ClientDataJsonHelper.ClientMode = ClientMode.SinglePlayer;
                ClientDataJsonHelper.PlayerPath = null;
                ClientDataJsonHelper.WorldPath = null;
                AutoloadPlayerInWorldSystem.EnterSingleplayerWorld();
            },
            Loc.Get("MainMenu.JoinSingleplayerText"),
            () =>
            {
                if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(worldName))
                    return Loc.Get("MainMenu.JoinSingleplayerTooltipNoData");
                return Loc.Get("MainMenu.JoinSingleplayerTooltip", $"[c/FFFF00:{playerName}]", $"[c/FFFF00:{worldName}]");
            },
            tooltipPanel
        );
        var spacer = new SpacerMainMenuElement();
        mainMenuList.Add(singleplayerHeader);
        mainMenuList.Add(joinSingleplayer);
        mainMenuList.Add(spacer);
    }

    private void AddMultiplayerSection(TooltipPanel tooltipPanel)
    {
        var multiplayerHeader = new HeaderMainMenuElement(Loc.Get("MainMenu.MultiplayerHeader"), () => Loc.Get("MainMenu.MultiplayerTooltip"), tooltipPanel);
        var hostMultiplayer = new ActionMainMenuElement(
            AutoloadPlayerInWorldSystem.HostMultiplayerWorld,
            Loc.Get("MainMenu.HostMultiplayerText"),
            () => Loc.Get("MainMenu.HostMultiplayerTooltip"),
            tooltipPanel
        );
        var joinMultiplayer = new ActionMainMenuElement(
            AutoloadPlayerInWorldSystem.EnterMultiplayerWorld,
            Loc.Get("MainMenu.JoinMultiplayerText"),
            () => Loc.Get("MainMenu.JoinMultiplayerTooltip"),
            tooltipPanel
        );
        var spacer = new SpacerMainMenuElement();
        mainMenuList.Add(multiplayerHeader);
        mainMenuList.Add(hostMultiplayer);
        mainMenuList.Add(joinMultiplayer);
        mainMenuList.Add(spacer);
    }

    private void AddWorldSection(TooltipPanel tooltipPanel)
    {
        var worldHeader = new HeaderMainMenuElement(Loc.Get("MainMenu.WorldHeader"), () => Loc.Get("MainMenu.WorldTooltip"), tooltipPanel);
        var createNewWorld = new ActionMainMenuElement(
            () => MainMenuActions.CreateNewWorld(MainMenuActions.GetNextAvailableTestWorldName()),
            Loc.Get("MainMenu.CreateNewWorld"),
            () => Loc.Get("MainMenu.CreateNewWorldTooltip",
            $"[c/FFFF00:{MainMenuActions.GetNextAvailableTestWorldName()}]",
            $"[c/FFFF00:{Conf.C.CreateTestWorldSize}]",
            $"[c/FFFF00:{Conf.C.CreateTestWorldDifficulty}]"
            ),
            tooltipPanel
        );
        var spacer = new SpacerMainMenuElement();
        mainMenuList.Add(worldHeader);
        mainMenuList.Add(createNewWorld);
        mainMenuList.Add(spacer);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        //DrawActionRowsDebug(spriteBatch);
    }

    private void PositionTooltip()
    {
        tooltipPanel.Left.Set(15 + 3, 0f);            
        tooltipPanel.Top.Set(470f, 0f);  // 6 px under the list
        tooltipPanel.Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        PositionTooltip();
    }

    private void DrawActionRowsDebug(SpriteBatch spriteBatch)
    {
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Color debugColor = Color.Red * 0.3f;

        // the first child is the inner list
        foreach (var inner in mainMenuList.Children)
        {
            if (inner is not UIElement) continue;

            foreach (var child in inner.Children)
            {
                if (child is ActionMainMenuElement)
                {
                    CalculatedStyle d = child.GetOuterDimensions();
                    Rectangle rect = new(
                        (int)d.X,
                        (int)d.Y,
                        (int)d.Width,
                        (int)d.Height);

                    spriteBatch.Draw(pixel, rect, debugColor);
                }
            }
        }
    }


}
