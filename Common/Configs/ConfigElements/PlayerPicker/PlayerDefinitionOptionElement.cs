using Microsoft.Xna.Framework.Graphics;
using ModReloader.Core.Features;
using System;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace ModReloader.Common.Configs.ConfigElements.PlayerPicker;

internal sealed class PlayerDefinitionOptionElement : DefinitionOptionElement<PlayerDefinition>
{
    private UICharacter _preview;
    private bool isActiveSelection;

    public PlayerDefinitionOptionElement(PlayerDefinition definition, float scale = 0.75f, bool isActiveSelection = false) : base(definition, scale)
    {
        this.isActiveSelection = isActiveSelection;
        OverflowHidden = true;
        NullID = -1;
    }

    public override void SetItem(PlayerDefinition definition)
    {
        base.SetItem(definition);

        RemoveAllChildren();
        _preview = null;

        Main.LoadPlayers();
        if (definition == null || definition.IsUnloaded)
        {
            Tooltip = "No Player Selected";
            Recalculate();
            return;
        }

        PlayerFileData file = Utilities.FindPlayer(definition.Type);

        Player player = file.Player;
        player.dead = false;

        Tooltip = player.name;
        if (player.difficulty == PlayerDifficultyID.Creative)
        {
            Tooltip += Utilities.ColorToTerrariaString(Main.creativeModeColor, " (Journey)");
        }

        float charScale = isActiveSelection ? 0.6f : 0.8f;
        _preview = new UICharacter(player, animated: false, hasBackPanel: false, characterScale: charScale, useAClone: true);
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
