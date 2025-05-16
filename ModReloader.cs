using System;
using System.IO;
using System.Linq;
using ModReloader.Common.Configs;
using ModReloader.PacketHandlers;
using Terraria.ModLoader;

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
    }
}
