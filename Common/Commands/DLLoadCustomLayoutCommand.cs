using System.IO;
using System.Linq;
using DragonLens.Content.Tools.Despawners;
using DragonLens.Content.Tools.Gameplay;
using DragonLens.Content.Tools.Map;
using DragonLens.Content.Tools.Multiplayer;
using DragonLens.Content.Tools.Spawners;
using DragonLens.Content.Tools.Visualization;
using DragonLens.Content.Tools;
using DragonLens.Core.Systems.ThemeSystem;
using DragonLens.Core.Systems.ToolbarSystem;
using DragonLens.Content.Themes.BoxProviders;
using DragonLens.Content.Themes.IconProviders;
using ModReloader.Common.Systems.Integrations;
using DragonLens.Content.GUI;
using Stubble.Core.Classes;
using Terraria.ModLoader.IO;

namespace ModReloader.Common.Commands
{
    [JITWhenModsEnabled("DragonLens")]
    public class DLLoadCustomLayoutCommand : ModCommand
    {
        public override string Command => "load";

        public override string Description => "Load a custom DL Layout (for dev only).";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (!ModLoader.TryGetMod("DragonLens", out _))
            {
                Main.NewText("DragonLens must be enabled.");
                return;
            }

            // If there is not exactly 1 arg, print usage
            if (args.Length != 1)
            {
                // Print usage
                Main.NewText("Usage: /load YourLayoutName");
                return;
            }

            string firstArg = args[0].ToLower();

            // check if the first arg exists in the path
            string layoutsFilePath = Path.Join(Main.SavePath, "DragonLensLayouts", firstArg);

            if (!File.Exists(layoutsFilePath))
            {
                // Print error
                Main.NewText($"Error: {layoutsFilePath} does not exist.", Color.Red);
                return;
            }

            string path = Path.Join(Main.SavePath, "DragonLensLayouts", firstArg);

            TagCompound tag = TagIO.FromFile(path);
            ToolbarHandler.LoadLayout(tag);
        }
    }
}