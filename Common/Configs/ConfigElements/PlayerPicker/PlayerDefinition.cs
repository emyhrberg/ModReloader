using ModReloader.Core.Features.Reload;
using Terraria.ModLoader.Config;


namespace ModReloader.Common.Configs.ConfigElements.PlayerPicker;

public class PlayerDefinition : EntityDefinition
{
    /// <summary>
    /// The index of the player in Main.PlayerList. -1 if not found.
    /// </summary>
    public override int Type => Utilities.FindPlayerId(Name);

    public override bool IsUnloaded
    => Type <= -1 || Name == null;


    public PlayerDefinition() : base()
    {

    }

    public PlayerDefinition(int type) : base(Utilities.FindPlayer(type).Path)
    {

    }

    public PlayerDefinition(string path) : base()
    {
        Name = path;
    }
    public override string ToString()
    {
        return $"{(Utilities.FindPlayer(Name) != null ? Utilities.FindPlayer(Name).Name : "null")}";
    }
}

