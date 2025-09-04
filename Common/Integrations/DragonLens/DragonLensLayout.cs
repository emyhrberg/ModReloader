﻿using System;
using System.IO;
using System.Reflection;
using DragonLens.Content.GUI;
using DragonLens.Content.Themes.BoxProviders;
using DragonLens.Content.Themes.IconProviders;
using DragonLens.Content.Tools;
using DragonLens.Content.Tools.Despawners;
using DragonLens.Content.Tools.Editors;
using DragonLens.Content.Tools.Gameplay;
using DragonLens.Content.Tools.Spawners;
using DragonLens.Content.Tools.Visualization;
using DragonLens.Core.Systems.ThemeSystem;
using DragonLens.Core.Systems.ToolbarSystem;
using Terraria.ModLoader.UI.Elements;

namespace ModReloader.Common.Integrations.DragonLens
{
    // References:
    // Adding a layout to the layout browser with grid.Add():
    // https://github.com/ScalarVector1/DragonLens/blob/master/Content/GUI/LayoutPresetBrowser.cs#L25
    // Creating a layout file with ToolbarHandler.ExportToFile():
    // https://github.com/ScalarVector1/DragonLens/blob/master/Content/GUI/FirstTimeLayoutPresetMenu.cs
    // Creating custom layouts using toolbars and tools:
    // https://github.com/ScalarVector1/DragonLens/blob/master/Core/Systems/FirstTimeSetupSystem.cs

    /// Adds a new layout to DragonLens
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensLayout : ModSystem
    {
        private delegate void orig_PopulateGrid(LayoutPresetBrowser self, UIGrid grid);

        public override void PostSetupContent()
        {
            OnPopulateGridHook();
        }

        private void OnPopulateGridHook()
        {
            MethodInfo populateGridMethod = typeof(LayoutPresetBrowser).GetMethod("PopulateGrid");
            if (populateGridMethod == null)
            {
                Log.Error("PopulateGrid not found in class LayoutPresetBrowser");
                return;
            }
            MonoModHooks.Add(populateGridMethod, OnPopulateGrid);
        }

        // This method adds the actual layouts to layout browser
        private void OnPopulateGrid(orig_PopulateGrid orig, LayoutPresetBrowser self, UIGrid grid)
        {
            orig(self, grid);

            // Setup layout names
            string cheatsheetLayout = "Cheatsheet + Mod Reloader";
            string herosLayout = "HEROs Mod + Mod Reloader";

            // Register the layouts
            RegisterHerosLayout(herosLayout);
            RegisterCheatSheetLayout(cheatsheetLayout);

            // Add the layouts to the grid of layout browser
            grid.Add(new LayoutPresetButton(self, cheatsheetLayout, GetLayoutPath(cheatsheetLayout), cheatsheetLayout));
            grid.Add(new LayoutPresetButton(self, herosLayout, GetLayoutPath(herosLayout), herosLayout));
        }

        private static string RegisterHerosLayout(string layoutName)
        {
            ToolbarHandler.BuildPreset(layoutName, n =>
            {
                // bottom heros toolbar
                n.Add(
                    new Toolbar(new Vector2(0.5f, 1f), Orientation.Horizontal, AutomaticHideOption.Never)
                    .AddTool<ItemSpawner>()
                    .AddTool<InfiniteReach>()
                    .AddTool<SpawnTool>()
                    .AddTool<ItemDespawner>()
                    .AddTool<Time>()
                    .AddTool<Weather>()
                    .AddTool<NPCSpawner>()
                    .AddTool<BuffSpawner>()
                    .AddTool<Godmode>()
                    .AddTool<ItemEditor>()
                    .AddTool<CustomizeTool>()
                    );

                // left modreloader toolbar
                n.Add(
                    new Toolbar(new Vector2(0f, 0.5f), Orientation.Vertical, AutomaticHideOption.Never)
                    .AddTool<DragonLensLogPanel>()
                    .AddTool<DragonLensUIPanel>()
                    .AddTool<DragonLensModsPanel>()
                    .AddTool<DragonLensReload>()
                    .AddTool<DragonLensReloadMP>()
                    );
            },
            ThemeHandler.GetBoxProvider<VanillaBoxes>(),
            ThemeHandler.GetIconProvider<HEROsIcons>());

            ExportLayout(layoutName);

            return layoutName;
        }

        private static string RegisterCheatSheetLayout(string layoutName)
        {
            ToolbarHandler.BuildPreset(layoutName, n =>
            {
                // bottom cheatsheet toolbar
                n.Add(
                new Toolbar(new Vector2(0.5f, 1f), Orientation.Horizontal, AutomaticHideOption.Never)
                .AddTool<ItemSpawner>()
                .AddTool<NPCSpawner>()
                .AddTool<ItemDespawner>()
                .AddTool<Paint>()
                .AddTool<AccessoryTray>()
                .AddTool<PlayerEditorTool>()
                .AddTool<Magnet>()
                .AddTool<NPCDespawner>()
                .AddTool<SpawnTool>()
                .AddTool<Floodlight>()
                .AddTool<CustomizeTool>()
                );

                // left modreloader toolbar
                n.Add(
                    new Toolbar(new Vector2(0f, 0.5f), Orientation.Vertical, AutomaticHideOption.Never)
                    .AddTool<DragonLensLogPanel>()
                    .AddTool<DragonLensUIPanel>()
                    .AddTool<DragonLensModsPanel>()
                    .AddTool<DragonLensReload>()
                    .AddTool<DragonLensReloadMP>()
                    );
            },
            ThemeHandler.GetBoxProvider<SimpleBoxes>(),
            ThemeHandler.GetIconProvider<DefaultIcons>());

            ExportLayout(layoutName);

            return layoutName;
        }

        // Helper to get folder path
        private static string GetLayoutPath(string layoutName)
        {
            return Path.Join(Main.SavePath, "DragonLensLayouts", layoutName);
        }

        // Helper to export a file to DragonLens layout folder path
        private static void ExportLayout(string layoutName)
        {
            try
            {
                string layoutPath = Path.Combine(Main.SavePath, "DragonLensLayouts", layoutName);
                Directory.CreateDirectory(Path.GetDirectoryName(layoutPath));
                ToolbarHandler.ExportToFile(layoutPath);
                Log.Info($"Successfully exported layout: {layoutName}");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to export layout {layoutName}: {ex.Message}");
            }
        }
    }
}