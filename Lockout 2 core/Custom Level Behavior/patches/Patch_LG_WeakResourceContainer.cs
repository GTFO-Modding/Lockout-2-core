using System;
using GameData;
using HarmonyLib;
using LevelGeneration;

namespace Lockout_2_core
{
    class Patch_LG_WeakResourceContainer
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_WeakResourceContainer);
            var patchType = typeof(Patch_LG_WeakResourceContainer);

            instance.Patch(gameType.GetMethod("SetupWeakLock"), new HarmonyMethod(patchType, "SetupWeakLock"));
        }

        public static void SetupWeakLock(ref eWeakLockType type)
        {
            if (RundownManager.ActiveExpedition.LevelLayoutData == 1008) type = eWeakLockType.Melee;
        }
    }
}