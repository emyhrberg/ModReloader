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

    public WorldDefinitionOptionElement(WorldDefinition definition, float scale = 0.75f) : base(definition, scale) { OverflowHidden = true; }

    // Initializes the option element with world info
    public override void SetItem(WorldDefinition definition)
    {
        base.SetItem(definition);

        _file = null;
        _iconAsset = null;

        if (definition?.Name is not string worldPath || string.IsNullOrEmpty(worldPath)) { Tooltip = "UnknownWorld"; return; }

        Main.LoadWorlds();
        _file = Main.WorldList.FirstOrDefault(w => string.Equals(w.Path, worldPath, StringComparison.OrdinalIgnoreCase));
        if (_file == null) { Tooltip = definition.DisplayName ?? "UnknownWorld"; return; }

        // Get icon asset
        _iconAsset = GetIconAsset(_file);

        // Get world info
        string worldName = definition.DisplayName ?? "UnknownWorld";
        int diffId = GetDifficultyId(_file);
        string diffName = GetDifficultyName(_file);
        Color diffColor = GetDifficultyColor(diffId);

        // Tooltip
        Tooltip = $"{worldName} [c/{ToHex(diffColor)}:({diffName})]";
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        CalculatedStyle dims = GetInnerDimensions();
        Rectangle bg = new((int)dims.X, (int)dims.Y, (int)(BackgroundTexture.Width() * Scale), (int)(BackgroundTexture.Height() * Scale));
        spriteBatch.Draw(BackgroundTexture.Value, bg, Color.White);

        Rectangle content = bg; content.Inflate(-(int)(2f * Scale), -(int)(2f * Scale));
        DrawWorldIcon(spriteBatch, content);

        if (IsMouseHovering) UIModConfig.Tooltip = Tooltip;
    }

    private void DrawWorldIcon(SpriteBatch spriteBatch, Rectangle target)
    {
        Texture2D tex = _iconAsset?.Value;
        if (tex == null) return;

        float s = Math.Min((float)target.Width / tex.Width, (float)target.Height / tex.Height);
        int w = (int)(tex.Width * s);
        int h = (int)(tex.Height * s);
        Rectangle dst = new(target.X + (target.Width - w) / 2, target.Y + (target.Height - h) / 2, w, h);

        spriteBatch.Draw(tex, dst, Color.White);
    }

    private static int GetDifficultyId(WorldFileData data)
    {
        if (data.GameMode == 3) return 0; // Journey
        int id = data.GameMode switch 
        { 
            0 => 1, // Normal
            1 => 2, // Expert
            2 => 3, // Master
            _ => 1 }; 

        if (data.ForTheWorthy) 
            id++; // FTW / Legendary

        return Math.Clamp(id, 0, 4);
    }

    private static Color GetDifficultyColor(int id)
    {
        return id switch
        {
            0 => Main.creativeModeColor,
            1 => Color.White,
            2 => Main.mcColor,
            3 => Main.hcColor,
            4 => Main.legendaryModeColor,
            _ => Color.White
        };
    }

    private static string GetDifficultyName(WorldFileData data)
    {
        if (data.GameMode == 3) return Language.GetTextValue("UI.Creative");

        int tier = data.GameMode switch 
        { 
            1 => 2, 
            2 => 3, 
            _ => 1 
        };
        if (data.ForTheWorthy) tier++;

        return tier switch
        {
            2 => Language.GetTextValue("UI.Expert"),
            3 => Language.GetTextValue("UI.Master"),
            4 => Language.GetTextValue("UI.Legendary"),
            _ => Language.GetTextValue("UI.Normal")
        };
    }

    private static string ToHex(Color c) => $"{c.R:X2}{c.G:X2}{c.B:X2}";

    private static Asset<Texture2D> GetIconAsset(WorldFileData data)
    {
        if (data.ZenithWorld) return Main.Assets.Request<Texture2D>("Images/UI/Icon" + (data.IsHardMode ? "Hallow" : "") + "Everything");
        if (data.DrunkWorld) return Main.Assets.Request<Texture2D>("Images/UI/Icon" + (data.IsHardMode ? "Hallow" : "") + "CorruptionCrimson");
        if (data.ForTheWorthy) return GetSeedIcon(data, "FTW");
        if (data.NotTheBees) return GetSeedIcon(data, "NotTheBees");
        if (data.Anniversary) return GetSeedIcon(data, "Anniversary");
        if (data.DontStarve) return GetSeedIcon(data, "DontStarve");
        if (data.RemixWorld) return GetSeedIcon(data, "Remix");
        if (data.NoTrapsWorld) return GetSeedIcon(data, "Traps");
        return Main.Assets.Request<Texture2D>("Images/UI/Icon" + (data.IsHardMode ? "Hallow" : "") + (data.HasCorruption ? "Corruption" : "Crimson"));
    }

    private static Asset<Texture2D> GetSeedIcon(WorldFileData data, string seed)
        => Main.Assets.Request<Texture2D>("Images/UI/Icon" + (data.IsHardMode ? "Hallow" : "") + (data.HasCorruption ? "Corruption" : "Crimson") + seed);

    internal static string GetDifficultyNameForFilter(WorldFileData data) => GetDifficultyName(data);
}
