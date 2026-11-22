using Microsoft.Xna.Framework.Graphics;
using ModReloader.Core.Features;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace ModReloader.Common.Configs.ConfigElements.PlayerPicker;

internal sealed class PlayerDefinitionOptionElement : DefinitionOptionElement<PlayerDefinition>
{
    private UICharacter _preview;
    private Player _previewPlayer;
    private readonly bool isActiveSelection;

    public PlayerDefinitionOptionElement(PlayerDefinition definition, float scale = 0.75f, bool isActiveSelection = false) : base(null, scale)
    {
        this.isActiveSelection = isActiveSelection;
        OverflowHidden = true;
        NullID = -1;
        SetItem(definition);
    }

    public override void SetItem(PlayerDefinition definition)
    {
        base.SetItem(definition);

        Player player = definition?.File?.Player;
        if (player == null)
        {
            Tooltip = "No Player Selected";
            RemoveAllChildren();
            _preview = null;
            _previewPlayer = null;
            return;
        }

        Tooltip = player.name;
        if (player.difficulty == PlayerDifficultyID.Creative)
        {
            Tooltip += Utilities.ColorToTerrariaString(Main.creativeModeColor, " (Journey)");
        }

        if (ReferenceEquals(_previewPlayer, player))
            return;

        RemoveAllChildren();
        _previewPlayer = player;
        player.dead = false;

        float charScale = isActiveSelection ? 0.6f : 0.8f;
        _preview = new UICharacter(player, animated: false, hasBackPanel: false, characterScale: charScale, useAClone: true);
        _preview.Top.Set(-6f, 0f);

        Append(_preview);
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
