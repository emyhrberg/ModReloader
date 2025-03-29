using System;

/// <summary>
/// Factory class for creating TimeSpans.
/// </summary>
namespace ModHelper.Helpers
{
    public static class TimeHelper
    {
        public static TimeSpan FromSeconds(double seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }
    }
}
