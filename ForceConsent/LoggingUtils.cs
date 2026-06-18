namespace ForceConsent
{
    public static class LoggingUtils
    {
        // TODO undo these changes
        public static void LogInfo(string _log)
        {
            string formatted = DateTime.Now.ToString("HH:mm:ss.fffffff");
            Debug.Log($"[{formatted}][{ModInfo.ModName}] " + _log);
        }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{ModInfo.ModName}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{ModInfo.ModName}] " + _log); }
    }
}
