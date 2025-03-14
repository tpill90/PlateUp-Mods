//TODO consider renaming this
//TODO write docs
//TODO create video and screenshots
//TODO create an icon for the mod, the one that shows up in search.
//TODO add preference system

namespace HideAdvancedBuildModeText
{
    [UsedImplicitly]
    public class HideAdvancedBuildModeText : GenericSystemBase, IModSystem
    {
        private readonly List<string> _uiElementPaths = new List<string>
        {
            "/Camera/UI Camera/UI Container/Day Display(Clone)",
            "/Camera/UI Camera/UI Container/Time Display(Clone)",
            "/Camera/UI Camera/UI Container/Start Day Display(Clone)/Warning",
            "/Camera/UI Camera/UI Container/Parameters Display(Clone)"
        };

        private EntityQuery _entityQuery;
        private List<GameObject> _uiElementsToHide;

        // This is used to keep track of how many frames it has been since we last ran our logic.
        private uint _frameCount;

        public override void Initialise()
        {
            LogWarning($"v{ModInfo.MOD_VERSION} in use!");

            _entityQuery = GetEntityQuery((ComponentType)typeof(CIsCraneMode));

            // Finding and caching the UI objects for later
            _uiElementsToHide = _uiElementPaths.Select(path => GameObject.Find(path)).Where(obj => obj != null).ToList();
        }

        protected override void OnUpdate()
        {
            _frameCount++;
            // Only update once every 100 frames, this really doesn't need to be run every frame
            if (_frameCount % 100 != 0)
            {
                return;
            }

            var playersInCraneMode = !_entityQuery.IsEmpty;
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
    }
}