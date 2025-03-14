//TODO consider renaming this
//TODO write docs
//TODO create video and screenshots
//TODO create an icon for the mod, the one that shows up in search.

namespace DeclutterCraneModeUI
{
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
            // Initialize these here because we can't do it in the Initialize() method since the  UI elements dont exist yet.
            // We'll want to keep trying until we find all 4 elements
            if (_uiElementsToHide.Count != 4)
            {
                // Finding and caching the UI objects for later
                _uiElementsToHide = _uiElementPaths.Select(path => GameObject.Find(path)).Where(obj => obj != null).ToList();
            }

            _frameCount++;
            // Only update once every 60 frames, this really doesn't need to be run every frame
            if (_frameCount % 60 != 0)
            {
                return;
            }

            bool modEnabled = _prefManager.Get<bool>(ModEnabledPreferenceKey);
            bool playersInCraneMode = !_entityQuery.IsEmpty;

            foreach (var gameObject in _uiElementsToHide)
            {
                if (modEnabled && playersInCraneMode)
                {
                    LogInfo("here");
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(true);
                }
            }
        }
    }
}