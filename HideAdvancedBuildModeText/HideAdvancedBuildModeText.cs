#region usings

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cinemachine;
using HideAdvancedBuildModeText.Properties;
using JetBrains.Annotations;
using Kitchen;
using KitchenMods;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using Object = UnityEngine.Object;
using static HideAdvancedBuildModeText.LoggingUtils;
using HarmonyLib;

#endregion

namespace HideAdvancedBuildModeText
{
    [UsedImplicitly]
    public class HideAdvancedBuildModeText : GenericSystemBase, IModSystem
    {
        private static readonly Harmony m_harmony = new Harmony(nameof(HideAdvancedBuildModeText));

        protected override void Initialise()
        {
            LogWarning($"v{ModInfo.MOD_VERSION} in use!");

            m_harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        protected override void OnUpdate()
        {

        }
    }

    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CreateCraneExplainer), nameof(CreateCraneExplainer.OnUpdate))]
        public static bool HideBuildText()
        {
            LogInfo("Here");
            return false;
        }
    }
}
