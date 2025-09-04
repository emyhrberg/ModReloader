using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Configs.ConfigElements.ModSources;
using ModReloader.Common.Systems.Hooks;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ModReloader.Common.Configs.ConfigElements;

public class WorldIndexSliderElement : IntPathOptionElement
{
    private WorldPreviewElement worldPreviewElement;

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

    public override void OnBind()
    {
        base.OnBind();

        worldPreviewElement = new();
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        var raw = MemberInfo.GetValue(Item);
        int worldIndex = 0;
        if (raw is string path)
        {
            worldIndex = PathToID(path);
        }
        
        if (worldIndex < 0 || Main.WorldList == null || worldIndex >= Main.WorldList.Count)
            return;

        var world = Main.WorldList[worldIndex];
        string name = world.GetWorldName();

        // Measure width for positioning text
        var font = FontAssets.ItemStack.Value;
        var width = font.MeasureString(name).X;

        var dims = GetDimensions();
        var rect = dims.ToRectangle();

        // Position of the world name
        var namePos = new Vector2(rect.X + dims.Width - 200 - width, rect.Y + 7);
        ChatManager.DrawColorCodedStringWithShadow(sb, font, name, namePos, Color.White, 0f, Vector2.Zero, Vector2.One);

        // Position for preview (to the left of the name)
        Vector2 previewPos = new(namePos.X  - 36, rect.Y + 2);

        // Update the preview element with this world’s options
        byte difficulty = (byte)world.GameMode;  // 0 normal, 1 expert, 2 master, 3 journey
        byte evil;
        if (world.HasCorruption) evil = 1;
        else if (world.HasCrimson) evil = 2;
        else evil = 0; // random / unknown

        byte size = 0;
        if (world.WorldSizeX >= 8400) size = 2; // large
        else if (world.WorldSizeX >= 6400) size = 1; // medium
        else size = 0; // small

        worldPreviewElement.UpdateOption(difficulty, evil, size);

        // Layout and draw at 30×30
        worldPreviewElement.Left.Set(previewPos.X, 0f);
        worldPreviewElement.Top.Set(previewPos.Y, 0f);
        worldPreviewElement.Width.Set(30, 0f);
        worldPreviewElement.Height.Set(30, 0f);

        worldPreviewElement.Recalculate();
        worldPreviewElement.Draw(sb);
    }

    protected override string IDToPath(int index)
    {
        return Utilities.FindWorld(index).Path;
    }

    protected override int PathToID(string path)
    {
        return Utilities.FindWorldId(path);
    }
}