using SquidTestingMod.Helpers;
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
            bool killServer = reader.ReadBoolean();
            if (Main.netMode == NetmodeID.Server)
            {
                if (killServer)
                {
                    Log.Info("Attempting to close the server");
                    Netplay.Disconnect = true;

                }
            }
        }
    }
}
