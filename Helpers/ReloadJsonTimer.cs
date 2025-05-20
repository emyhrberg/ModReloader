namespace ModReloader.Helpers
{
    public class ReloadJsonTimerTimer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            Log.Info("ReloadTimer: Player entered world");
        }
    }
}
