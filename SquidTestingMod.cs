using SquidTestingMod.Helpers;
using SquidTestingMod.PacketHandlers;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod
{
    public class SquidTestingMod : Mod
    {
        public override void Load()
        {
            base.Load();
        }

        //TODO: Steal PacketHandler from https://github.com/tModLoader/tModLoader/wiki/intermediate-netcode#good-practice-managing-many-packets
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModNetHandler.HandlePacket(reader, whoAmI);
        }
    }
}
