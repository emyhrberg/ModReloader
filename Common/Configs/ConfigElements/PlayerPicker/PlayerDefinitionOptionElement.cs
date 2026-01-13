using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace ModReloader.Common.Configs.ConfigElements.PlayerPicker;

internal sealed class PlayerDefinitionOptionElement: DefinitionOptionElement<PlayerDefinition>
{
    private UICharacter _preview;
    private bool isActiveSelection;

    public PlayerDefinitionOptionElement(PlayerDefinition definition,float scale = 0.75f, bool isActiveSelection=false): base(definition, scale)
    {
        this.isActiveSelection = isActiveSelection;
        OverflowHidden = true;
    }

    public override void SetItem(PlayerDefinition definition)
    {
        base.SetItem(definition);

        RemoveAllChildren();
        _preview = null;

        string path = definition?.Name;
        if (string.IsNullOrEmpty(path)) { Tooltip = "UnknownPlayer"; return; }

        Main.LoadPlayers();
        PlayerFileData file = Main.PlayerList.FirstOrDefault(p => string.Equals(p.Path, path, StringComparison.OrdinalIgnoreCase));
        if (file?.Player == null) { Tooltip = definition?.ToString() ?? "UnknownPlayer"; return; }

        Tooltip = definition?.ToString() ?? "UnknownPlayer";
        if (file.Player.difficulty == PlayerDifficultyID.Creative) { Color c = Main.creativeModeColor; Tooltip += $" [c/{c.R:X2}{c.G:X2}{c.B:X2}:(Journey)]"; }

        Player p = file.Player;
        p.dead = false;

        float charScale = isActiveSelection ? 0.6f : 0.8f;
        _preview = new UICharacter(p, animated: false, hasBackPanel: false, characterScale: charScale, useAClone: true);
        _preview.Top.Set(-6f, 0f);

        Append(_preview);
        Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Animate when hovering
        if (_preview != null)
        {
            // HOT RELOAD TESTING
            //_preview.Top.Set(-6, 0);
            //_preview._characterScale = 0.5f;

            _preview.SetAnimated(IsMouseHovering);
        }
        
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        // Position
        CalculatedStyle dimensions = GetInnerDimensions(); 
        Vector2 position = dimensions.Position(); 
        Vector2 size = BackgroundTexture.Size() * Scale;
        Rectangle destination = new((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

        // Draw background
        spriteBatch.Draw(BackgroundTexture.Value, destination, Color.White);

        // Update tooltip
        if (IsMouseHovering)
            UIModConfig.Tooltip = Tooltip;
    }
}
