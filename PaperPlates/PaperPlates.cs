namespace PaperPlates
{
    [UsedImplicitly]
    public class PaperPlates : GenericSystemBase, IModSystem
    {
        private PreferenceSystemManager _prefManager;
        private const string ModEnabledPreferenceKey = "PaperPlates_ModEnabledKey";

        public override void Initialise()
        {
            LogInfo($"v{ModInfo.ModVersion} in use!");

            _prefManager = new PreferenceSystemManager(ModInfo.ModName, ModInfo.ModNameHumanReadable);
            _prefManager.AddLabel("Mod Enabled")
                        .AddLabel("(Change requires restart!)")
                        .AddOption(ModEnabledPreferenceKey, initialValue: true, values: new bool[] { false, true }, strings: new string[] { "Disabled", "Enabled" })
                        .AddSpacer()
                        .AddSpacer();
            _prefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
            _prefManager.RegisterMenu(PreferenceSystemManager.MenuType.MainMenu);

            // Sinks must be disabled before the game actually loads, otherwise the changes won't be applied correctly.
            ConfigureSinkUsability();

            // Will apply the mod once per in game day
            RequireSingletonForUpdate<SIsDayFirstUpdate>();
        }

        // This will only apply the mod once per day when the day starts.
        protected override void OnUpdate()
        {
            var modEnabled = _prefManager.Get<bool>(ModEnabledPreferenceKey);

            // Allowing plates to be thrown away. When IsIndisposable is set to false it will be possible to throw them away.
            GameData.Main.Get<Item>(ItemIDs.PlateDirty).IsIndisposable = !modEnabled;
            GameData.Main.Get<Item>(ItemIDs.Plate).IsIndisposable = !modEnabled;

            // Finding all currently instantiated appliances.  This includes plate stacks as well as sinks.
            EntityQuery query = GetEntityQuery(new QueryHelper().All(typeof(CAppliance)));
            using var entities = query.ToEntityArray(Allocator.TempJob);

            foreach (var entity in entities)
            {
                // Setting the amount of plates in each plate stack instance
                ConfigurePlateStackAmounts(modEnabled, entity);
            }
        }

        private void ConfigurePlateStackAmounts(bool modEnabled, Entity entity)
        {
            var appliance = EntityManager.GetComponentData<CAppliance>(entity);
            if (!ItemIDs.PlateStacks.Contains(appliance.ID))
            {
                return;
            }

            var itemProvider = EntityManager.GetComponentData<CItemProvider>(entity);

            if (modEnabled)
            {
                // Setting the amount of plates to an absurdly high number, effectively making it "infinite" since there is no way to use this many in a day.
                // This value will be reset every day.
                itemProvider.Maximum = 200_000;
                itemProvider.Available = 200_000;
            }
            else
            {
                // Resets the plate amounts back to their default when the mod is disabled.
                if (appliance.ID == ItemIDs.PlateStackStarting || appliance.ID == ItemIDs.AutoPlater)
                {
                    itemProvider.Maximum = 4;
                    itemProvider.Available = 4;
                }
                if (appliance.ID == ItemIDs.PlateStack)
                {
                    itemProvider.Maximum = 8;
                    itemProvider.Available = 8;
                }
            }
            EntityManager.SetComponentData(entity, itemProvider);
        }

        /// <summary>
        /// This must be run prior to the game fully loading, otherwise the changes won't be applied.
        /// This can't be applied at runtime since it's essentially modifying the "base template" of the appliances, which then gets
        /// cloned and instantiated at runtime.  Once the appliances have been cloned off of the "base" version the base version is never referenced again.
        /// </summary>
        private void ConfigureSinkUsability()
        {
            var modEnabled = _prefManager.Get<bool>(ModEnabledPreferenceKey);
            if (!modEnabled)
            {
                return;
            }

            foreach (var sinkId in ItemIDs.Sinks)
            {
                Appliance appliance = GameData.Main.Get<Appliance>(sinkId);
                var process = appliance.Processes[0];
                process.Speed = 0.0f;
                appliance.Processes[0] = process;
            }
        }
    }
}