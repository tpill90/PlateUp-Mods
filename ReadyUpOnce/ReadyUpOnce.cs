namespace ReadyUpOnce
{
    [UsedImplicitly]
    public class ReadyUpOnce : IModInitializer
    {
        public void PostActivate(Mod mod)
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            LogInfo($"v{ModInfo.ModVersion} in use!");
        }

        // These are empty on purpose
        public void PreInject() {}
        public void PostInject() {}
    }
}