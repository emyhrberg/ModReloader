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
    public class DLSaveCustomLayoutCommand : ModCommand
    {
        public override string Command => "save";

        public override string Description => "Save the current DL Layout (for dev only).";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length != 1)
            {
                // Print usage
                Main.NewText("Usage: /save");
                return;
            }

            // get first arg
            string firstArg = args[0];

            // save it
            string layoutPath = Path.Join(Main.SavePath, "DragonLensLayouts", firstArg);
            TagCompound tag = TagIO.FromFile(layoutPath);
            ToolbarHandler.SaveLayout(tag);
            Main.NewText("Successfully saved layout to " + layoutPath);
        }
    }
}