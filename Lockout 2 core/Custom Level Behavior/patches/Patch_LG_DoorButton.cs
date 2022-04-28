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
            if (RundownManager.Current.m_activeExpedition.LevelLayoutData != 1000 && RundownManager.Current.m_activeExpedition.LevelLayoutData != 1008) return true;
            if (WardenObjectiveManager.Current.CheckWardenObjectiveCompleted(LG_LayerType.MainLayer) || s_Override) return true;

            return false;
        }

        public static bool s_Override = false;
    }
}