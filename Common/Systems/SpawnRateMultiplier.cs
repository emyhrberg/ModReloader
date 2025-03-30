namespace ModHelper.Common.Systems
{
    public static class SpawnRateMultiplier
    {
        public static float Multiplier
        {
            get => SpawnRateSystem.Multiplier;
            set => SpawnRateSystem.SetMultiplier(value);
        }

        public static bool didPrint = false;

        public static void SetSpawnRateMultiplier(float value)
        {
            Multiplier = value;
        }
    }
}