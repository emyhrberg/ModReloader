using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public enum SliderUsageLevel
    {
        NotSelected,
        SelectedAndLocked,
        OtherElementIsLocked
    }

    public class CustomSliderBase : UIElement
    {
        internal static UIElement CurrentLockedSlider;
        internal static UIElement CurrentAimedSlider;

        internal SliderUsageLevel UsageLevel =>
            CurrentLockedSlider == this ? SliderUsageLevel.SelectedAndLocked :
            CurrentLockedSlider != null ? SliderUsageLevel.OtherElementIsLocked :
            SliderUsageLevel.NotSelected;

        public static void EscapeElements()
        {
            CurrentLockedSlider = null;
            CurrentAimedSlider = null;
        }
    }
}