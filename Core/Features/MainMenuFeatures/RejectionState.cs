using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

internal class RejectionState : UIState
{
    private readonly string message;
    private static readonly Dictionary<string, float> _scale = new();
    private static readonly HashSet<string> _hoverWas = new();

    public RejectionState(string message)
    {
        this.message = message;
    }

    public override void OnInitialize()
    {
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        // Centered message
        Vector2 centerMessage = new(Main.screenWidth / 2, Main.screenHeight / 2-120);
        DrawCenteredMultiline(sb, message, centerMessage, 0.6f);

        // Back button under it
        Vector2 centerButton = new(Main.screenWidth / 2, Main.screenHeight / 2 + 150);
        DrawItem(sb, "back", "Back", centerButton);
    }

    public static void DrawCenteredMultiline(SpriteBatch sb, string message, Vector2 center, float scale = 0.6f)
    {
        var font = FontAssets.DeathText.Value;
        string[] lines = message.Split('\n');
        float lineHeight = font.MeasureString("A").Y * scale;
        float totalHeight = lines.Length * lineHeight;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string clean = StripTags(line);
            Vector2 size = font.MeasureString(clean) * scale; // correct width
            Vector2 pos = new(center.X - size.X / 2f, center.Y - totalHeight / 2f + i * lineHeight);

            // Draw with full tags (keeps colors)
            ChatManager.DrawColorCodedStringWithShadow(sb, font, line, pos, Color.White, 0f, Vector2.Zero, new Vector2(scale));

        }
    }

    public static void DrawItem(SpriteBatch sb, string id, string text, Vector2 center, float baseScale = 1f)
    {
        // Handle hover
        var font = FontAssets.DeathText.Value;
        if (!_scale.TryGetValue(id, out var scale)) scale = baseScale;
        Vector2 size = font.MeasureString(text);
        Rectangle hit = new(
            (int)(center.X - (size.X * scale) * 0.5f),
            (int)(center.Y - (size.Y * scale) * 0.5f),
            (int)(size.X * scale),
            (int)(size.Y * scale)
        );
        bool hovered = hit.Contains(Main.MouseScreen.ToPoint());
        float target = hovered ? baseScale * 1.18f : baseScale;
        scale = MathHelper.Lerp(scale, target, 0.3f);
        _scale[id] = scale;
        if (hovered && !_hoverWas.Contains(id))
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            _hoverWas.Add(id);
        }
        if (!hovered) _hoverWas.Remove(id);
        Color color = hovered ? Color.Yellow : new Color(230, 230, 230);

        // Draw text
        ChatManager.DrawColorCodedStringWithShadow(sb, font, text, center, color, 0f, size * 0.5f, new Vector2(scale));

        // Handle click or escape
        bool clicked = hovered && Main.mouseLeft && Main.mouseLeftRelease;
        bool escape = Main.keyState.IsKeyDown(Keys.Escape);

        if (clicked || escape)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.menuMode = 0;
            Main.MenuUI.SetState(null);
        }
    }

    private static readonly Regex colorTagRegex = new(@"\[c\/[0-9A-Fa-f]{6}:(.*?)\]", RegexOptions.Compiled);

    private static string StripTags(string text)
    {
        // Replace [c/xxxxxx:Text] → Text
        return colorTagRegex.Replace(text, "$1");
    }

}
