using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements;

public class CustomSlider : CustomSliderBase
{
    private Color _color;
    private LocalizedText _textKey;
    private Func<float> _getStatusTextAct;
    private Action<float> _slideKeyboardAction;
    private Func<float, Color> _blipFunc;
    private Action _slideGamepadAction;
    private const bool BOTHER_WITH_TEXT = false;
    private bool _isReallyMouseOvered;
    private bool _alreadyHovered;
    private bool _soundedUsage;

    public CustomSlider(LocalizedText textKey, Func<float> getStatus, Action<float> setStatusKeyboard, Action setStatusGamepad, Func<float, Color> blipColorFunction, Color color)
    {
        _color = color;
        _textKey = textKey;
        _getStatusTextAct = ((getStatus != null) ? getStatus : ((Func<float>)(() => 0f)));
        _slideKeyboardAction = ((setStatusKeyboard != null) ? setStatusKeyboard : ((Action<float>)delegate
        {
        }));

        _blipFunc = ((blipColorFunction != null) ? blipColorFunction : ((Func<float, Color>)((float s) => Color.Lerp(Color.Black, Color.White, s))));
        _slideGamepadAction = setStatusGamepad;
        _isReallyMouseOvered = false;

        // CUSTOM SIZES
        Width.Set(170f, 0f);
        Height.Set(15f, 0f); // this has no effect?


        // CUSTOM OFFSETS, UNSURE ABOUT THIS
        // this sets the size in the element
        VAlign = 0.0f;
        HAlign = 0.0f;
        Left.Set(130f, 0f);
        Top.Set(5f, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        // hot reload testing
        Width.Set(180f, 0f);
        Height.Set(15f, 0f);
        Top.Set(5f, 0f);

        CustomSliderBase.CurrentAimedSlider = null;
        if (!Main.mouseLeft)
            CustomSliderBase.CurrentLockedSlider = null;

        int usageLevel = GetUsageLevel();
        float num = 8f;

        // draw the yellow highlighting around the bar i think
        base.DrawSelf(spriteBatch);
        CalculatedStyle dimensions = GetDimensions();
        float num2 = dimensions.Width + 1f;
        Vector2 vector = new Vector2(dimensions.X, dimensions.Y);
        bool flag = false;
        bool flag2 = base.IsMouseHovering;
        if (usageLevel == 2)
            flag2 = false;

        if (usageLevel == 1)
            flag2 = true;

        Color value = (flag ? Color.Gold : (flag2 ? Color.White : Color.Silver));
        value = Color.Lerp(value, Color.White, flag2 ? 0.5f : 0f);
        TextureAssets.ColorBar.Frame();

        // actual position of the entire slider here
        Vector2 vector2 = new(dimensions.X + dimensions.Width, dimensions.Y + num);


        bool wasInBar;
        float obj = DrawValueBar(spriteBatch, vector2, 1f, _getStatusTextAct(), usageLevel, out wasInBar, _blipFunc);
        if (CustomSliderBase.CurrentLockedSlider == this || wasInBar)
        {
            CustomSliderBase.CurrentAimedSlider = this;
            if (PlayerInput.Triggers.Current.MouseLeft && !PlayerInput.UsingGamepad && CustomSliderBase.CurrentLockedSlider == this)
            {
                _slideKeyboardAction(obj);
                if (!_soundedUsage)
                    // SoundEngine.PlaySound(12);

                    _soundedUsage = true;
            }
            else
            {
                _soundedUsage = false;
            }
        }

        if (CustomSliderBase.CurrentAimedSlider != null && CustomSliderBase.CurrentLockedSlider == null)
            CustomSliderBase.CurrentLockedSlider = CustomSliderBase.CurrentAimedSlider;

        if (_isReallyMouseOvered)
            _slideGamepadAction();
    }

    private float DrawValueBar(SpriteBatch sb, Vector2 drawPosition, float drawScale, float sliderPosition, int lockMode, out bool wasInBar, Func<float, Color> blipColorFunc)
    {
        Texture2D value = TextureAssets.ColorBar.Value;
        Vector2 vector = new Vector2(value.Width, value.Height) * drawScale;
        drawPosition.X -= (int)vector.X;
        Rectangle rectangle = new Rectangle((int)drawPosition.X, (int)drawPosition.Y - (int)vector.Y / 2, (int)vector.X, (int)vector.Y);
        Rectangle destinationRectangle = rectangle;
        sb.Draw(value, rectangle, Color.White);
        float num = (float)rectangle.X + 5f * drawScale;
        float num2 = (float)rectangle.Y + 4f * drawScale;
        for (float num3 = 0f; num3 < 167f; num3 += 1f)
        {
            float arg = num3 / 167f;
            Color color = blipColorFunc(arg);
            sb.Draw(TextureAssets.ColorBlip.Value, new Vector2(num + num3 * drawScale, num2), null, color, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 0f);
        }

        rectangle.X = (int)num - 2;
        rectangle.Y = (int)num2;
        rectangle.Width -= 4;
        rectangle.Height -= 8;
        bool flag = (_isReallyMouseOvered = rectangle.Contains(new Point(Main.mouseX, Main.mouseY)));
        if (IgnoresMouseInteraction)
            flag = false;

        if (lockMode == 2)
            flag = false;

        if (flag || lockMode == 1)
        {
            sb.Draw(TextureAssets.ColorHighlight.Value, destinationRectangle, Main.OurFavoriteColor);
            if (!_alreadyHovered)
                // SoundEngine.PlaySound(12);

                _alreadyHovered = true;
        }
        else
        {
            _alreadyHovered = false;
        }

        wasInBar = false;
        if (!IgnoresMouseInteraction)
        {
            // draw the knob
            sb.Draw(TextureAssets.ColorSlider.Value, new Vector2(num + 167f * drawScale * sliderPosition, num2 + 4f * drawScale), null, Color.White, 0f, new Vector2(0.5f * (float)TextureAssets.ColorSlider.Value.Width, 0.5f * (float)TextureAssets.ColorSlider.Value.Height), drawScale, SpriteEffects.None, 0f);
            if (Main.mouseX >= rectangle.X && Main.mouseX <= rectangle.X + rectangle.Width)
            {
                wasInBar = flag;
                return (float)(Main.mouseX - rectangle.X) / (float)rectangle.Width;
            }
        }

        if (rectangle.X >= Main.mouseX)
            return 0f;

        return 1f;
    }
}
