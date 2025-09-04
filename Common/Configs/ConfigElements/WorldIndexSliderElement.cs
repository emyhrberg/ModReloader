using Microsoft.Xna.Framework.Graphics;

namespace ModReloader.Common.Configs.ConfigElements;

public class WorldIndexSliderElement : IntOptionElement
{
    protected override int GetCount()
    {
        if (Main.WorldList == null) return 0;
        return Main.WorldList.Count;
    }

    protected override string ResolveName(int index)
    {
        if (Main.WorldList == null || Main.WorldList.Count == 0) return "No worlds";
        int clamped = index;
        if (clamped < 0) clamped = 0;
        if (clamped > Main.WorldList.Count - 1) clamped = Main.WorldList.Count - 1;
        var data = Main.WorldList[clamped];
        if (data != null && !string.IsNullOrWhiteSpace(data.Name)) return data.Name;
        return "World " + clamped;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}