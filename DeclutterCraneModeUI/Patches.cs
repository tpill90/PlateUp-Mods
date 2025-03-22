namespace DeclutterCraneModeUI
{
    [HarmonyPatch]
    public static class Patches
    {
        private static readonly List<string> _uiElementPaths = new List<string>
        {
            "/Camera/UI Camera/UI Container/Day Display(Clone)",
            "/Camera/UI Camera/UI Container/Time Display(Clone)",
            "/Camera/UI Camera/UI Container/Start Day Display(Clone)/Warning",
            "/Camera/UI Camera/UI Container/Parameters Display(Clone)"
        };

        [HarmonyPatch(typeof(LocalViewRouter), nameof(LocalViewRouter.GetPrefab))]
        [HarmonyPostfix]
        public static void LocalViewRouterGetPrefab(ViewType view_type, ref GameObject __result)
        {
            // If the mod isn't enabled, then there's nothing to do.
            bool modEnabled = DeclutterCraneModeUI.PrefManager.Get<bool>(DeclutterCraneModeUI.ModEnabledPreferenceKey);
            if (!modEnabled)
            {
                return;
            }

            var uiElementsToHide = _uiElementPaths.Select(path => GameObject.Find(path)).Where(obj => obj != null).ToList();
            foreach (var gameObject in uiElementsToHide)
            {
                if (view_type == ViewType.PlayerCrane)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(true);
                }
            }
        }
    }
}
