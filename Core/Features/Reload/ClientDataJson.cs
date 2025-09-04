using System.Text.Json.Serialization;

namespace ModReloader.Core.Features.Reload
{
    public class ClientDataJson
    {
        public int ProcessID { get; set; }

        // This is needed to serialize enums as strings
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ClientMode ClientMode { get; set; }
        public string PlayerPath { get; set; }
        public string WorldPath { get; set; }
    }

    public enum ClientMode
    {
        FreshClient,
        SinglePlayer,
        MPMajor,
        MPMinor,
    }
}
