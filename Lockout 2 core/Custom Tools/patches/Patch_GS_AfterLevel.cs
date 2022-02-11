using System;
using HarmonyLib;

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
        }

        public static event Action OnLevelCleanup;
    }
}