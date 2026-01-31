using ModReloader.Core.Features;
using System.IO;
using Terraria.ModLoader.Config;


namespace ModReloader.Common.Configs.ConfigElements.WorldPicker;

public class WorldDefinition : EntityDefinition
{
    /// <summary>
    /// The index of the world in Main.WorldList. -1 if not found.
    /// </summary>
    public override int Type => Utilities.FindWorldId(Name);

    public override bool IsUnloaded
        => Type <= -1 || Name == null;

    public WorldDefinition() : base()
    {

    }

    public WorldDefinition(int type) : base(Utilities.FindWorld(type).Path)
    {

    }

    public WorldDefinition(string path) : base()
    {
        Name = path;
    }
}

