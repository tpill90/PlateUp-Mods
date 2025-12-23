namespace ForceConsent
{
    // TODO upload test mod
    // TODO Update readme
    // TODO post that there is an updated fix and link to my mod on the original
    // TODO maybe make a PR for the original mod.  And see if the original author is on discord
    // TODO rename all of the Force*.cs classes to something better.
    // TODO port over some of the new debugger shim changes to this repo.
    [UsedImplicitly]
    public class ForceConsent : GenericSystemBase, IModSystem
    {
        // TODO probably don't need this, but need it for now for debugging to work
        public override void Initialise()
        {
            LogInfo($"v{ModInfo.ModVersion} in use!");
        }

        protected override void OnUpdate()
        {
        }
    }
}