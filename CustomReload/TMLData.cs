using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod.CustomReload
{
    internal static class TMLData
    {
        const BindingFlags finstance = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        public const BindingFlags fstatic = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
        public static ObjectBackup MenuBackup;
        public static ObjectBackup EventBackup;

        public static void SaveTMLData()
        {
            Assembly tmlAssembly = typeof(ModLoader).Assembly;

            //Type MenuLoader_Type = typeof(MenuLoader);

            Type TypeCashing_type = tmlAssembly.GetType("Terraria.ModLoader.Core.TypeCaching");
            MenuBackup = new ObjectBackup(typeof(MenuLoader), "menus", fstatic);
            EventBackup = new ObjectBackup(TypeCashing_type, "OnClear", fstatic);
        } 
    }
}
