using GameData;
using HarmonyLib;
using Il2CppSystem;
using SNetwork;
using UnityEngine.CrashReportHandler;

namespace Lockout_2_core
{
    class Patch_PUI_GameObjectives
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(PUI_GameObjectives);
            var patchType = typeof(Patch_PUI_GameObjectives);

            //instance.Patch(gameType.GetMethod("SetHeader"), new HarmonyMethod(patchType, "SetHeader"));
        }

        /*
        public static void SetHeader(ref string txt)
        {
            if (RundownManager.ActiveExpedition == null || RundownManager.ActiveExpedition.Descriptive == null) return;
            txt = $"{RundownManager.ActiveExpedition.Descriptive.Prefix} : {RundownManager.ActiveExpedition.Descriptive.PublicName}";
        }
        */
    }
}