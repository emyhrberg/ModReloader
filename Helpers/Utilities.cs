using System;
using System.IO;
using System.Text;
using System.Threading;
using Terraria;

namespace ModHelper.Helpers
{
    //Class basically for universal helping functions
    internal static class Utilities
    {
        public static int ProcessID => System.Environment.ProcessId;

        public static int FindPlayerId()
        {
            Main.LoadPlayers();
            var playerId = Main.PlayerList.FindIndex(p => p.Path == Main.ActivePlayerFileData.Path);
            return playerId;
        }

        public static int FindWorldId()
        {
            Main.LoadWorlds();
            var worldId = Main.WorldList.FindIndex(w => w.Path == Main.ActiveWorldFileData.Path);
            return worldId;
        }

        public static void LockingFile(string filePath, Action<StreamReader, StreamWriter> action)
        {
            int retryDelay = 200; // 200ms delay between retries
            int maxAttempts = 20; // Maximum retries before giving up
            int attempts = 0;

            while (true)
            {
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    using (StreamReader reader = new StreamReader(fs, new UTF8Encoding(false)))
                    using (StreamWriter writer = new StreamWriter(fs, new UTF8Encoding(false)))
                    {
                        // Log.Info($"File {Path.GetFileName(filePath)} is locked by {Utilities.ProcessID} process. Editing...");
                        fs.Seek(0, SeekOrigin.Begin);
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);

                        action?.Invoke(reader, writer);

                        // Log.Info($"File {Path.GetFileName(filePath)} editing complete by {Utilities.ProcessID} process");
                        break;
                    }
                }
                catch (IOException)
                {
                    attempts++;
                    if (attempts >= maxAttempts)
                    {
                        Log.Info("Timeout: Unable to access file.");
                        break;
                    }

                    Log.Info($"File is in use. Retrying {attempts} time...");
                    Thread.Sleep(retryDelay); // Wait before retrying
                }
            }
        }

        public static string GetModHelperFolderPath(string fileName)
        {
            return GetFolderPath("ModHelper", fileName);
        }

        public static string GetFolderPath(string folderName, string fileName)
        {
            try
            {
                string folderPath = Path.Combine(Main.SavePath, folderName); //ModHelper
                Directory.CreateDirectory(folderPath); // Ensure the directory exists
                string filePath = Path.Combine(folderPath, fileName); //
                Log.Info("Found save path: " + filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                Log.Error("Could not find part of the path: " + ex.Message);
                return null;
            }
        }
    }
}
