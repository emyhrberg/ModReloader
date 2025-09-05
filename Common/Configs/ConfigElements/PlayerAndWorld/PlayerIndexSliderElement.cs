using Microsoft.Xna.Framework.Graphics;
using ModReloader.Core.Features.Reload;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ModReloader.Common.Configs.ConfigElements.PlayerAndWorld;

public class PlayerIndexSliderElement : IntPathOptionElement
{
    protected override int GetCount()
    {
        if (Main.PlayerList == null)
        {
            return 0;
        }
        //Log.Info($"Found {Main.PlayerList.Count} players");
        return Main.PlayerList.Count;
    }

    protected override string ResolveName(int index)
    {
        if (Main.PlayerList == null || Main.PlayerList.Count == 0) return "No players";
        int clamped = index;
        if (clamped < 0) clamped = 0;
        if (clamped > Main.PlayerList.Count - 1) clamped = Main.PlayerList.Count - 1;
        var file = Main.PlayerList[clamped];
        if (file != null && file.Player != null && !string.IsNullOrWhiteSpace(file.Player.name))
            return file.Player.name;
        if (file != null && !string.IsNullOrWhiteSpace(file.Name)) return file.Name;
        return "Player " + clamped;
    }

    protected override string ResolveDifficulty(int index)
    {
        if (Main.PlayerList == null || Main.PlayerList.Count == 0)
            return "No players";

        int clamped = index;
        if (clamped < 0) clamped = 0;
        if (clamped > Main.PlayerList.Count - 1) clamped = Main.PlayerList.Count - 1;

        var file = Main.PlayerList[clamped];
        if (file == null)
            return "Unknown";

        // map difficulty byte -> string
        string difficultyText = file.Player?.difficulty switch
        {
            0 => "Classic",
            1 => "Mediumcore",
            2 => "Hardcore",
            3 => "Journey",
            _ => "Unknown"
        };

        return difficultyText;
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        // Player instance
        int playerIndex = -1;
        object raw = MemberInfo.GetValue(Item);
        if (raw is string path)
        {
            playerIndex = PathToID(path);
        }

        if (playerIndex < 0)
            playerIndex = 0;


        // Dims
        var dims = GetDimensions();
        var rect = dims.ToRectangle();
        var pos = new Vector2(rect.X + dims.Width - 200, rect.Y + 12);

        // Player
        var player = Main.PlayerList[playerIndex].Player;
        //string name = player.name;
        //var font = FontAssets.ItemStack.Value;
        //var width = font.MeasureString(name).X;
        //var namePos = new Vector2(rect.X + dims.Width - 200 - width, rect.Y + 7);
        //ChatManager.DrawColorCodedStringWithShadow(sb, font,
            //name, namePos, Color.White, 0f, Vector2.Zero, Vector2.One);

        // Player head pos
        Vector2 headPos = new(rect.X +rect.Width-210, rect.Y + 13);

        // Draw player head
        Main.MapPlayerRenderer.DrawPlayerHead(
            Main.Camera,
            player,
            headPos,
            scale: 0.8f,
            borderColor: Color.White
        );
    }

    protected override string IDToPath(int index)
    {
        return Utilities.FindPlayer(index).Path;
    }

    protected override int PathToID(string path)
    {
        return Utilities.FindPlayerId(path);
    }
}