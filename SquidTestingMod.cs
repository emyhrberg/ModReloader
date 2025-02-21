using SquidTestingMod.Helpers;
using SquidTestingMod.PacketHandlers;
using System;
using System.IO;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod
{
    [Autoload(Side = ModSide.Client)]
    public class SquidTestingMod : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModNetHandler.HandlePacket(reader, whoAmI);
        }

        public override void Load()
        {
            ClientDataHandler.ReadData();
        }

        public override void Unload()
        {
            ClientDataHandler.WriteData();
        }

    }
}
