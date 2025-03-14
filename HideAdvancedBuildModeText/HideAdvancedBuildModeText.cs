//TODO consider renaming this
namespace HideAdvancedBuildModeText
{
    [UsedImplicitly]
    public class HideAdvancedBuildModeText : GenericSystemBase, IModSystem
    {
        public override void Initialise()
        {
            LogWarning($"v{ModInfo.MOD_VERSION} in use!");

            base.Initialise();
        }

        //TODO is the performance OK on this?
        protected override void OnUpdate()
        {
            var cranePlayerQuery = GetEntityQuery((ComponentType)typeof(CIsCraneMode));
            var playersInCraneMode = !cranePlayerQuery.IsEmpty;

            LogInfo($"Players in crane mode : {playersInCraneMode}");
            var objectPaths = new List<string>
            {
                "/Camera/UI Camera/UI Container/Day Display(Clone)",
                "/Camera/UI Camera/UI Container/Time Display(Clone)",
                "/Camera/UI Camera/UI Container/Start Day Display(Clone)/Warning",
                "/Camera/UI Camera/UI Container/Parameters Display(Clone)"
            };
            foreach (var path in objectPaths)
            {
                var obj = GameObject.Find(path);
                if (obj != null)
                {
                    if (playersInCraneMode)
                    {
                        obj.SetActive(false);
                    }
                    else
                    {
                        obj.SetActive(true);
                    }
                }
            }
        }
    }
}
