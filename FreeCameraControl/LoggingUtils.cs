using FreeCameraControl.Properties;
using System.Reflection;
using System;
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

        public static void LogObject(object obj)
        {
            if (obj == null)
            {
                LogInfo("Object is null");
                return;
            }

            var type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //TODO order these by name
            LogInfo($"Log Object {type.Name} has {properties.Length} properties {Environment.NewLine})");
            //TODO make blank line function
            LogInfo("");
            // Log each property and its value
            foreach (PropertyInfo property in properties)
            {
                object value;
                try
                {
                    value = property.GetValue(obj);
                }
                catch (Exception ex)
                {
                    value = $"Error retrieving value: {ex.Message}";
                }

                LogInfo($"{property.Name}: {value}{Environment.NewLine}");
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            //TODO order these by name
            LogInfo("");
            LogInfo($"{fields.Length} fields {Environment.NewLine})");
            //TODO make blank line function
            LogInfo("");
            // Log each property and its value
            foreach (var field in fields)
            {
                object value;
                try
                {
                    value = field.GetValue(obj);
                }
                catch (Exception ex)
                {
                    value = $"Error retrieving value: {ex.Message}";
                }

                LogInfo($"{field.Name}: {value}{Environment.NewLine}");
            }

            LogInfo("---------------------------------------------------------------");
        }
    }
}
