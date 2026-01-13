using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Configs.ConfigElements.PlayerPicker;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace ModReloader.Common.Configs.ConfigElements.PlayerPicker;

internal sealed class PlayerDefinitionOptionElement
    : DefinitionOptionElement<PlayerDefinition>
{
    private UICharacter _preview;

    public PlayerDefinitionOptionElement(PlayerDefinition definition,float scale = 0.75f): base(definition, scale)
    {
        OverflowHidden = true;
    }

    public override void SetItem(PlayerDefinition definition)
    {
        base.SetItem(definition);

        // Set tooltip
        Tooltip = definition?.ToString() ?? "UnknownPlayer";

        RemoveAllChildren();
        _preview = null;

        if (definition?.Name is not string playerPath)
            return;

        PlayerFileData file = Main.PlayerList.FirstOrDefault(p => p.Path == playerPath);

        if (file == null)
            return;

        // Update tooltip
        if (file?.Player != null && file.Player.difficulty == PlayerDifficultyID.Creative)
        {
            Tooltip += " " + "[c/4FD9FF:(" + "Journey" + ")]";
        }

        // Create a preview player from file
        Player previewPlayer = file.Player;
        previewPlayer.dead = false;

        _preview = new UICharacter(
            previewPlayer,
            animated: false,
            hasBackPanel: false, // no, we draw it manually later
            characterScale: 0.6f,
            useAClone: true
        );
        _preview.Top.Set(-6, 0);
        Recalculate();
        
        Append(_preview);
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

            if (IsMouseHovering)
            {
                _preview.SetAnimated(true);
            }
            else
            {

                _preview.SetAnimated(false);
            }
        }
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (IsMouseHovering)
            UIModConfig.Tooltip = Tooltip;

        // Position
        CalculatedStyle dimensions = GetInnerDimensions(); 
        Vector2 position = dimensions.Position(); 
        Vector2 size = BackgroundTexture.Size() * Scale;
        Rectangle destination = new((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

        // Draw background
        spriteBatch.Draw(BackgroundTexture.Value, destination, Color.White);
    }
}
