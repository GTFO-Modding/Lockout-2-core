using HarmonyLib;
using Lockout_2_core.Custom_Player_Behavior;
using Player;
using UnityEngine;

namespace Lockout_2_core
{
    class Patch_PLOC_Downed
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(PLOC_Downed);
            var patchType = typeof(Patch_PLOC_Downed);

            instance.Patch(gameType.GetMethod("FixedUpdate"), new HarmonyMethod(patchType, "FixedUpdate"));
            instance.Patch(gameType.GetMethod("Enter"), null, new HarmonyMethod(patchType, "Enter"));
        }

        public static bool FixedUpdate(PLOC_Downed __instance)
        {
            if (PlayerDeathManager.Current.m_DeadPlayers.Contains(__instance.m_owner)) return false;
            return true;
        }

        public static void Enter()
        {
            if (s_PermaDeath) PlayerDeathManager.Current.KillLocalPlayerAgent();
            s_PermaDeath = false;
        }

        public static bool s_PermaDeath = false;
    }
}