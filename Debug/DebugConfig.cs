namespace ModHelper.Debug
{
    public static class DebugConfig
    {
        // public static bool IsDebugMode => System.Diagnostics.Debugger.IsAttached || System.Diagnostics.Debugger.IsLogging();
        public static bool IS_DEBUGGING => true; // This should be set to true in debug builds but false in release builds.
    }
}
