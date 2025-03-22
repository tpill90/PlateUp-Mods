namespace BepinExDebuggerShim
{
    [UsedImplicitly]
    [BepInPlugin($"tpill90.{ModInfo.ModName}", ModInfo.ModName, ModInfo.ModVersion)]
    public class BepinPluginTest : BaseUnityPlugin
    {
        public static ManualLogSource UnityLogger;

        [UsedImplicitly]
        public void Awake()
        {
            UnityLogger = Logger;
            UnityLogger.LogInfo("Loading BepinEx Debugger Shim");

            // Harmony isn't needed to make the shim work.  All we need is to reference the mod and use it in some way, which will cause it to be loaded.
            //TODO I do need a way to make this more dynamic
            UnityLogger.LogInfo($"{typeof(DeclutterCraneModeUI.DeclutterCraneModeUI).Name} loaded.");
            UnityLogger.LogInfo($"{typeof(FreeCameraControl.CameraPlus).Name} loaded.");
        }
    }
}