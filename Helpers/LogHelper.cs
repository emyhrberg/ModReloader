// using System;
// using System.Linq;
// using System.Reflection;
// using log4net;
// using log4net.Appender;
// using log4net.Core;
// using log4net.Layout;
// using log4net.Repository.Hierarchy;

// namespace EliteTestingMod.Helpers
// {
//     public static class LogHelper
//     {
//         public static void ChangeLogDateFormat(string newDatePattern)
//         {
//             try
//             {
//                 // Get the repository
//                 Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

//                 // Find existing file appender to get its properties
//                 FileAppender oldAppender = hierarchy.GetAppenders()
//                     .OfType<FileAppender>()
//                     .FirstOrDefault(a => a.Name == "FileAppender");

//                 if (oldAppender == null)
//                 {
//                     Log.Warn("FileAppender not found");
//                     return;
//                 }

//                 // Create a new pattern layout
//                 PatternLayout layout = new PatternLayout();
//                 layout.ConversionPattern = $"[%date{{{newDatePattern}}}] [%thread/%level] [%logger]: %message%newline";
//                 layout.ActivateOptions();

//                 // Create a new file appender with the same settings
//                 FileAppender newAppender = new FileAppender();
//                 newAppender.Name = "NewFileAppender";
//                 newAppender.File = oldAppender.File;
//                 newAppender.AppendToFile = oldAppender.AppendToFile;
//                 newAppender.Encoding = oldAppender.Encoding;
//                 newAppender.Layout = layout;
//                 newAppender.ActivateOptions();

//                 // Remove the old appender from all loggers
//                 foreach (Logger logger in hierarchy.GetCurrentLoggers().Cast<Logger>())
//                 {
//                     logger.RemoveAppender(oldAppender);
//                     logger.AddAppender(newAppender);
//                 }

//                 // Update the root logger too
//                 Logger rootLogger = hierarchy.Root;
//                 rootLogger.RemoveAppender(oldAppender);
//                 rootLogger.AddAppender(newAppender);

//                 // Close the old appender
//                 oldAppender.Close();

//                 Log.Info($"Successfully changed log date format to: {newDatePattern}");
//             }
//             catch (Exception ex)
//             {
//                 Log.Error($"Error changing log date format: {ex.Message}");
//             }
//         }

//         // ...existing code...

//         public static void ResetLogPattern()
//         {
//             try
//             {
//                 // Default tModLoader pattern
//                 const string DEFAULT_PATTERN = "[%d{HH:mm:ss.fff}] [%t/%level] [%logger]: %m%n";

//                 // Get the repository
//                 Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

//                 // Find all file appenders
//                 var appenders = hierarchy.GetAppenders()
//                     .OfType<FileAppender>()
//                     .ToList();

//                 if (appenders.Count == 0)
//                 {
//                     Log.Warn("No FileAppenders found");
//                     return;
//                 }

//                 foreach (var appender in appenders)
//                 {
//                     // Create a new layout with the default pattern
//                     PatternLayout defaultLayout = new PatternLayout();
//                     defaultLayout.ConversionPattern = DEFAULT_PATTERN;
//                     defaultLayout.ActivateOptions();

//                     // Close the appender first to release file locks
//                     appender.Close();

//                     // Set the new layout
//                     appender.Layout = defaultLayout;

//                     // Reactivate the appender
//                     appender.ActivateOptions();
//                 }

//                 Log.Info("Log pattern reset to default");
//             }
//             catch (Exception ex)
//             {
//                 Log.Error($"Error resetting log pattern: {ex.Message}");
//             }
//         }

//         public static void ClearLogFile()
//         {
//             try
//             {
//                 // Get the repository
//                 var hierarchy = LogManager.GetRepository() as Hierarchy;
//                 if (hierarchy == null) return;

//                 // Temporarily disable logging by raising the level threshold
//                 Level oldThreshold = hierarchy.Threshold;
//                 hierarchy.Threshold = Level.Off;

//                 // Find the file appender
//                 var appender = hierarchy.GetAppenders()
//                     .OfType<FileAppender>()
//                     .FirstOrDefault();

//                 if (appender == null) return;

//                 // Get the file path
//                 string filePath = appender.File;

//                 // Use reflection to access private Writer field and close it
//                 var type = typeof(TextWriterAppender);
//                 var writerField = type.GetField("m_writer", BindingFlags.NonPublic | BindingFlags.Instance);

//                 if (writerField != null)
//                 {
//                     var writer = writerField.GetValue(appender);
//                     if (writer != null)
//                     {
//                         type.GetMethod("CloseWriter", BindingFlags.NonPublic | BindingFlags.Instance)
//                             ?.Invoke(appender, null);
//                     }
//                 }

//                 // Clear the file
//                 System.IO.File.WriteAllText(filePath, string.Empty);

//                 // Restore logging
//                 hierarchy.Threshold = oldThreshold;

//                 Log.Info("Log file cleared successfully");
//             }
//             catch (Exception ex)
//             {
//                 Log.Error($"Failed to clear log file: {ex.Message}");
//             }
//         }
//     }
// }