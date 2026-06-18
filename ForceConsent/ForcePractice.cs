namespace ForceConsent
{
    // TODO comment and cleanup
    [UsedImplicitly]
    public class ForceStartPracticeMode : GenericSystemBase, IModSystem
    {
        private EntityQuery _popupQuery;

        public override void Initialise()
        {
            // TODO make sure that this isn't running on every frame, and only runs when the popup is visible.
            // Can only go to practice mode if we're in preparation mode
            RequireSingletonForUpdate<SIsNightTime>();
            _popupQuery = GetEntityQuery(typeof(StartPracticePopup.CRequest));
        }

        //TODO comment + cleanup
        protected override void OnUpdate()
        {
            // Skip if there is no confirmation popup displayed.
            if (_popupQuery.IsEmpty)
            {
                return;
            }

            ConsentElement[] consentElements = GameObject.FindObjectsOfType<ConsentElement>();
            if (consentElements.Length < 2)
            {
                return;
            }

            ConsentElement elemTwo = consentElements[1];
            elemTwo.Mode = ConsentElement.ConsentMode.AnyRequired;
            elemTwo.SetAllConsents(true);
        }
    }

    //TODO comment + cleanup
    [UsedImplicitly]
    public class ForceLeavePracticeMode : LeavePracticeMode, IModSystem
    {
        public override void Initialise()
        {
            // Only run if we're in practice mode, can't leave practice mode if we're not in practice mode!
            RequireSingletonForUpdate<SPracticeMode>();
        }

        public override void OnUpdate()
        {
            EndPracticeView view = GameObject.FindObjectOfType<EndPracticeView>();
            if (!view)
            {
                return;
            }

            // If anyone is ready then we're going to exit practice mode
            if (view.Consent.Consents.Values.Any(e => e))
            {
                LogInfo("Leaving Practice Mode");

                // We're exiting practice mode this way rather than changing the consent mode to ConsentMode.AnyRequired
                // because it's significantly faster at exiting.
                var leavePracticeView = GetOrDefault<SLeavePracticeView>();
                leavePracticeView.Ready = true;
                Set(leavePracticeView);
            }
        }
    }
}
