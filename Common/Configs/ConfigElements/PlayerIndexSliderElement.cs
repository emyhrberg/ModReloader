using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ModReloader.Common.Configs.ConfigElements;

public class PlayerIndexSliderElement : IntOptionElement
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

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        // Player instance
        int playerIndex = 0;
        object raw = MemberInfo.GetValue(Item);
        if (raw is int i)
            playerIndex = i;

        if (playerIndex < 0)
            return;

        if (playerIndex != -1)
        {
            // Dims
            var dims = GetDimensions();
            var rect = dims.ToRectangle();
            var pos = new Vector2(rect.X+dims.Width-200, rect.Y+12);

            // Player
            var player = Main.PlayerList[playerIndex].Player;

            // Draw player head
            Main.MapPlayerRenderer.DrawPlayerHead(
                Main.Camera,
                player,
                pos,
                scale: 0.8f,
                borderColor: Color.White
            );

        }
    }
}