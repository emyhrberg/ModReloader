using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Systems.Hooks;
using Terraria.GameContent;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;

namespace ModReloader.Common.Configs.ConfigElements;

public class PlayerIndexSliderElement : IntPathOptionElement
{
    protected override int GetCount()
    {
        if (Main.PlayerList == null) return 0;
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
        string name = player.name;

        // Measure width
        var font = FontAssets.ItemStack.Value;
        var width = font.MeasureString(name).X;
        var namePos = new Vector2(rect.X + dims.Width - 200 - width, rect.Y + 7);

        ChatManager.DrawColorCodedStringWithShadow(sb, font,
            name, namePos, Color.White, 0f, Vector2.Zero, Vector2.One);

        // Player head pos
        Vector2 headPos = new(namePos.X - 20, namePos.Y + 5);

        // Draw player head
        PlayerHeadFlipHook.shouldFlipHeadDraw = player.direction == -1;
        Main.MapPlayerRenderer.DrawPlayerHead(
            Main.Camera,
            player,
            headPos,
            scale: 0.8f,
            borderColor: Color.White
        );
        PlayerHeadFlipHook.shouldFlipHeadDraw = false;


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