using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Terraria.IO;
using Terraria.ModLoader.Core;

namespace ModReloader.Helpers
{
    //Class basically for universal helping functions
    internal static class Utilities
    {
        public static int ProcessID => Environment.ProcessId;

        /// <summary>
        /// Finds the current player ID in the player list.
        /// </summary>
        /// <returns>The index of the current player in the player list.</returns>
        public static string FindCurrentPlayerPath()
        {
            return Main.ActivePlayerFileData.Path;
        }

        public static PlayerFileData FindPlayer(int i)
        {
            Main.LoadPlayers();
            if (i < 0 || i >= Main.PlayerList.Count)
            {
                var pfd = new PlayerFileData();
                pfd.Name = "None";
                return pfd;

            }
            return Main.PlayerList[i];
        }

        public static int FindPlayerId(string path)
        {
            Main.LoadPlayers();
            
            return Main.PlayerList.FindIndex(p => p.Path == path);
        }

        /// <summary>
        /// Finds the current world ID in the world list.
        /// </summary>
        /// <returns>The index of the current world in the world list.</returns>
        public static string FindCurrentWorldPath()
        {
            return Main.ActiveWorldFileData.Path;
        }

        /// <summary>
        /// Locks a file for reading and writing, allowing only one process to access it at a time.
        /// </summary>
        /// <param name="filePath">The path to the file to be locked.</param>
        /// <param name="action">The action to perform on the file.</param>
        public static void LockingFile(string filePath, Action<StreamReader, StreamWriter> action)
        {
            // TODO: Unhardcode this
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
                        fs.Seek(0, SeekOrigin.Begin);
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);

                        action?.Invoke(reader, writer);
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

        /// <summary>
        /// Gets the path to the file in the ModReloader folder.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The full path to the file.</returns>
        public static string GetModReloaderFolderPath(string fileName)
        {
            return GetFolderPath("ModReloader", fileName);
        }

        /// <summary>
        /// Gets the path to the file in the specified folder.<br/>
        /// If the folder does not exist, it will be created.<br/>
        /// But the file should be checked manually.
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFolderPath(string folderName, string fileName)
        {
            try
            {
                string folderPath = Path.Combine(Main.SavePath, folderName);
                Directory.CreateDirectory(folderPath); // Ensure the directory exists
                string filePath = Path.Combine(folderPath, fileName);
                //Log.Info("Found save path: " + filePath);
                return filePath;
            }
            catch (Exception ex)
            {
                Log.Error("Could not find part of the path: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets the Mod instance for a given type.
        /// Will return null if the type is not from a Mod assembly or cannot be instantiated.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static Mod GetModInstance(Type type)
        {
            var modType = AssemblyManager.GetLoadableTypes(type.Assembly)
                .FirstOrDefault(t => t.IsSubclassOf(typeof(Mod)) && !t.IsAbstract, null);
            if (modType == null)
                return null;

            // Imagine this as ModContent.GetInstance<modType>()
            var method = typeof(ModContent)
                .GetMethod(nameof(ModContent.GetInstance))
                ?.GetGenericMethodDefinition()
                ?.MakeGenericMethod(modType);
            var result = method?.Invoke(null, null);

            if (result is not Mod instance)
                throw new InvalidCastException($"{modType.FullName} is not a Mod or could not be instantiated via ModContent.GetInstance<T>()");

            return instance;
        }

        /// <summary>
        /// Gets the Mod instance for a given object.
        /// Will return null if the object is not from a Mod assembly or cannot be instantiated.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Mod GetModInstance(object obj)
        {
            return GetModInstance(obj.GetType());
        }
    }
}
