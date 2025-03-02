﻿using FreeCameraControl.Properties;
using UnityEngine;

namespace FreeCameraControl
{
    public static class LoggingUtils
    {
        public static void LogInfo(string _log) { Debug.Log($"[{ModInfo.MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{ModInfo.MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{ModInfo.MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
    }
}
