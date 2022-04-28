using System;
using AIGraph;
using GameData;
using HarmonyLib;
using LevelGeneration;

namespace Lockout_2_core
{
    class Patch_LG_LateGeomorphScanJob
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_LateGeomorphScanJob);
            var patchType = typeof(Patch_LG_LateGeomorphScanJob);

            instance.Patch(gameType.GetMethod("Build"), null, new HarmonyMethod(patchType, "Build"));
        }

        public static void Build()
        {
            OnLateGeomorphScanDone?.Invoke();
        }

        public static event Action OnLateGeomorphScanDone;
    }
}