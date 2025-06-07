using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers.API;
using ModReloader.PacketHandlers;
using ReLogic.Content;

namespace ModReloader
{
    public class ModReloader : Mod
    {
        public static ModReloader Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
            Conf.C.ModsToReload = Conf.C.ModsToReload
                .Distinct()
                .Select(modName => modName.Trim())
                .Where(modName => !string.IsNullOrEmpty(modName))
                .ToList();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModNetHandler.HandlePacket(reader, whoAmI);
        }

        public override object Call(params object[] args)
        {
            if (args.Length < 3 || args[0] is not string command)
            {
                Log.Error($"Invalid arguments for ModReloader.Call. Please supply the following arguments: command, name, action, [asset], [tooltip]. For example: ModReloader.Call('AddButton', 'Example Button', ExampleAsset.Value, 'Example Tooltip').");
                return false;
            }

            if (command.Equals("AddButton", StringComparison.CurrentCultureIgnoreCase))
            {
                string name = args[1]?.ToString();
                Action action = args[2] as Action;
                Asset<Texture2D> asset = args.Length > 3 ? args[3] as Asset<Texture2D> : null;
                string tooltip = args.Length > 4 ? args[4]?.ToString() : null;

                if (string.IsNullOrEmpty(name) || action == null)
                {
                    Log.Error("Invalid arguments for AddButton. Please provide a valid name and action.");
                    return false;
                }
                Log.Info($"Adding button '{name}' with asset '{asset?.Name}' and tooltip '{tooltip}'.");
                return ModReloaderAPI.AddButton(name, action, asset, tooltip);
            }

            return false;
        }
    }
}
