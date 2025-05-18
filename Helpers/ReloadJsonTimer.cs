namespace ModReloader.Helpers
{
    public class PlayerTest : ModPlayer
    {
        public override void OnEnterWorld()
        {
            Log.Info("ReloadTimer: Player entered world");
        }
    }
}
