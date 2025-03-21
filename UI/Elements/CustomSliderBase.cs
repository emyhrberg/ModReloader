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
        protected static UIElement CurrentLockedSlider;
        protected static UIElement CurrentAimedSlider;

        public static bool IsAnySliderLocked => CurrentLockedSlider != null;

        protected SliderUsageLevel UsageLevel =>
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