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
                        .AddOption(ModEnabledPreferenceKey, initialValue: true, values: new bool[] { false, true }, strings: new string[] { "Disabled", "Enabled" }, ModEnabledPreferenceChanged)
                        .AddSpacer()
                        .AddSpacer();
            _prefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
            _prefManager.RegisterMenu(PreferenceSystemManager.MenuType.MainMenu);

            // Will apply the mod once per in game day
            RequireSingletonForUpdate<SIsDayFirstUpdate>();
        }

        private void ModEnabledPreferenceChanged(bool newSettingValue)
        {
            ApplyMod(newSettingValue);
        }

        // This will only apply the mod once per day when the day starts.
        protected override void OnUpdate()
        {
            var modEnabled = _prefManager.Get<bool>(ModEnabledPreferenceKey);
            ApplyMod(modEnabled);
        }

        private void ApplyMod(bool modEnabled)
        {
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

                // Making dirty plates be unable to be washed
                ConfigureSinkUsability(modEnabled, entity);
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
                itemProvider.Maximum = 20_000;
                itemProvider.Available = 20_000;
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

        private void ConfigureSinkUsability(bool modEnabled, Entity entity)
        {
            var appliance = EntityManager.GetComponentData<CAppliance>(entity);
            if (!ItemIDs.Sinks.Contains(appliance.ID))
            {
                return;
            }

            if (modEnabled && Has<CItemHolder>(entity))
            {
                // Removing the CItemHolder prevents sinks from being able to have an item inserted into them.
                EntityManager.RemoveComponent<CItemHolder>(entity);
            }
            else if (!modEnabled && !Has<CItemHolder>(entity))
            {
                // Readding the component allows sinks to hold items again.
                EntityManager.AddComponentData(entity, new CItemHolder());
            }
        }
    }
}