namespace DeclutterCraneModeUI
{
    [UsedImplicitly]
    public class DeclutterCraneModeUI : IModInitializer
    {
        // Preference manager must be static otherwise it will be null after the mod is initially activated.
        public static PreferenceSystemManager PrefManager;
        public static readonly string ModEnabledPreferenceKey = "DeclutterCraneUI_ModEnabledKey";

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

        // These are empty on purpose
        public void PreInject() {}
        public void PostInject() {}
    }
}