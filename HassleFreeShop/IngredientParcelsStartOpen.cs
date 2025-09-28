namespace KitchenHassleFreeShop
{
    public class IngredientParcelsStartOpen : NightSystem, IModSystem
    {
        EntityQuery parcelsQuery;

        public override void Initialise()
        {
            base.Initialise();
            parcelsQuery = GetEntityQuery(new QueryHelper()
                .All(typeof(CLetterIngredient), typeof(CPosition)));
        }

        protected override void OnUpdate()
        {
            if ((Main.PrefManager?.Get<int>(Main.ENABLED_PREFERENCE_ID) ?? 0) == 1)
            {
                NativeArray<Entity> parcels = parcelsQuery.ToEntityArray(Allocator.Temp);
                foreach (Entity parcel in parcels)
                {
                    if (!Require(parcel, out CLetterIngredient letter) || !Require(parcel, out CPosition position))
                    {
                        continue;
                    }
                    CreateIngredientAppliance(letter, position);
                    EntityManager.DestroyEntity(parcel);
                    Main.LogInfo($"Destroyed Parcel.");
                }
            }
        }

        protected void CreateIngredientAppliance(CLetterIngredient letter, CPosition position)
        {
            int iD = base.Data.ReferableObjects.DefaultProvider.ID;
            if (base.Data.TryGet<Item>(letter.IngredientID, out var output, warn_if_fail: true))
            {
                Appliance dedicatedProvider = output.DedicatedProvider;
                iD = ((dedicatedProvider == null) ? base.Data.ReferableObjects.DefaultProvider.ID : dedicatedProvider.ID);
            }
            Entity entity = EntityManager.CreateEntity();
            Set(entity, new CCreateAppliance
            {
                ID = iD
            });
            Set(entity, CItemProvider.InfiniteItemProvider(letter.IngredientID));
            Set(entity, new CPosition(position));

            //string providerName = GameData.Main.TryGet(iD, out Appliance appliance) ? appliance.name : "Unknown";
            //Main.LogInfo($"Created Ingredient Provider {providerName} ({iD}) from parcel.");
        }
    }
}
