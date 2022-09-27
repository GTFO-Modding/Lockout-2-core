using CellMenu;
using HarmonyLib;
using UnityEngine;

namespace Lockout_2_core
{
    class Patch_CM_PageRundown_New
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(CM_PageRundown_New);
            var patchType = typeof(Patch_CM_PageRundown_New);

            instance.Patch(gameType.GetMethod("OnCortexDone"), null, new HarmonyMethod(patchType, "OnCortexDone"));
        }

        public static void OnCortexDone(CM_PageRundown_New __instance)
        {
            __instance.gameObject.transform.FindChild("MovingContent/Rundown/Button VanityItemDrops").position = Vector3.one * 999999;
            __instance.gameObject.transform.FindChild("MovingContent/Rundown/GUIX_layer_Tier_1/CM_PageRundown_VanityItemDropsNext(Clone)").position = Vector3.one * 999999;
        }
    }
}