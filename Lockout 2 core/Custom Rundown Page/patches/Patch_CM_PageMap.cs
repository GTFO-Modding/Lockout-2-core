using CellMenu;
using GameData;
using HarmonyLib;
using Il2CppSystem;
using SNetwork;
using UnityEngine.CrashReportHandler;

namespace Lockout_2_core
{
    class Patch_CM_PageMap
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(CM_PageMap);
            var patchType = typeof(Patch_CM_PageMap);

            instance.Patch(gameType.GetMethod("SetExpeditionName"), new HarmonyMethod(patchType, "SetExpeditionName"));
        }

        public static void SetExpeditionName(ref string name)
        {
            if (RundownManager.ActiveExpedition == null || RundownManager.ActiveExpedition.Descriptive == null) return;
            name = $"{RundownManager.ActiveExpedition.Descriptive.Prefix} : {RundownManager.ActiveExpedition.Descriptive.PublicName}";
        }
    }
}