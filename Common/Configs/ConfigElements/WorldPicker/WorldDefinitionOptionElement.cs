using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace ModReloader.Common.Configs.ConfigElements.WorldPicker;

internal sealed class WorldDefinitionOptionElement : DefinitionOptionElement<WorldDefinition>
{
    private WorldFileData _file;
    private Asset<Texture2D> _iconAsset;

    public WorldDefinitionOptionElement(WorldDefinition definition, float scale = 0.75f)
        : base(definition, scale)
    {
        OverflowHidden = true;
    }

    private static readonly Color[] DifficultyColors =
    [
        Main.creativeModeColor,
        Color.White,
        Main.mcColor,
        Main.hcColor
    ];

        private static readonly Color[] SizeColors =
        {
        Color.Cyan,
        Color.Lerp(Color.Cyan, Color.LimeGreen, 0.5f),
        Color.LimeGreen
    };

    public override void SetItem(WorldDefinition definition)
    {
        base.SetItem(definition);

        _file = null;
        _iconAsset = null;

        if (definition?.Name is not string worldPath || string.IsNullOrEmpty(worldPath))
        {
            Tooltip = "UnknownWorld";
            return;
        }

        Main.LoadWorlds();
        _file = Main.WorldList.FirstOrDefault(w => string.Equals(w.Path, worldPath, StringComparison.OrdinalIgnoreCase));

        if (_file == null)
        {
            Tooltip = definition.DisplayName ?? "UnknownWorld";
            return;
        }

        _iconAsset = GetIconAsset(_file);

        string worldName = definition.DisplayName ?? "UnknownWorld";

        byte sizeId = GetSizeId(_file);
        string sizeName = sizeId switch { 0 => "Small", 1 => "Medium", 2 => "Large", _ => "Unknown" };
        Color sizeColor = SizeColors[Math.Clamp(sizeId, (byte)0, (byte)2)];

        int diffId = GetDifficultyId(_file);
        string diffName = GetDifficultyName(_file);
        Color diffColor = DifficultyColors[Math.Clamp(diffId, 0, 3)];

        Tooltip = $"{worldName} ([c/{ToHex(sizeColor)}:{sizeName}]) ([c/{ToHex(diffColor)}:{diffName}])";
    }

    private static int GetDifficultyId(WorldFileData data)
    {
        if (data.GameMode == 3)
            return 0;

        return data.GameMode switch
        {
            0 => 1,
            1 => 2,
            2 => 3,
            _ => 1
        };
    }

    private static string ToHex(Color c) => $"{c.R:X2}{c.G:X2}{c.B:X2}";

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        CalculatedStyle dims = GetInnerDimensions();

        Rectangle bg = new(
            (int)dims.X,
            (int)dims.Y,
            (int)(BackgroundTexture.Width() * Scale),
            (int)(BackgroundTexture.Height() * Scale)
        );

        spriteBatch.Draw(BackgroundTexture.Value, bg, Color.White);

        int pad = (int)(2f * Scale);
        Rectangle content = bg;
        content.Inflate(-pad, -pad);

        DrawWorldIcon(spriteBatch, content);

        if (IsMouseHovering)
            UIModConfig.Tooltip = Tooltip;
    }

    private void DrawWorldIcon(SpriteBatch spriteBatch, Rectangle target)
    {
        if (_iconAsset == null)
            return;

        Texture2D tex = _iconAsset.Value;
        if (tex == null)
            return;

        int srcW = tex.Width;
        int srcH = tex.Height;

        float sx = (float)target.Width / srcW;
        float sy = (float)target.Height / srcH;
        float s = Math.Min(sx, sy);

        int drawW = (int)(srcW * s);
        int drawH = (int)(srcH * s);

        Rectangle dst = new(
            target.X + (target.Width - drawW) / 2,
            target.Y + (target.Height - drawH) / 2,
            drawW,
            drawH
        );

        spriteBatch.Draw(tex, dst, Color.White);
    }

    private static Asset<Texture2D> GetIconAsset(WorldFileData data)
    {
        if (data.ZenithWorld)
            return Main.Assets.Request<Texture2D>("Images/UI/Icon" + (data.IsHardMode ? "Hallow" : "") + "Everything");

        if (data.DrunkWorld)
            return Main.Assets.Request<Texture2D>("Images/UI/Icon" + (data.IsHardMode ? "Hallow" : "") + "CorruptionCrimson");

        if (data.ForTheWorthy)
            return GetSeedIcon(data, "FTW");

        if (data.NotTheBees)
            return GetSeedIcon(data, "NotTheBees");

        if (data.Anniversary)
            return GetSeedIcon(data, "Anniversary");

        if (data.DontStarve)
            return GetSeedIcon(data, "DontStarve");

        if (data.RemixWorld)
            return GetSeedIcon(data, "Remix");

        if (data.NoTrapsWorld)
            return GetSeedIcon(data, "Traps");

        return Main.Assets.Request<Texture2D>(
            "Images/UI/Icon" + (data.IsHardMode ? "Hallow" : "") + (data.HasCorruption ? "Corruption" : "Crimson")
        );
    }

    private static Asset<Texture2D> GetSeedIcon(WorldFileData data, string seed)
    {
        return Main.Assets.Request<Texture2D>(
            "Images/UI/Icon" + (data.IsHardMode ? "Hallow" : "") + (data.HasCorruption ? "Corruption" : "Crimson") + seed
        );
    }

    private static string GetDifficultyName(WorldFileData data)
    {
        if (data.GameMode == 3)
            return Language.GetTextValue("UI.Creative");

        int num = 1;
        switch (data.GameMode)
        {
            case 1: num = 2; break;
            case 2: num = 3; break;
        }

        if (data.ForTheWorthy)
            num++;

        return num switch
        {
            2 => Language.GetTextValue("UI.Expert"),
            3 => Language.GetTextValue("UI.Master"),
            4 => Language.GetTextValue("UI.Legendary"),
            _ => Language.GetTextValue("UI.Normal")
        };
    }

    private static byte GetSizeId(WorldFileData data)
    {
        if (data.WorldSizeX >= 8400) return 2;
        if (data.WorldSizeX >= 6400) return 1;
        return 0;
    }
}
