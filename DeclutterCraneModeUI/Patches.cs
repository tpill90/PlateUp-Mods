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

        private static readonly List<int> _craneViewIds = new List<int>();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalViewRouter), nameof(LocalViewRouter.RemoveView))]
        public static void LocalViewRouter_RemoveView(ViewIdentifier id)
        {
            // If the mod isn't enabled, then there's nothing to do.
            bool modEnabled = DeclutterCraneModeUI.PrefManager.Get<bool>(DeclutterCraneModeUI.ModEnabledPreferenceKey);
            if (!modEnabled)
            {
                return;
            }

            // Only want to hide/unhide the UI elements if we're exiting crane mode.
            if (!_craneViewIds.Contains(id.Identifier))
            {
                return;
            }

            _craneViewIds.Remove(id.Identifier);
            var uiElementsToHide = _uiElementPaths.Select(path => GameObject.Find(path)).Where(obj => obj != null).ToList();
            foreach (var gameObject in uiElementsToHide)
            {
                if (_craneViewIds.Any())
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(true);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalViewRouter), nameof(LocalViewRouter.CreateNewView))]
        public static void LocalViewRouter_CreateNewView(ViewIdentifier id, ViewType view_type, ViewMode view_mode, bool is_redraw)
        {
            // If the mod isn't enabled, then there's nothing to do.
            bool modEnabled = DeclutterCraneModeUI.PrefManager.Get<bool>(DeclutterCraneModeUI.ModEnabledPreferenceKey);
            if (!modEnabled)
            {
                return;
            }

            if (view_type != ViewType.PlayerCrane)
            {
                return;
            }

            _craneViewIds.Add(id.Identifier);

            var uiElementsToHide = _uiElementPaths.Select(path => GameObject.Find(path)).Where(obj => obj != null).ToList();
            foreach (var gameObject in uiElementsToHide)
            {
                gameObject.SetActive(false);
            }
        }
    }
}