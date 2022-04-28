using HarmonyLib;
using Lockout_2_core.Custom_Player_Behavior;
using Player;
using UnityEngine;

namespace Lockout_2_core
{
    class Patch_PlayerAgent
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(PlayerAgent);
            var patchType = typeof(Patch_PlayerAgent);

            instance.Patch(gameType.GetMethod("Setup"), null, new HarmonyMethod(patchType, "Setup"));
            instance.Patch(gameType.GetMethod("OnDespawn"), new HarmonyMethod(patchType, "OnDespawn"));
        }

        public static void Setup(PlayerAgent __instance)
        {
            var spectatorCamTarget = new GameObject("ThirdPersonCamTarget");
            spectatorCamTarget.transform.parent = __instance.transform;
            spectatorCamTarget.transform.localPosition = Vector3.zero;
            spectatorCamTarget.AddComponent<ThirdPersonCamTarget>();
        }

        public static void OnDespawn(PlayerAgent __instance)
        {
            L.Error($"Player was despawned!!!! does this patch actually run doe?????\nname {__instance} : local player? {__instance == PlayerManager.GetLocalPlayerAgent()}");

            if (__instance == PlayerManager.GetLocalPlayerAgent()) return;

            L.Debug($"s_IsThirdPerson: {Patch_FPSCamera_3RDPersonTest.s_IsThirdPerson}\nTargetPlayer is instance? {Patch_FPSCamera_3RDPersonTest.TargetPlayer == __instance}");

            if (Patch_FPSCamera_3RDPersonTest.s_IsThirdPerson && Patch_FPSCamera_3RDPersonTest.s_TargetPlayerCache == __instance)
            {
                Patch_FPSCamera_3RDPersonTest.UnTarget();
            }
        }
    }
}