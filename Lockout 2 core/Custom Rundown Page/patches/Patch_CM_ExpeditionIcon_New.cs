using CellMenu;
using HarmonyLib;

namespace Lockout_2_core
{
    class Patch_CM_ExpeditionIcon_New
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(CM_ExpeditionIcon_New);
            var patchType = typeof(Patch_CM_ExpeditionIcon_New);

            instance.Patch(gameType.GetMethod("SetBorderColor"), null, new HarmonyMethod(patchType, "SetBorderColor"));
        }

        public static void SetBorderColor(CM_ExpeditionIcon_New __instance)
        {
            __instance.SetShortName(__instance.DataBlock.Descriptive.Prefix);
            __instance.SetFullName($"{__instance.DataBlock.Descriptive.Prefix} : {__instance.DataBlock.Descriptive.PublicName}");
        }
    }
}