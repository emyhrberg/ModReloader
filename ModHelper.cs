using System.IO;
using System.Linq;
using ModHelper.Common.Configs;
using ModHelper.PacketHandlers;

namespace ModHelper
{
    // If no Autoload(Side) is provided, it will default to Both (which is wanted in this case)
    // [Autoload(Side = ModSide.Client)]
    // [Autoload(Side = ModSide.Both)]
    public class ModHelper : Mod
    {
        public static ModHelper Instance { get; private set; }

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
    }
}