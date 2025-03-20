using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod.CustomReload
{
    internal static class TMLData
    {
        const BindingFlags finstance = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        public const BindingFlags fstatic = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
        public static ObjectBackup MenusListBackup;
        public static PropertyBackup CloudsCountBackup;
        public static ObjectBackup CloudsDictBackup;
        public static ObjectBackup EventBackup;

        public static void SaveTMLData()
        {
            Assembly tmlAssembly = typeof(ModLoader).Assembly;

            //Type MenuLoader_Type = typeof(MenuLoader);

            Type TypeCashing_type = tmlAssembly.GetType("Terraria.ModLoader.Core.TypeCaching");
            MenusListBackup = new ObjectBackup(typeof(MenuLoader), "menus", fstatic);
            CloudsCountBackup = new PropertyBackup(typeof(CloudLoader), "CloudCount", fstatic);
            CloudsDictBackup = new ObjectBackup(typeof(CloudLoader), "clouds", fstatic);
            EventBackup = new ObjectBackup(TypeCashing_type, "OnClear", fstatic);
        } 

        public static void Restore()
        {
            //---UnloadModContent()---
            //MenuLoader.Unload();
            MenusListBackup.Restore();

            //CloudLoader.Unload();
            CloudsCountBackup.Restore();
            CloudsDictBackup.Restore();
            //Cloud.SwapOutModdedClouds()
            var ModCloudProperty = typeof(Cloud).GetProperty("ModCloud");
            if (!Main.dedServ)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.cloud[i].type >= CloudID.Count)
                    {
                        Main.cloud[i].type = Main.rand.Next(0, 22);
                        ModCloudProperty.SetValue(Main.cloud[i], null);
                    }
                }
            }

            //Unloading mods
            /*
            int i = 0;
            foreach (var mod in ModLoader.Mods.Reverse())
            {
                Interface.loadMods.SetCurrentMod(i++, mod);

                try
                {
                    MonoModHooks.RemoveAll(mod);
                    mod.Close();
                    mod.UnloadContent();
                }
                catch (Exception e)
                {
                    e.Data["mod"] = mod.Name;
                    throw;
                }
            }*/


            //
            GetDiferenceOfOldAndNewEvent(EventBackup).DynamicInvoke();
            EventBackup.Restore();
        }
        
        public static Delegate GetDiferenceOfOldAndNewEvent(ObjectBackup eventBackup)
        {
            return Delegate.Remove((Delegate)eventBackup.ClonedValue, (Delegate)eventBackup.FieldValue);
        }
    }
}
