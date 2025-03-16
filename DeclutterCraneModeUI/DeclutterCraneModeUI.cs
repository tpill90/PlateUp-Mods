namespace DeclutterCraneModeUI
{
    //TODO the way that I did this seems to only work for the host
    //TODO need to probably check when the scene changes and reset everything when that happens
    [UsedImplicitly]
    public class DeclutterCraneModeUI : GenericSystemBase, IModSystem
    {
        private readonly List<string> _uiElementPaths = new List<string>
        {
            "/Camera/UI Camera/UI Container/Day Display(Clone)",
            "/Camera/UI Camera/UI Container/Time Display(Clone)",
            "/Camera/UI Camera/UI Container/Start Day Display(Clone)/Warning",
            "/Camera/UI Camera/UI Container/Parameters Display(Clone)"
        };

        private List<GameObject> _uiElementsToHide = new List<GameObject>();

        // This is used to keep track of how many frames it has been since we last ran our logic.
        private uint _frameCount;

        private PreferenceSystemManager _prefManager;
        private const string ModEnabledPreferenceKey = "ModEnabledKey";

        public override void Initialise()
        {
            base.Initialise();
            //TODO don't know if I need this or why
            RequireSingletonForUpdate<SIsNightTime>();

            LogWarning($"v{ModInfo.ModVersion} in use!");

            _prefManager = new PreferenceSystemManager(ModInfo.ModName, ModInfo.ModNameHumanReadable);
            _prefManager.AddLabel("Mod Enabled")
                       .AddOption(ModEnabledPreferenceKey, initialValue: true, values: new bool[] { false, true }, strings: new string[] { "Disabled", "Enabled" })
                       .AddSpacer()
                       .AddSpacer();

            _prefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);

            CraneActivators = GetEntityQuery(typeof(CActivatingCraneMode), typeof(CBlockPing), typeof(CInputData));
            PlayersWithoutCraneMode = GetEntityQuery(new QueryHelper().All(typeof(CPlayer), typeof(CInputData)).None(typeof(CActivatingCraneMode)));
        }

        EntityQuery CraneActivators;
        EntityQuery PlayersWithoutCraneMode;

        protected override void OnUpdate()
        {
            // If the mod isn't enabled, then there's nothing to do.
            bool modEnabled = _prefManager.Get<bool>(ModEnabledPreferenceKey);
            if (!modEnabled)
            {
                return;
            }

            // Should only be disabling this during night time.
            if (!HasSingleton<SIsNightTime>())
            {
                return;
            }

            _frameCount++;
            // Only update once every 60 frames, this really doesn't need to be run every frame
            if (_frameCount % 200 != 0)
            {
                return;
            }

            // Initialize these here because we can't do it in the Initialize() method since the UI elements don't exist yet.
            // We'll want to keep trying until we find all 4 elements
            if (_uiElementsToHide.Count != 4)
            {
                // Finding and caching the UI objects for later
                _uiElementsToHide = _uiElementPaths.Select(path => GameObject.Find(path)).Where(obj => obj != null).ToList();
                LogInfo($"Found {_uiElementsToHide.Count} UI elements");
            }

            //TODO is this needed?
            EntityManager.AddComponent<CActivatingCraneMode>(PlayersWithoutCraneMode);

            using NativeArray<Entity> entities = CraneActivators.ToEntityArray(Allocator.Temp);
            bool playersInCraneMode = entities.Any(e => Has<CIsCraneMode>(e));

            try
            {
                foreach (var gameObject in _uiElementsToHide)
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
            catch (Exception e)
            {
                LogInfo(e.Message);
                LogInfo("Something went wrong with SetActive().  Resetting UI element collection");
                //TODO this is a hack
                _uiElementsToHide.Clear();
            }
        }
    }
}