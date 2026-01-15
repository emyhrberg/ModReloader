using System;

namespace ModReloader.Core.Features.Reload
{
    /// <summary>
    /// Stores client data in AppDomain memory to persist across mod reloads.
    /// Uses separate keys for each value to avoid Assembly reload issues.
    /// </summary>
    public static class ClientDataMemoryStorage
    {
        private const string ClientModeKey = "ModReloader_ClientMode";
        private const string PlayerPathKey = "ModReloader_PlayerPath";
        private const string WorldPathKey = "ModReloader_WorldPath";

        public static ClientMode ClientMode
        {
            get
            {
                var value = AppDomain.CurrentDomain.GetData(ClientModeKey);
                if (value is int intValue)
                    return (ClientMode)intValue;
                return ClientMode.FreshClient;
            }
            set => AppDomain.CurrentDomain.SetData(ClientModeKey, (int)value);
        }

        public static string PlayerPath
        {
            get => AppDomain.CurrentDomain.GetData(PlayerPathKey) as string;
            set => AppDomain.CurrentDomain.SetData(PlayerPathKey, value);
        }

        public static string WorldPath
        {
            get => AppDomain.CurrentDomain.GetData(WorldPathKey) as string;
            set => AppDomain.CurrentDomain.SetData(WorldPathKey, value);
        }

        /// <summary>
        /// Writes the current client data to memory.
        /// </summary>
        public static void WriteData()
        {
            if (Main.dedServ)
                return;

            Log.Info($"Wrote ClientData to memory: C {ClientMode}, P {PlayerPath}, W {WorldPath}");
        }

        /// <summary>
        /// Reads the client data from memory.
        /// </summary>
        public static void ReadData()
        {
            if (Main.dedServ)
                return;

            Log.Info($"Read ClientData from memory: C {ClientMode}, P {PlayerPath}, W {WorldPath}");
        }

        /// <summary>
        /// Clears all stored client data from memory.
        /// </summary>
        public static void ClearData()
        {
            AppDomain.CurrentDomain.SetData(ClientModeKey, null);
            AppDomain.CurrentDomain.SetData(PlayerPathKey, null);
            AppDomain.CurrentDomain.SetData(WorldPathKey, null);
        }
    }

    public enum ClientMode
    {
        FreshClient,
        SinglePlayer,
        MPMajor,
        MPMinor,
    }
}
