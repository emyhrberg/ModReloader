using Microsoft.Xna.Framework.Graphics;
using ModReloader.Core.Features.MainMenuFeatures.UI;
using ModReloader.Core.Features.ModToggler.UI;
using ModReloader.Core.Features.Reload;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace ModReloader.Core.Features.MainMenuFeatures;

internal sealed class MainMenuState : UIState
{
    // Rebuild if screen width changes
    private int previousScreenWidth = -1;

    private const float MainMenuListWidth = 310f;
    private const float MainMenuModElementHeight = 30f;

    private static HashSet<string> enabledModNamesAtLoad;
    private static HashSet<string> currentEnabledModNames;

    private UIText modsHeaderText;

    // Elements
    private UIList leftMainMenuList;
    private UIList rightMainMenuList;
    private TooltipPanel leftTooltipPanel;
    private TooltipPanel rightTooltipPanel;

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
            Width = { Pixels = 210f },
            Height = StyleDimension.Fill,
            ListPadding = 0f,
            Left = { Pixels = 15f },
            Top = { Pixels = GetLeftMenuTop() },
            ManualSortMethod = (e) => { }
        };

        leftTooltipPanel = new TooltipPanel();
        leftTooltipPanel.Left.Set(18f, 0f);
        leftTooltipPanel.Top.Set(GetLeftTooltipTop(), 0f);

        rightMainMenuList = new UIList
        {
            Width = { Pixels = 310 },
            Height = StyleDimension.Fill,
            ListPadding = 0f,
            Left = { Pixels = -310 - 15f, Percent = 1f },
            Top = { Pixels = GetRightMenuTop() },
            ManualSortMethod = (e) => { }
        };

        rightTooltipPanel = new TooltipPanel();
        rightTooltipPanel.Left.Set(-310 - 15f, 1f);
        rightTooltipPanel.Top.Set(GetRightMenuTop(), 0f);

        Append(leftTooltipPanel);
        Append(leftMainMenuList);
        Append(rightMainMenuList);
        Append(rightTooltipPanel);
    }

    private void Rebuild()
    {
        if (leftMainMenuList == null || leftTooltipPanel == null)
            return;

        leftMainMenuList.Clear();
        rightMainMenuList.Clear();
        rightMainMenuList.Top.Set(GetRightMenuTop(), 0f);
        rightTooltipPanel.Hidden = true;

        AddModReloaderSection(leftTooltipPanel);
        AddStartSection(leftTooltipPanel);
        AddLogsSection(leftTooltipPanel);
        AddSingleplayerSection(leftTooltipPanel);
        AddMultiplayerSection(leftTooltipPanel);

        if (Conf.C.ShowQuickWorldGenSection)
            AddWorldSection(leftTooltipPanel);

        leftTooltipPanel.Top.Set(GetLeftTooltipTop(), 0f);

        if (Conf.C.ShowModsSection)
            AddModsSection(leftTooltipPanel);

        leftMainMenuList.Recalculate();
        rightMainMenuList.Recalculate();
        leftTooltipPanel.Recalculate();
        rightTooltipPanel.Recalculate();
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
        float top = Conf.C.ShowQuickWorldGenSection ? 530f : 458f;

        if (ModLoader.HasMod("TerrariaOverhaul") || ModLoader.HasMod("Terramon"))
            top += 205f;

        if (ModLoader.HasMod("CompatChecker"))
            top += 30f;

        return top;
    }

    private static int GetRightSideMaxMods()
    {
        return IsReeseShownInMainMenu() ? 10 : 21;
    }

    private static float GetRightMenuTop()
    {
        return IsReeseShownInMainMenu() ? 465f : 5f;
    }

    private static bool IsReeseShownInMainMenu()
    {
        try
        {
            Mod reese = ModLoader.Mods.FirstOrDefault(mod => mod.Name.Equals("Reese", StringComparison.OrdinalIgnoreCase));
            if (reese == null)
                return false;

            FieldInfo configsField = typeof(ConfigManager).GetField("Configs", BindingFlags.Static | BindingFlags.NonPublic);
            if (configsField?.GetValue(null) is not IDictionary<Mod, List<ModConfig>> configs)
                return false;

            if (!configs.TryGetValue(reese, out List<ModConfig> modConfigs))
                return false;

            ModConfig clientConfig = modConfigs.FirstOrDefault(config => config.GetType().FullName == "Reese.Core.Configs.ClientConfig");
            if (clientConfig == null)
                return false;

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo showField = clientConfig.GetType().GetField("ShowInMainMenu", flags);
            if (showField?.GetValue(clientConfig) is bool fieldValue)
                return fieldValue;

            PropertyInfo showProperty = clientConfig.GetType().GetProperty("ShowInMainMenu", flags);
            return showProperty?.GetValue(clientConfig) is bool propertyValue && propertyValue;
        }
        catch
        {
            return false;
        }
    }

    private void AddModReloaderSection(TooltipPanel tooltipPanel)
    {
        string headerModName = $"{ModContent.GetInstance<ModReloader>().DisplayName} v{ModContent.GetInstance<ModReloader>().Version}";
        var headerElement = new HeaderMainMenuElement(headerModName, () => Loc.Get("MainMenu.WelcomeTooltip"), tooltipPanel);
        leftMainMenuList.Add(headerElement);
        var spacer = new SpacerMainMenuElement();

        var configElement = new ActionMainMenuElement(
            () => Conf.C.Open(),
            Loc.Get("MainMenu.OpenConfigText"),
            () => Loc.Get("MainMenu.OpenConfigTooltip"),
            tooltipPanel
        );

        leftMainMenuList.Add(configElement);
        leftMainMenuList.Add(CreateBuildReloadElement(tooltipPanel));
        leftMainMenuList.Add(spacer);
    }

    private void AddStartSection(TooltipPanel tooltipPanel)
    {
        var startHeader = new HeaderMainMenuElement(Loc.Get("MainMenu.StartHeader"), () => Loc.Get("MainMenu.StartTooltip"), tooltipPanel);
        leftMainMenuList.Add(startHeader);

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

        var startServerElement = new ActionMainMenuElement(
            MainMenuActions.StartServer,
            Loc.Get("MainMenu.StartServerText"),
            () =>
            {
                string args = MainMenuActions.GetStartServerArgumentsText();
                string argsText = string.IsNullOrEmpty(args) ? "None" : args;

                return Loc.Get("MainMenu.StartServerTooltip",
                    $"[c/FFFF00:{worldName}]") + $"\nArgs: [c/FFFF00:{argsText}]";
            },
            tooltipPanel
        );
        var startClientElement = new ActionMainMenuElement(
            MainMenuActions.StartClient,
            Loc.Get("MainMenu.StartClientText"),
            () => Loc.Get("MainMenu.StartClientTooltip"),
            tooltipPanel
        );

        leftMainMenuList.Add(startServerElement);
        leftMainMenuList.Add(startClientElement);
        var spacer = new SpacerMainMenuElement();
        leftMainMenuList.Add(spacer);
    }

    private void AddLogsSection(TooltipPanel tooltipPanel)
    {
        var logsHeader = new HeaderMainMenuElement(Loc.Get("MainMenu.LogsHeader"), () => Loc.Get("MainMenu.LogsTooltip"), tooltipPanel);
        leftMainMenuList.Add(logsHeader);
        
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
        leftMainMenuList.Add(openLogElement);
        leftMainMenuList.Add(openServerLogElement);
        leftMainMenuList.Add(clearLogElement);
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
            AutoloadPlayerInWorldSystem.EnterSingleplayerWorldFromConfig,
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
            AutoloadPlayerInWorldSystem.EnterMultiplayerWorldFromConfig,
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
        int maxVisibleMods = GetRightSideMaxMods();
        int loadedModCount = ModLoader.Mods.Count(mod => mod.Name != "ModLoader");
        TooltipPanel rightTooltip = IsReeseShownInMainMenu() || loadedModCount > maxVisibleMods + 5 ? null : rightTooltipPanel;

        var modsHeader = new HeaderMainMenuElement(Loc.Get("MainMenu.ModsHeader"), () => Loc.Get("MainMenu.ModsTooltip"), rightTooltip);
        var reloadOnlyElement = CreateReloadOnlyElement(rightTooltip);
        var openEnabledJsonElement = new ActionMainMenuElement(
            MainMenuActions.OpenEnabledJson,
            Loc.Get("MainMenu.OpenEnabledText"),
            () => Loc.Get("MainMenu.OpenEnabledTooltip"),
            rightTooltip
        );

        var spacer = new SpacerMainMenuElement(height:4);
        rightMainMenuList.Add(modsHeader);
        rightMainMenuList.Add(reloadOnlyElement);
        rightMainMenuList.Add(openEnabledJsonElement);
        rightMainMenuList.Add(spacer);
        rightMainMenuList.Add(CreateModsHeaderPanel());
        rightMainMenuList.Add(spacer);

        List<Mod> modsToShow = ModLoader.Mods
            .Where(mod => mod.Name != "ModLoader")
            .OrderBy(mod => mod.DisplayName)
            .Take(maxVisibleMods)
            .ToList();
        Dictionary<string, string> modDescriptions = GetModDescriptionsByInternalName();

        foreach (Mod mod in modsToShow)
        {
            Texture2D modIcon = ModsPanel.GetModIconFromAllMods(mod.File);
            modDescriptions.TryGetValue(mod.Name, out string modDescription);

            rightMainMenuList.Add(new MainMenuModElement(
                cleanModName: mod.DisplayName,
                internalModName: mod.Name,
                icon: modIcon,
                version: mod.Version.ToString(),
                enabled: currentEnabledModNames.Contains(mod.Name),
                stateChanged: OnModElementStateChanged,
                modDescription: modDescription ?? string.Empty
            ));
        }

        PositionRightTooltip(modsToShow.Count);
    }

    private static ActionMainMenuElement CreateBuildReloadElement(TooltipPanel tooltipPanel)
    {
        return new ActionMainMenuElement(
            action: async () => await ReloadUtilities.SinglePlayerReload(),
            text: Loc.Get("MainMenu.ReloadText"),
            tooltip: GetBuildReloadTooltip,
            tooltipPanel: tooltipPanel
        );
    }

    private static ActionMainMenuElement CreateReloadOnlyElement(TooltipPanel tooltipPanel)
    {
        return new ActionMainMenuElement(
            action: async () => await ReloadWithoutBuilding(),
            text: Loc.Get("MainMenu.ReloadText"),
            tooltip: () => Loc.Get("MainMenu.ReloadOnlyTooltip"),
            tooltipPanel: tooltipPanel
        );
    }

    private static string GetBuildReloadTooltip()
    {
        return ReloadUtilities.IsModsToReloadEmpty
            ? Loc.Get("MainMenu.ReloadNoMods")
            : Loc.Get("MainMenu.ReloadTooltip", $"[c/FFFF00:{string.Join(",", Conf.C.ModsToReload)}]");
    }

    private static async Task ReloadWithoutBuilding()
    {
        ReloadUtilities.forceJustReload = true;

        try
        {
            await ReloadUtilities.SinglePlayerReload();
        }
        finally
        {
            ReloadUtilities.forceJustReload = false;
        }
    }

    private void PositionRightTooltip(int visibleModCount)
    {
        float rightListTop = GetRightMenuTop();
        float headerAndOptionsHeight = 25f + 25f + 25f + 20f + 34f;
        rightTooltipPanel.Top.Set(rightListTop + headerAndOptionsHeight + visibleModCount * MainMenuModElementHeight + 8f, 0f);
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

    private UIPanel CreateModsHeaderPanel()
    {
        var panel = new UIPanel
        {
            Width = { Pixels = -0f, Percent = 1f },
            Height = { Pixels = 34f },
            Left = { Pixels = 0f }
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

        int maxVisibleMods = GetRightSideMaxMods();
        string countText = currentEnabledModNames.Count > maxVisibleMods
            ? $"{maxVisibleMods}+"
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

    private static Dictionary<string, LocalMod> GetLocalModsByInternalName()
    {
        return ModOrganizer.FindAllMods()
            .GroupBy(localMod => localMod.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.First(),
                StringComparer.OrdinalIgnoreCase
            );
    }

    private static Dictionary<string, string> GetModDescriptionsByInternalName()
    {
        return GetLocalModsByInternalName()
            .ToDictionary(
                pair => pair.Key,
                pair => pair.Value.properties.description ?? string.Empty,
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
