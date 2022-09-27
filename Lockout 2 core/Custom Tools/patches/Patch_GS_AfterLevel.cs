using System;
using HarmonyLib;
using Lockout_2_core.Custom_Player_Behavior;

namespace Lockout_2_core
{
    class Patch_GS_AfterLevel
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(GS_AfterLevel);
            var patchType = typeof(Patch_GS_AfterLevel);

            instance.Patch(gameType.GetMethod("CleanupAfterExpedition"), null,  new HarmonyMethod(patchType, "CleanupAfterExpedition"));
        }

        public static void CleanupAfterExpedition()
        {
            OnLevelCleanup?.Invoke();
            Patch_PLOC_Downed.s_PermaDeath = false;
            PlayerDeathManager.Current.m_LocalPlayerDeathIncurred = false;
        }

        public static event Action OnLevelCleanup;
    }
}