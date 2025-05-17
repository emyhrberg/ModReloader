using System.IO;
using DragonLens.Core.Systems.ToolbarSystem;
using Terraria.ModLoader.IO;

namespace ModReloader.Common.Commands
{
    [JITWhenModsEnabled("DragonLens")]
    public class DLSaveCustomLayoutCommand : ModCommand
    {
        public override string Command => "save";

        public override string Description => "Save the current DL Layout (for dev only).";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (!ModLoader.TryGetMod("DragonLens", out _))
            {
                Main.NewText("DragonLens must be enabled.");
                return;
            }

            if (args.Length < 1 || string.IsNullOrWhiteSpace(args[0]))
            {
                Main.NewText("Please provide a layout name. Usage: /save <layoutName>");
                return;
            }

            string firstArg = args[0];
            string layoutsDir = Path.Join(Main.SavePath, "DragonLensLayouts");
            string layoutPath = Path.Join(layoutsDir, firstArg);

            // Ensure directory exists
            Directory.CreateDirectory(layoutsDir);

            // Create new tag and save layout
            TagCompound tag = [];
            ToolbarHandler.SaveLayout(tag);

            // Write to file
            TagIO.ToFile(tag, layoutPath);

            Main.NewText("Successfully saved layout to " + layoutPath);
        }
    }
}