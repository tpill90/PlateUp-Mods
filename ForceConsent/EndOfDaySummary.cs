namespace ForceConsent
{
    /// <summary>
    /// This class handles the end of day earnings report that requires all players to consent in order for the bar to move at full speed.
    /// If at least one person consents then the bar will move at full speed.
    ///
    /// Additionally speeds up the fill speed so that you don't have to wait as long to continue on to the next day.
    /// </summary>
    [UsedImplicitly]
    public class EndOfDaySummary : GenericSystemBase, IModSystem
    {
        public override void Initialise()
        {
            // Only runs if the popup is currently on screen
            var query = GetEntityQuery(new QueryHelper().All(typeof(CPopupEndDayData)));
            RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            var endOfDayPopup = GameObject.FindObjectOfType<EndOfDayPopupView>();

            // If nobody has consented yet then there is nothing to do.
            if (!endOfDayPopup.Consent.Consents.Values.Any(e => e))
            {
                return;
            }

            endOfDayPopup.Consent.Mode = ConsentElement.ConsentMode.AnyRequired;
            endOfDayPopup.Consent.FillSpeed = 2.5f;
            endOfDayPopup.Consent.SetAllConsents(true);
        }
    }
}