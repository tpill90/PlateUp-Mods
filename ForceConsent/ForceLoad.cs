namespace ForceConsent
{
    [UsedImplicitly]
    public class ForceLoad : GenericSystemBase, IModSystem
    {
        public override void Initialise()
        {
            // Only runs if the "return to restaurant" popup is currently on screen
            var query = GetEntityQuery(new QueryHelper().All(typeof(CLocationPopupRequest)));
            RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            var endOfDayPopup = GameObject.FindObjectOfType<GenericChoiceView>();
            if (endOfDayPopup == null)
            {
                return;
            }

            var LocalID = GetLocalPlayerID();

            // TODO make sure at least 1 player consented before continuing
            // Only force load a save if the host has consented

            if (endOfDayPopup.Consent.GetConsent(LocalID))
            {
                endOfDayPopup.Consent.SetAllConsents(true);
            }
        }

        // TODO figure out exactly what this is doing
        public int GetLocalPlayerID()
        {
            int LocalID = 0;

            foreach (PlayerInfo info in Kitchen.Players.Main.All())
            {
                if (info.IsLocalUser)
                {
                    //this.LogObject(info);
                    LocalID = info.ID;
                    break;
                }
            }

            return LocalID;
        }
    }
}
