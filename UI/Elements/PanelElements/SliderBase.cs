using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements
{
    public enum SliderUsageLevel
    {
        NotSelected,
        SelectedAndLocked,
        OtherElementIsLocked
    }

    public class SliderBase : UIElement
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