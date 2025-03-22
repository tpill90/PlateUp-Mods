// TODO add ability to enable + disable + pick which slot
namespace AutoRestaurantLoader
{
    [UsedImplicitly]
    [UpdateAfter(typeof(CreateLocationsRoom))]
    public class AutoLoadRestaurant : FranchiseFirstFrameSystem, IModSystem
    {
        private int saveSlot = 4;

        private EntityQuery Query;

        public override void Initialise()
        {
            base.Initialise();
            Query = GetEntityQuery(new QueryHelper().All(typeof(CLocationChoice)));
            LogWarning($" loaded !");
        }

        protected override void OnUpdate()
        {
            using var entities = Query.ToEntityArray(Allocator.TempJob);
            foreach (var entity in entities)
            {
                if (Require(entity, out CLocationChoice location))
                {
                    if (location.Slot == saveSlot && location.State == SaveState.Loaded)
                    {
                        Set<SSelectedLocation>(new SSelectedLocation
                        {
                            Valid = true,
                            Selected = location
                        });

                        Entity e = base.EntityManager.CreateEntity(new ComponentType[]
                        {
                            typeof(SPerformSceneTransition),
                            typeof(CDoNotPersist)
                        });
                        base.EntityManager.SetComponentData<SPerformSceneTransition>(e, new SPerformSceneTransition
                        {
                            NextScene = SceneType.LoadFullAutosave
                        });
                    }
                }
            }
        }

        #region Logging

        public static string MOD_NAME = "AutoLoadRestaurant";
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }


}