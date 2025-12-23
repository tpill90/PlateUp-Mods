namespace ForceConsent
{
    // TODO comment + cleanup
    //TODO should this even exist at all?  It might be a bit of a surprise when you accidentially click save in the middle of a run and lose your progress.  Could be a setting.
    [UsedImplicitly]
    public class ForceSave : GenericSystemBase, IModSystem
    {
        int LocalID = 0;
        EntityQuery EntityQuery;

        public override void Initialise()
        {
            RequireSingletonForUpdate<SIsNightTime>();
            EntityQuery = GetEntityQuery((ComponentType)typeof(CRequestSave));
        }

        // TODO uncomment
        protected override void OnUpdate()
        {
            var consentElements = GameObject.FindObjectsOfType<ConsentElement>();
            if (consentElements == null || consentElements.Length < 2)
            {
                // If there isn't 2 ConsentElements it means that there hasn't been any prompt to confirm exiting to lobby
                return;
            }

            // TODO fix this its not getting past this point when trying to save
            // If its not asking to save then we don't do anything
            if (EntityQuery.IsEmpty)
            {
                return;
            }

            ConsentElement elem = consentElements[1];
            // At least one person has to consent
            if (elem.Consents.Values.Any(e => e))
            {
                LogInfo("Force Saving and returning to lobby");
                elem.Mode = ConsentElement.ConsentMode.AnyRequired;
                elem.SetAllConsents(true);
            }
        }
    }
}
