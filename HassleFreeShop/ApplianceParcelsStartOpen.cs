namespace KitchenHassleFreeShop
{
    public class ApplianceParcelsStartOpen : NightSystem, IModSystem
    {
        EntityQuery parcelsQuery;

        public override void Initialise()
        {
            base.Initialise();
            parcelsQuery = GetEntityQuery(new QueryHelper()
                .All(typeof(CLetterAppliance), typeof(CPosition)));
        }

        protected override void OnUpdate()
        {
            if ((Main.PrefManager?.Get<int>(Main.ENABLED_PREFERENCE_ID) ?? 0) == 1)
            {
                NativeArray<Entity> parcels = parcelsQuery.ToEntityArray(Allocator.Temp);
                foreach (Entity parcel in parcels)
                {
                    if (!Require(parcel, out CLetterAppliance letter) || !Require(parcel, out CPosition position))
                    {
                        continue;
                    }
                    CreateAppliance(letter, position);
                    EntityManager.DestroyEntity(parcel);
                    Main.LogInfo($"Destroyed Parcel.");
                }
            }
        }

        protected void CreateAppliance(CLetterAppliance letter, CPosition position)
        {
            Entity entity = EntityManager.CreateEntity();
            Set(entity, new CCreateAppliance
            {
                ID = letter.ApplianceID
            });
            Set(entity, new CPosition(position));

            //string applianceName = GameData.Main.TryGet(letter.ApplianceID, out Appliance appliance) ? appliance.name : "Unknown";
            //Main.LogInfo($"Created Appliance {applianceName} ({letter.ApplianceID}) from parcel.");
        }
    }
}
