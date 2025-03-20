using System;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using SquidTestingMod.CustomReload;
using SquidTestingMod.Helpers;
using SquidTestingMod.PacketHandlers;
using SquidTestingMod.UI.Buttons;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod
{
    // Use both sides currently (it is default if none is set), but can be changed to client only if needed
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
            TMLData.SaveTMLData();
            foreach (var d in DetourManager.GetDetourInfo(typeof(ModLoader).GetMethod(
                        "Unload",
                        BindingFlags.NonPublic | BindingFlags.Static
                    )).Detours)
            {
                Log.Info($"{d} is Undo");
                d.Undo();

                //Doesn't work for some reason lol
            }
            HookEndpointManager.Clear();
            GC.SuppressFinalize(new Hook(typeof(ModLoader).GetMethod(
                        "Unload",
                        BindingFlags.NonPublic | BindingFlags.Static
                    ), (Func<bool> orig) =>
                    {
                        bool o = orig();
                        LogManager.GetLogger("SQUID").Info("Hi!");
                        return o;
                    }));
            GC.SuppressFinalize(new Hook(typeof(ModLoader).GetMethod(
                        "Unload",
                        BindingFlags.NonPublic | BindingFlags.Static
                    ), (Func<bool> orig) =>
                    {
                        bool o = orig();
                        LogManager.GetLogger("SQUID").Info("Hi!");
                        return o;
                    }));
        }

        public override void Unload()
        {
            ClientDataHandler.WriteData();
        }
    }
}
