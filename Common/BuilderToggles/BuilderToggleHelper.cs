namespace ModReloader.Common.BuilderToggles
{
    public static class BuilderToggleHelper
    {
        public static bool GetActive()
        {
            var toggle = ModContent.GetInstance<MainStateBuilderToggle>();
            if (toggle == null)
            {
                Log.Info("MainStateBuilderToggle not found, defaulting to true.");
                return true; // Default to true if the toggle is not found
            }
            else
            {
                return toggle.CurrentState == 0; // Active when toggle is "On" (state 0)
            }
        }
    }
}
