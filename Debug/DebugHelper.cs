namespace ModHelper.Debug
{
    public static class DebugHelper
    {
        // public static bool IsDebugMode => System.Diagnostics.Debugger.IsAttached || System.Diagnostics.Debugger.IsLogging();
        public static bool IsDebugBuild => true; // This should be set to true in debug builds
        public static bool IsReleaseBuild => false; // This should be set to false in release builds
    }
}
