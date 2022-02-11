using HarmonyLib;
using LevelGeneration;

namespace Lockout_2_core
{
    class Patch_LG_HSUActivator_Core
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_HSUActivator_Core);
            var patchType = typeof(Patch_LG_HSUActivator_Core);

            instance.Patch(gameType.GetMethod("OnStateChange"), new HarmonyMethod(patchType, "OnStateChange"));
        }

        public static void OnStateChange(pHSUActivatorState oldState)
        {
            if (RundownManager.ActiveExpedition.LevelLayoutData != 1001) return;
            if (oldState.status != eHSUActivatorStatus.ExtractionDone) return;
            oldState.status = eHSUActivatorStatus.WaitingForInsert;
        }
    }
}