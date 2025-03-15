namespace DeclutterCraneModeUI
{
    //TODO the way that I did this seems to only work for the host
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

        private EntityQuery _entityQuery;
        private List<GameObject> _uiElementsToHide = new List<GameObject>();

        // This is used to keep track of how many frames it has been since we last ran our logic.
        private uint _frameCount;

        private PreferenceSystemManager _prefManager;
        private const string ModEnabledPreferenceKey = "ModEnabledKey";

        public override void Initialise()
        {
            LogWarning($"v{ModInfo.ModVersion} in use!");

            _entityQuery = GetEntityQuery((ComponentType)typeof(CIsCraneMode));

            _prefManager = new PreferenceSystemManager(ModInfo.ModName, ModInfo.ModNameHumanReadable);
            _prefManager.AddLabel("Mod Enabled")
                       .AddOption(ModEnabledPreferenceKey, initialValue: true, values: new bool[] { false, true }, strings: new string[] { "Disabled", "Enabled" })
                       .AddSpacer()
                       .AddSpacer();

            _prefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        }

        protected override void OnUpdate()
        {
            // If the mod isn't enabled, then
            bool modEnabled = _prefManager.Get<bool>(ModEnabledPreferenceKey);
            if (!modEnabled)
            {
                return;
            }

            // Initialize these here because we can't do it in the Initialize() method since the  UI elements don't exist yet.
            // We'll want to keep trying until we find all 4 elements
            if (_uiElementsToHide.Count != 4)
            {
                // Finding and caching the UI objects for later
                _uiElementsToHide = _uiElementPaths.Select(path => GameObject.Find(path)).Where(obj => obj != null).ToList();
                LogInfo($"Found {_uiElementsToHide.Count} UI elements");
            }

            _frameCount++;
            // Only update once every 60 frames, this really doesn't need to be run every frame
            if (_frameCount % 60 != 0)
            {
                return;
            }

            bool playersInCraneMode = !_entityQuery.IsEmpty;

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
                _uiElementsToHide.Clear();
            }
        }
    }
}