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
                // first 2 args: name and action are required
                string name = args[1]?.ToString();
                Action action = args[2] as Action;

                // next 3 args: asset, tooltip, showHighlight are optional
                Asset<Texture2D> asset = args.Length > 3 ? args[3] as Asset<Texture2D> : null;
                string tooltip = args.Length > 4 ? args[4]?.ToString() : null;
                Func<bool> showHighlight = args.Length > 5 ? args[5] as Func<bool> : null;

                // Validate arguments
                if (string.IsNullOrEmpty(name))
                {
                    Log.Error("Button name cannot be null or empty.");
                    return false;
                }
                if (action == null)
                {
                    Log.Error("Button action cannot be null.");
                    return false;
                }
                if (asset == null)
                {
                    Log.Warn("Button asset is null, using default asset.");
                }
                if (string.IsNullOrEmpty(tooltip))
                {
                    Log.Warn("Button tooltip is null or empty, using button name as tooltip.");
                    tooltip = name; // Default tooltip to button name if not provided
                }
                if (showHighlight == null)
                {
                    Log.Warn("Button highlight function is null, using default (no highlight).");
                    showHighlight = () => false; // Default to no highlight
                }

                Log.Info($"Successfully adding button '{name}' with asset '{asset?.Name}' and tooltip '{tooltip}'.");
                return ModReloaderAPI.AddButton(name, action, asset, tooltip, showHighlight);
            }
            else
            {
                Log.Error($"Unknown command '{command}' for ModReloader.Call.");
            }

            return false;
        }
    }
}
