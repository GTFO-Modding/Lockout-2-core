using GameData;
using HarmonyLib;
using Il2CppSystem;
using SNetwork;
using System;
using UnityEngine.CrashReportHandler;

namespace Lockout_2_core
{
    class Patch_RundownManager
    {
        [System.Obsolete]
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(RundownManager);
            var patchType = typeof(Patch_RundownManager);

            instance.Patch(gameType.GetMethod("GetExpeditionShortName"), null, new HarmonyMethod(patchType, "GetExpeditionShortName"));
        }

        public static void GetExpeditionShortName(ref ExpeditionInTierData data, string __result)
        {
            __result = data.Descriptive.Prefix;
        }
    }
}