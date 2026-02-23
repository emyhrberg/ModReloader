using ModReloader.Core.Features;
using Terraria.IO;
using Terraria.ModLoader.Config;


namespace ModReloader.Common.Configs.ConfigElements.PlayerPicker;

public class PlayerDefinition : EntityDefinition
{
    /// <summary>
    /// The index of the player in Main.PlayerList. -1 if not found.
    /// </summary>
    public override int Type => Utilities.FindPlayerId(Name);

    public PlayerFileData File => IsUnloaded ? null : Main.PlayerList[Type];

    public override bool IsUnloaded
    => Type <= -1 || Name == null;


    public PlayerDefinition() : base()
    {

    }

    public PlayerDefinition(int type) : base(Utilities.FindPlayer(type)?.Path ?? "None")
    {

    }

    public PlayerDefinition(string path) : base()
    {
        Name = path;
    }
}

