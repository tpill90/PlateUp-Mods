namespace DeclutterCraneModeUI
{
    [UsedImplicitly]
    public class DeclutterCraneModeUI : IModInitializer
    {
        public static PreferenceSystemManager PrefManager;
        public const string ModEnabledPreferenceKey = "DeclutterCraneUI_ModEnabledKey";

        public void PostActivate(Mod mod)
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            PrefManager = new PreferenceSystemManager(ModInfo.ModName, ModInfo.ModNameHumanReadable);
            PrefManager.AddLabel("Mod Enabled")
                       .AddOption(ModEnabledPreferenceKey, initialValue: true, values: new bool[] { false, true }, strings: new string[] { "Disabled", "Enabled" })
                       .AddSpacer()
                       .AddSpacer();
            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);


            LogInfo($"v{ModInfo.ModVersion} in use!");
        }

        public void PreInject()
        {
        }

        public void PostInject()
        {
        }
    }
}