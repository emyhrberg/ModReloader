using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Core.Features.MainMenuFeatures.UI;
using ModReloader.Core.Features.Reload;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.Core.Features.MainMenuFeatures;

internal sealed class MainMenuState : UIState
{
    // Elements
    private UIList mainMenuList;
    private TooltipPanel tooltipPanel;

    public MainMenuState()
    {
        Log.Info("Created menu state");

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

        tooltipPanel = new TooltipPanel();
        tooltipPanel.Left.Set(15 + 3, 0f);
        tooltipPanel.Top.Set(490f, 0f);  // 6 px under the list

        // Extra spacing if other big menu mods are loaded
        if (ModLoader.HasMod("TerrariaOverhaul") || ModLoader.HasMod("Terramon"))
        {
            mainMenuList.Top.Pixels += 205f;
        }
        if (ModLoader.HasMod("CompatChecker"))
        {
            mainMenuList.Top.Pixels += 30f;
            tooltipPanel.Top.Pixels += 30f;
        }

        // Add elements to the main menu list
        AddModReloaderSection(tooltipPanel);
        AddOptionsSection(tooltipPanel);
        AddSingleplayerSection(tooltipPanel);
        AddMultiplayerSection(tooltipPanel);
        AddWorldSection(tooltipPanel);

        Append(tooltipPanel);
        Append(mainMenuList);
    }

    public override void OnActivate()
    {
        base.OnActivate();

        if (mainMenuList == null || tooltipPanel == null)
            return;

        Rebuild();
    }

    private void Rebuild()
    {
        mainMenuList.Clear();

        AddModReloaderSection(tooltipPanel);
        AddOptionsSection(tooltipPanel);
        AddSingleplayerSection(tooltipPanel);
        AddMultiplayerSection(tooltipPanel);
        AddWorldSection(tooltipPanel);

        mainMenuList.Recalculate();
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
        Func<string> reloadTooltip;

        if (ReloadUtilities.IsModsToReloadEmpty)
            reloadTooltip = () => Loc.Get("MainMenu.ReloadNoMods"); // e.g. "No mods selected"
        else
            reloadTooltip = () => Loc.Get("MainMenu.ReloadTooltip", $"[c/FFFF00:{reloadHoverMods}]");

        var reloadElement = new ActionMainMenuElement(
            action: async () => await ReloadUtilities.SinglePlayerReload(),
            text: Loc.Get("MainMenu.ReloadText"),
            tooltip: reloadTooltip,
            tooltipPanel: tooltipPanel
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
        var openServerLogElement = new ActionMainMenuElement(
            Log.OpenServerLog,
            Loc.Get("MainMenu.OpenServerLogText"),
            () => Loc.Get("MainMenu.OpenLogTooltip", $"[c/FFFF00:server.log]"),
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
        mainMenuList.Add(openServerLogElement);
        mainMenuList.Add(clearLogElement);
        mainMenuList.Add(spacer);
    }

    private int playerIndex = -1;
    private int worldIndex = -1;

    public void UpdatePlayerIndex(int index) => playerIndex = index;
    public void UpdateWorldIndex(int index) => worldIndex = index;

    private void AddSingleplayerSection(TooltipPanel tooltipPanel)
    {
        Main.LoadPlayers();
        int playerIdx = Conf.C.Player != null ? Utilities.FindPlayerId(Conf.C.Player.Name) : 0;
        if (playerIdx < 0 || playerIdx >= Main.PlayerList.Count)
            playerIdx = 0;

        string playerName = Main.PlayerList.Count > 0 ? Main.PlayerList[playerIdx].Name : "";

        Main.LoadWorlds();
        int worldIdx = Conf.C.World != null ? Utilities.FindWorldId(Conf.C.World.Name) : 0;
        if (worldIdx < 0 || worldIdx >= Main.WorldList.Count)
            worldIdx = 0;

        string worldName = Main.WorldList.Count > 0 ? Main.WorldList[worldIdx].Name : "";

        Log.Info("Loaded and found this many worlds in main menu: " + Main.WorldList.Count);

    var singleplayerHeader = new HeaderMainMenuElement(Loc.Get("MainMenu.SingleplayerHeader"), () => Loc.Get("MainMenu.SingleplayerTooltip"), tooltipPanel);
        var joinSingleplayer = new ActionMainMenuElement(
    () =>
    {
        ClientDataMemoryStorage.ClientMode = ClientMode.SinglePlayer;
        ClientDataMemoryStorage.PlayerPath = null;
        ClientDataMemoryStorage.WorldPath = null;
        AutoloadPlayerInWorldSystem.EnterSingleplayerWorld();
    },
    Loc.Get("MainMenu.JoinSingleplayerText"),
    () =>
    {
        Main.LoadPlayers();
        Main.LoadWorlds();

        int playerIdx = Conf.C.Player.Type;
        int worldIdx = Conf.C.World.Type;

        string pName = (playerIdx >= 0 && playerIdx < Main.PlayerList.Count)
            ? Main.PlayerList[playerIdx].Name
            : "";
        string wName = (worldIdx >= 0 && worldIdx < Main.WorldList.Count)
            ? Main.WorldList[worldIdx].Name
            : "";

        if (string.IsNullOrEmpty(pName) || string.IsNullOrEmpty(wName))
            return Loc.Get("MainMenu.JoinSingleplayerTooltipNoData");

        return Loc.Get("MainMenu.JoinSingleplayerTooltip",
            $"[c/FFFF00:{pName}]",
            $"[c/FFFF00:{wName}]");
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
        Main.LoadPlayers();
        Main.LoadWorlds();

        int playerIdx = Conf.C.Player.Type;
        int worldIdx = Conf.C.World.Type;

        string pName = (playerIdx >= 0 && playerIdx < Main.PlayerList.Count)
            ? Main.PlayerList[playerIdx].Name
            : "";
        string wName = (worldIdx >= 0 && worldIdx < Main.WorldList.Count)
            ? Main.WorldList[worldIdx].Name
            : "";

        var multiplayerHeader = new HeaderMainMenuElement(Loc.Get("MainMenu.MultiplayerHeader"), () => Loc.Get("MainMenu.MultiplayerTooltip"), tooltipPanel);
        var hostMultiplayer = new ActionMainMenuElement(
            AutoloadPlayerInWorldSystem.HostMultiplayerWorld,
            Loc.Get("MainMenu.HostMultiplayerText"),
            () => Loc.Get("MainMenu.HostMultiplayerTooltip", 
            $"[c/FFFF00:{pName}]",
            $"[c/FFFF00:{wName}]"),
            tooltipPanel
        );
        var joinMultiplayer = new ActionMainMenuElement(
            AutoloadPlayerInWorldSystem.EnterMultiplayerWorld,
            Loc.Get("MainMenu.JoinMultiplayerText"),
            () => Loc.Get("MainMenu.JoinMultiplayerTooltip",
            $"[c/FFFF00:{pName}]",
            $"[c/FFFF00:{wName}]"),
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

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        // PositionTooltip();
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
