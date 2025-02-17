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
    public class SquidTestingMod : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModNetHandler.HandlePacket(reader, whoAmI);
        }

    }
}
