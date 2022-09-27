using HarmonyLib;
using LevelGeneration;
using System;

namespace Lockout_2_core
{
    class Patch_LG_PowerGeneratorClusterObjectiveEndSequence
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_PowerGeneratorCluster._ObjectiveEndSequence_d__16);
            var patchType = typeof(Patch_LG_PowerGeneratorClusterObjectiveEndSequence);

            instance.Patch(gameType.GetMethod("MoveNext"), null, new HarmonyMethod(patchType, "MoveNext"));
        }

        public static void MoveNext(LG_PowerGeneratorCluster._ObjectiveEndSequence_d__16 __instance)
        {
            if (__instance.__1__state == -1) OnGenClusterAlarmStart?.Invoke();
        }

        public static event Action OnGenClusterAlarmStart;
    }
}