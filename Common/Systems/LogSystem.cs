// // filepath: c:\Users\erikm\Documents\My Games\Terraria\tModLoader\ModSources\SquidTestingMod\Common\Systems\LogSystem.cs
// using System;
// using System.Reflection;
// using log4net;
// using log4net.Appender;
// using log4net.Core; // For Level
// using log4net.Repository.Hierarchy;
// using SquidTestingMod.Helpers;
// using Terraria.ModLoader;

// namespace SquidTestingMod.Common.Systems
// {
//     public class LogSystem : ModSystem
//     {
//         public override void Load()
//         {
//             try
//             {
//                 // Reflect the internal tML logger
//                 PropertyInfo tmlProp = typeof(Logging).GetProperty("tML", BindingFlags.Static | BindingFlags.NonPublic);
//                 if (tmlProp == null)
//                     return;

//                 ILog tmlLogger = (ILog)tmlProp.GetValue(null);
//                 if (tmlLogger == null)
//                     return;

//                 if (tmlLogger.Logger is not Logger loggerImpl)
//                     return;

//                 // Remove all appenders and set logger level to OFF
//                 Log.Info("Disabling tML logger");
//                 loggerImpl.RemoveAllAppenders();
//                 loggerImpl.Level = Level.Info;
//                 loggerImpl.Repository.Threshold = Level.Info;
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine("Could not disable tML logger: " + ex.Message);
//             }
//         }
//     }
// }