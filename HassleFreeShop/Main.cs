// Namespace should have "Kitchen" in the beginning
namespace KitchenHassleFreeShop
{
    public class Main : IModInitializer
    {
        public const string MOD_GUID = "IcedMilo.PlateUp.HassleFreeShop";
        public const string MOD_NAME = "Hassle Free Shop";
        public const string MOD_VERSION = "0.1.4";

        public const string ENABLED_PREFERENCE_ID = "Enabled";

        internal static PreferenceSystemManager PrefManager;

        public Main()
        {

        }

        public void PostActivate(KitchenMods.Mod mod)
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");

            PrefManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);
            PrefManager
                .AddLabel("Blueprints and Parcels Auto Open")
                .AddOption<int>(
                    ENABLED_PREFERENCE_ID,
                    1,
                    new int[] { 0, 1 },
                    new string[] { "Disabled", "Enabled" })
                .AddSpacer()
                .AddSpacer();

            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        }

        public void PreInject() { }

        public void PostInject() { }

        #region Logging
        // You can remove this, I just prefer a more standardized logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
