using System;
using AIGraph;
using HarmonyLib;
using LevelGeneration;

namespace Lockout_2_core
{
    class Patch_LG_PopulateFunctionMarkersInZoneJob
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_PopulateFunctionMarkersInZoneJob);
            var patchType = typeof(Patch_LG_PopulateFunctionMarkersInZoneJob);

            instance.Patch(gameType.GetMethod("TriggerFunctionBuilder"), null, new HarmonyMethod(patchType, "TriggerFunctionBuilder"));
        }

        public static void TriggerFunctionBuilder()
        {
            OnTriggerFunctionBuilder?.Invoke();
        }

        public static event Action OnTriggerFunctionBuilder;
    }
}