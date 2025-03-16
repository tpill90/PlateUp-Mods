namespace DeclutterCraneModeUI
{
    [UsedImplicitly]
    public class DeclutterCraneModeUI : GenericSystemBase, IModInitializer, IModSystem
    {
        private readonly List<string> _uiElementPaths = new List<string>
        {
            "/Camera/UI Camera/UI Container/Day Display(Clone)",
            "/Camera/UI Camera/UI Container/Time Display(Clone)",
            "/Camera/UI Camera/UI Container/Start Day Display(Clone)/Warning",
            "/Camera/UI Camera/UI Container/Parameters Display(Clone)"
        };

        private EntityQuery _craneActivators;

        // This is used to keep track of how many frames it has been since we last ran our logic.
        private uint _frameCount;

        private PreferenceSystemManager _prefManager;
        private const string ModEnabledPreferenceKey = "ModEnabledKey";

        // This is called first before anything else.
        public void PostActivate(Mod mod)
        {
            LogWarning($"v{ModInfo.ModVersion} in use!");

            _prefManager = new PreferenceSystemManager(ModInfo.ModName, ModInfo.ModNameHumanReadable);
            _prefManager.AddLabel("Mod Enabled")
                        .AddOption(ModEnabledPreferenceKey, initialValue: true, values: new bool[] { false, true }, strings: new string[] { "Disabled", "Enabled" })
                        .AddSpacer()
                        .AddSpacer();

            _prefManager.RegisterMenu(PreferenceSystemManager.MenuType.MainMenu);
            _prefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        }

        // Initializes the mod systems
        public override void Initialise()
        {
            base.Initialise();

            // Setting RequireSingletonForUpdate() means that our Update() method will only trigger if this object currently exists
            // Should only disable UI elements during nighttime/preparation phase
            RequireSingletonForUpdate<SIsNightTime>();
            _craneActivators = GetEntityQuery(typeof(CActivatingCraneMode), typeof(CBlockPing), typeof(CInputData));
        }

        protected override void OnUpdate()
        {
            // If the mod isn't enabled, then there's nothing to do.
            bool modEnabled = _prefManager.Get<bool>(ModEnabledPreferenceKey);
            if (!modEnabled)
            {
                return;
            }

            _frameCount++;
            // Only update once every 60 frames, this really doesn't need to be run every frame
            if (_frameCount % 60 != 0)
            {
                return;
            }

            using NativeArray<Entity> craneEntities = _craneActivators.ToEntityArray(Allocator.Temp);
            bool playersInCraneMode = craneEntities.Any(e => Has<CIsCraneMode>(e));

            var uiElementsToHide = _uiElementPaths.Select(path => GameObject.Find(path)).Where(obj => obj != null).ToList();
            foreach (var gameObject in uiElementsToHide)
            {
                if (playersInCraneMode)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(true);
                }
            }
        }

        // These are empty on purpose because we don't use them
        public void PreInject() {}
        public void PostInject() {}
    }
}