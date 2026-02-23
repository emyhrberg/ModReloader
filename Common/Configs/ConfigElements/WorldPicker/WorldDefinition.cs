using ModReloader.Core.Features;
using Newtonsoft.Json;
using System.IO;
using Terraria.IO;
using Terraria.ModLoader.Config;


namespace ModReloader.Common.Configs.ConfigElements.WorldPicker;

public class WorldDefinition : EntityDefinition
{
    /// <summary>
    /// The index of the world in Main.WorldList. -1 if not found.
    /// </summary>
    public override int Type => Utilities.FindWorldId(Name);

    [JsonIgnore]
    public WorldFileData File => IsUnloaded ? null : Main.WorldList[Type];

    public override bool IsUnloaded
        => Type <= -1;

    public WorldDefinition() : base()
    {

    }

    public WorldDefinition(int type) : base(Utilities.FindWorld(type)?.Path ?? "None")
    {

    }

    public WorldDefinition(string path) : base()
    {
        Name = path;
    }
}

