using HarmonyLib;
using LevelGeneration;

namespace Lockout_2_core
{
    class Patch_LG_DoorButton
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_DoorButton);
            var patchType = typeof(Patch_LG_DoorButton);

            instance.Patch(gameType.GetMethod("OnWeakLockUnlocked"), new HarmonyMethod(patchType, "OnWeakLockUnlocked"));
        }

        public static bool OnWeakLockUnlocked()
        {
            if (RundownManager.Current.m_activeExpedition.LevelLayoutData != 1000
                || WardenObjectiveManager.Current.CheckWardenObjectiveCompleted(LG_LayerType.MainLayer)) return true;

            return false;
        }
    }
}