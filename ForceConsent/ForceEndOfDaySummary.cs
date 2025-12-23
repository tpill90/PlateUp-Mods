namespace ForceConsent
{
    [UsedImplicitly]
    public class ForceEndOfDaySummary : GenericSystemBase, IModSystem
    {
        public override void Initialise()
        {
            RequireSingletonForUpdate<SIsNightTime>();
        }

        // TODO comment + cleanup + Test performance impact
        protected override void OnUpdate()
        {
            // TODO make finding a single object
            var endOfDayPopups = GameObject.FindObjectsOfType<EndOfDayPopupView>();
            if (endOfDayPopups == null || !endOfDayPopups.Any())
            {
                return;
            }

            var endOfDayPopup = endOfDayPopups.First();
            if (endOfDayPopup.Consent.Consents.Values.Any(e => e))
            {
                LogInfo("Forcing End of Day popup");
                endOfDayPopup.Consent.Mode = ConsentElement.ConsentMode.AnyRequired;
                endOfDayPopup.Consent.SetAllConsents(true);
            }
        }
    }
}
