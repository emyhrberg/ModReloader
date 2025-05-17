using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonLens.Content.GUI;
using DragonLens.Content.Tools.Despawners;
using DragonLens.Content.Tools.Editors;
using DragonLens.Content.Tools.Gameplay;
using DragonLens.Content.Tools.Spawners;
using DragonLens.Content.Tools.Visualization;
using DragonLens.Content.Tools;
using DragonLens.Core.Systems.ThemeSystem;
using DragonLens.Core.Systems.ToolbarSystem;
using DragonLens.Content.Themes.BoxProviders;
using DragonLens.Content.Themes.IconProviders;
using System.Reflection;
using Terraria.ModLoader.UI.Elements;

namespace ModReloader.Common.Systems.Integrations
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
    public class DragonLensModReloaderLayout : ModSystem
    {
        private delegate void orig_PopulateGrid(LayoutPresetBrowser self, UIGrid grid);

        public override void PostSetupContent()
        {
            string presetName = SetupLayout();
            CreateLayout(presetName);
            AddLayoutToGrid(presetName);
        }

        private void AddLayoutToGrid(string presetName)
        {
            MethodInfo populateGridMethod = typeof(LayoutPresetBrowser).GetMethod("PopulateGrid");
            if (populateGridMethod == null)
            {
                Log.Error("PopulateGrid not found in class LayoutPresetBrowser");
                return;
            }
            MonoModHooks.Add(populateGridMethod, OnPopulateGrid);
        }

        private void OnPopulateGrid(orig_PopulateGrid orig, LayoutPresetBrowser self, UIGrid grid)
        {
            orig(self, grid);

            string presetName = "Cheat Sheet + Mod Reloader";
            string presetPath = Path.Join(Main.SavePath, "DragonLensLayouts", presetName);

            grid.Add(new LayoutPresetButton(self, presetName, presetPath, "Custom layout with reload tools"));
        }

        private static void CreateLayout(string presetName)
        {
            string layoutPath = Path.Join(Main.SavePath, "DragonLensLayouts", presetName);
            ToolbarHandler.ExportToFile(layoutPath);
            Log.Info("Successfully exported layout to " + layoutPath);
        }

        private static string SetupLayout()
        {
            string presetName = "Cheatsheet + Mod Reloader";

            ToolbarHandler.BuildPreset(presetName, n =>
            {
                // bottom toolbar
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

                // left toolbar
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

            return presetName;
        }
    }
}