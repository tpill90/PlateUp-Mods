namespace ForceConsent
{
    [UsedImplicitly]
    public class ForceLoad : GenericSystemBase, IModSystem
    {
        int LocalID = 0;

        public override void Initialise()
        {
            // Only runs when we're in the lobby
            RequireSingletonForUpdate<SFranchiseMarker>();
        }

        protected override void OnUpdate()
        {
            //TODO figure out what this is for
            if(LocalID == 0)
            {
                LocalID = GetLocalPlayerID();
            }

            ConsentElement[] consentElements = GameObject.FindObjectsOfType<ConsentElement>();

            if(consentElements.Length < 2 )
            {
                return;
            }

            // TODO make sure at least 1 player consented before continuing
            //TODO figure out what this is for
            ConsentElement elem = consentElements[1];
            if (elem.GetConsent(this.LocalID))
            {
                elem.SetAllConsents(true);
            }
        }

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
