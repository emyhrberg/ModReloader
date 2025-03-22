// using System;
// using System.Reflection;
// using log4net;
// using log4net.Appender;
// using log4net.Core; // For Level
// using log4net.Filter;
// using log4net.Layout;
// using log4net.Repository.Hierarchy;
// using Terraria.ModLoader;

// namespace SquidTestingMod.UI
// {
//     public class CustomLogModSystem : ModSystem
//     {
//         public override void Load()
//         {
//             try
//             {
//                 // 1. Reflect the internal tML logger
//                 PropertyInfo tmlProp = typeof(Logging).GetProperty("tML", BindingFlags.Static | BindingFlags.NonPublic)
//                     ?? throw new Exception("Could not find Logging.tML property");
//                 ILog tmlLogger = (ILog)tmlProp.GetValue(null)
//                     ?? throw new Exception("tML logger instance is null");

//                 // 2. Access its underlying logger
//                 Logger loggerImpl = tmlLogger.Logger as Logger
//                     ?? throw new Exception("Could not cast to log4net.Repository.Hierarchy.Logger");

//                 // 3. Find any FileAppenders
//                 foreach (var appender in loggerImpl.Appenders)
//                 {
//                     // Close the file to release the lock.
//                     var closeFileMethod = typeof(FileAppender).GetMethod("CloseFile", BindingFlags.NonPublic | BindingFlags.Instance);
//                     closeFileMethod?.Invoke(appender, null);

//                     if (appender is FileAppender fileAppender &&
//                         fileAppender.Layout is PatternLayout patternLayout)
//                     {
//                         // Add a custom prefix to each log line
//                         patternLayout.ConversionPattern = "[CustomLog] " + patternLayout.ConversionPattern;
//                         patternLayout.ActivateOptions();

//                         // Only keep debug messages
//                         fileAppender.ClearFilters(); // remove default threshold filter if present

//                         // This filter allows only messages at level = Debug:
//                         var rangeFilter = new LevelRangeFilter
//                         {
//                             LevelMin = Level.Debug,
//                             LevelMax = Level.Debug,
//                             AcceptOnMatch = true
//                         };
//                         fileAppender.AddFilter(rangeFilter);

//                         // Deny all other levels
//                         fileAppender.AddFilter(new DenyAllFilter());

//                         // Re-activate
//                         fileAppender.ActivateOptions();
//                     }
//                 }

//                 tmlLogger.Info("Custom log layout applied via reflection.");
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine("Error applying custom log hook: " + ex);
//             }
//         }
//     }
// }