using CellMenu;
using GameData;
using HarmonyLib;
using Il2CppSystem;
using SNetwork;
using UnityEngine.CrashReportHandler;

namespace Lockout_2_core
{
    class Patch_CM_PageExpeditionSuccess
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(CM_PageExpeditionSuccess);
            var patchType = typeof(Patch_CM_PageExpeditionSuccess);

            instance.Patch(gameType.GetMethod("OnEnable"), null, new HarmonyMethod(patchType, "OnEnable"));
        }

        public static void OnEnable(CM_PageExpeditionSuccess __instance)
        {
            if (__instance.m_expeditionName != null && RundownManager.ActiveExpedition != null && RundownManager.ActiveExpedition.Descriptive != null)
            {
                string str = (TimeSpan.FromSeconds((double)Clock.ExpeditionProgressionTime) + TimeSpan.FromSeconds((double)Clock.ExpeditionCheckpointWastedTime)).ToString("hh':'mm':'ss");
                string text = RundownManager.ActiveExpedition.Descriptive.Prefix + ":\"" + RundownManager.ActiveExpedition.Descriptive.PublicName + "\"";
                text = text + "<color=#ccc><size=70%> - " + str + "</size></color>";
                __instance.m_expeditionName.text = text;
                //TextmeshRebuildManager.RegisterForRebuild(__instance.m_expeditionName, __instance.m_root);
            }
        }
    }
}