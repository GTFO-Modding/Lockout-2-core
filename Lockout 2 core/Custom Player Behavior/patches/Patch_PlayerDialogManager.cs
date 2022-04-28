using HarmonyLib;
using Lockout_2_core.Custom_Player_Behavior;
using Player;
using UnityEngine;

namespace Lockout_2_core
{
    class Patch_PlayerDialogManager
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(PlayerDialogManager);
            var patchType = typeof(Patch_PlayerDialogManager);

            instance.Patch(gameType.GetMethod("WantToStartDialog", new System.Type[] { typeof(uint), typeof(int), typeof(bool), typeof(bool) }), new HarmonyMethod(patchType, "WantToStartDialog"));
        }

        public static bool WantToStartDialog(int playerID)
        {
            if (PlayerDeathManager.Current.m_DeadPlayerIDs.Contains(playerID)) return false;
            return true;
        }
    }
}