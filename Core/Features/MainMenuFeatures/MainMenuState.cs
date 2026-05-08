using Microsoft.Xna.Framework.Graphics;
using ModReloader.Core.Features.MainMenuFeatures.UI;
using ModReloader.Core.Features.ModToggler.UI;
using ModReloader.Core.Features.Reload;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace ModReloader.Core.Features.MainMenuFeatures;

internal sealed class MainMenuState : UIState
{
    // Rebuild if screen width changes
    private int previousScreenWidth = -1;

    // Mods header panel
    private const float ModsPanelGapHeight = 75f;

    private static HashSet<string> enabledModNamesAtLoad;
    private static HashSet<string> currentEnabledModNames;

    private UIText modsHeaderText;

    // Elements
    private UIList leftMainMenuList;
    private TooltipPanel leftTooltipPanel;

    public MainMenuState()
    {
        Log.Info("Created new MainMenuState");

        if (Conf.C is null || !Conf.C.ShowMainMenuInfo)
            return;

        EnsureModStateCache();
        BuildLayout();
        Rebuild();

        previousScreenWidth = Main.screenWidth;
    }

    public override void OnActivate()
    {
        Rebuild();
    }

    private void BuildLayout()
    {
        leftMainMenuList = new UIList
        {
            Width = { Pixels = 310f },
            Height = StyleDimension.Fill,
            ListPadding = 0f,
            Left = { Pixels = 15f },
            Top = { Pixels = GetLeftMenuTop() },
            ManualSortMethod = (e) => { }
        };

        leftTooltipPanel = new TooltipPanel();
        leftTooltipPanel.Left.Set(18f, 0f);
        leftTooltipPanel.Top.Set(GetLeftTooltipTop(), 0f);

        Append(leftTooltipPanel);
        Append(leftMainMenuList);
    }

    private void Rebuild()
    {
        if (leftMainMenuList == null || leftTooltipPanel == null)
            return;

        leftMainMenuList.Clear();

        AddModReloaderSection(leftTooltipPanel);
        AddOptionsSection(leftTooltipPanel);
        AddSingleplayerSection(leftTooltipPanel);
        AddMultiplayerSection(leftTooltipPanel);

        if (Conf.C.ShowQuickWorldGenSection)
            AddWorldSection(leftTooltipPanel);

        leftTooltipPanel.Top.Set(GetLeftTooltipTop(), 0f);

        if (Conf.C.ShowModsSection)
            AddModsSection(leftTooltipPanel);

        leftMainMenuList.Recalculate();
        leftTooltipPanel.Recalculate();
    }

    private static float GetLeftMenuTop()
    {
        float top = 5f;

        if (ModLoader.HasMod("TerrariaOverhaul") || ModLoader.HasMod("Terramon"))
            top += 205f;

        if (ModLoader.HasMod("CompatChecker"))
            top += 30f;

        return top;
    }

    private static float GetLeftTooltipTop()
    {
        float top = Conf.C.ShowQuickWorldGenSection ? 495f : 435f;

        if (ModLoader.HasMod("TerrariaOverhaul") || ModLoader.HasMod("Terramon"))
            top += 205f;

        if (ModLoader.HasMod("CompatChecker"))
            top += 30f;

        return top;
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
        leftMainMenuList.Add(headerElement);
        leftMainMenuList.Add(configElement);
        leftMainMenuList.Add(reloadElement);
        leftMainMenuList.Add(spacer);
    }

    private void AddOptionsSection(TooltipPanel tooltipPanel)
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

        var optionsHeader = new HeaderMainMenuElement(Loc.Get("MainMenu.OptionsHeader"), () => Loc.Get("MainMenu.OptionsTooltip"), tooltipPanel);
        var startServerElement = new ActionMainMenuElement(
            MainMenuActions.StartServer,
            Loc.Get("MainMenu.StartServerText"),
            () => Loc.Get("MainMenu.StartServerTooltip",
            $"[c/FFFF00:{worldName}]"),
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
        var openEnabledJsonElement = new ActionMainMenuElement(
            MainMenuActions.OpenEnabledJson,
            Loc.Get("MainMenu.OpenEnabledText"),
            () => Loc.Get("MainMenu.OpenEnabledTooltip"),
            tooltipPanel
        );
        var spacer = new SpacerMainMenuElement();
        leftMainMenuList.Add(optionsHeader);
        leftMainMenuList.Add(startServerElement);
        leftMainMenuList.Add(startClientElement);
        leftMainMenuList.Add(openLogElement);
        leftMainMenuList.Add(openServerLogElement);
        leftMainMenuList.Add(clearLogElement);
        leftMainMenuList.Add(openEnabledJsonElement);
        leftMainMenuList.Add(spacer);
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

        string pName = Conf.C.Player.File?.Name ?? "Undefined";
        string wName = Conf.C.World.File?.Name ?? "Undefined";

        if (string.IsNullOrEmpty(pName) || string.IsNullOrEmpty(wName))
            return Loc.Get("MainMenu.JoinSingleplayerTooltipNoData");

        return Loc.Get("MainMenu.JoinSingleplayerTooltip",
            $"[c/FFFF00:{pName}]",
            $"[c/FFFF00:{wName}]");
    },
    tooltipPanel
);

        var spacer = new SpacerMainMenuElement();
        leftMainMenuList.Add(singleplayerHeader);
        leftMainMenuList.Add(joinSingleplayer);
        leftMainMenuList.Add(spacer);
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
        leftMainMenuList.Add(multiplayerHeader);
        leftMainMenuList.Add(hostMultiplayer);
        leftMainMenuList.Add(joinMultiplayer);
        leftMainMenuList.Add(spacer);
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
        leftMainMenuList.Add(worldHeader);
        leftMainMenuList.Add(createNewWorld);
        leftMainMenuList.Add(spacer);
    }

    private void AddModsSection(TooltipPanel tooltipPanel)
    {
        leftMainMenuList.Add(new SpacerMainMenuElement(height: ModsPanelGapHeight - 10));

        leftMainMenuList.Add(CreateModsHeaderPanel(tooltipPanel));
        leftMainMenuList.Add(new SpacerMainMenuElement(height: 10));

        Dictionary<string, LocalMod> localModsByInternalName = GetLocalModsByInternalName();

        foreach (Mod mod in ModLoader.Mods
            .Where(mod => mod.Name != "ModLoader")
            .OrderBy(mod => mod.DisplayName)
            .Take(12))
        {
            Texture2D modIcon = ModsPanel.GetModIconFromAllMods(mod.File);

            string modDescription = "";

            if (localModsByInternalName.TryGetValue(mod.Name, out LocalMod localMod))
                modDescription = localMod.properties.description ?? "";

            var modElement = new ModElement(
                cleanModName: mod.DisplayName,
                internalModName: mod.Name,
                icon: modIcon,
                leftClick: null,
                modDescription: modDescription,
                version: mod.Version.ToString(),
                side: mod.Side.ToString(),
                large: false,
                enabledLayout: true,
                stateChanged: OnModElementStateChanged
            );

            modElement.SetState(
                currentEnabledModNames.Contains(mod.Name)
                    ? OptionElement.EnabledState.Enabled
                    : OptionElement.EnabledState.Disabled
            );

            leftMainMenuList.Add(modElement);
        }

        leftMainMenuList.Add(new SpacerMainMenuElement());
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        //DrawActionRowsDebug(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (previousScreenWidth != Main.screenWidth)
        {
            previousScreenWidth = Main.screenWidth;

            Rebuild();
            Recalculate();
        }
        // PositionTooltip();
    }

    #region Mod header and mod count

    private UIPanel CreateModsHeaderPanel(TooltipPanel tooltipPanel)
    {
        var panel = new UIPanel
        {
            Width = { Pixels = -35f, Percent = 1f },
            Height = { Pixels = 34f },
            Left = { Pixels = 5f }
        };

        modsHeaderText = new UIText(GetModsHeaderText(), 0.85f)
        {
            HAlign = 0.5f,
            VAlign = 0.5f
        };

        panel.Append(modsHeaderText);

        return panel;
    }
    private static void EnsureModStateCache()
    {
        if (enabledModNamesAtLoad != null && currentEnabledModNames != null)
            return;

        enabledModNamesAtLoad = GetLoadedModNameSet();
        currentEnabledModNames = new HashSet<string>(enabledModNamesAtLoad, StringComparer.OrdinalIgnoreCase);
    }

    private static HashSet<string> GetLoadedModNameSet()
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (Mod mod in ModLoader.Mods)
        {
            if (mod.Name == "ModLoader")
                continue;

            names.Add(mod.Name);
        }

        return names;
    }

    private string GetModsHeaderText()
    {
        EnsureModStateCache();

        string countText = currentEnabledModNames.Count > 12
            ? "12+"
            : currentEnabledModNames.Count.ToString();

        string reloadText = currentEnabledModNames.SetEquals(enabledModNamesAtLoad)
            ? ""
            : " (Reload Required)";

        return $"{countText} mods enabled{reloadText}";
    }

    private void UpdateModsHeader()
    {
        if (modsHeaderText == null)
            return;

        modsHeaderText.SetText(GetModsHeaderText());
    }

    private void OnModElementStateChanged(string internalModName, bool enabled)
    {
        EnsureModStateCache();

        if (enabled)
            currentEnabledModNames.Add(internalModName);
        else
            currentEnabledModNames.Remove(internalModName);

        UpdateModsHeader();
    }
    private static int GetEnabledModsCount()
    {
        int count = 0;

        foreach (Mod mod in ModLoader.Mods)
        {
            if (mod.Name == "ModLoader")
                continue;

            count++;
        }

        return count;
    }
    private static Dictionary<string, LocalMod> GetLocalModsByInternalName()
    {
        return ModOrganizer.FindWorkshopMods()
            .GroupBy(localMod => localMod.ToString(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.First(),
                StringComparer.OrdinalIgnoreCase
            );
    }
    #endregion

    private void DrawActionRowsDebug(SpriteBatch spriteBatch)
    {
        Texture2D pixel = TextureAssets.MagicPixel.Value;
        Color debugColor = Color.Red * 0.3f;

        // the first child is the inner list
        foreach (var inner in leftMainMenuList.Children)
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
