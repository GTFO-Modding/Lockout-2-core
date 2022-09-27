using CellMenu;
using HarmonyLib;

namespace Lockout_2_core
{
    class Patch_CM_ScrollWindowInfoBox
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(CM_ScrollWindowInfoBox);
            var patchType = typeof(Patch_CM_ScrollWindowInfoBox);

            instance.Patch(gameType.GetMethod("SetInfoBox"), new HarmonyMethod(patchType, "SetInfoBox"));
        }

        public static void SetInfoBox(CM_ScrollWindowInfoBox __instance, string subTitle, ref string description)
        {
            if (subTitle != "Thermal Vision") return;
            description = $"Limited battery life\nGreatly improves visibility in dark areas\nDetect invisible enemies through heat signatures\n\nPress <color=yellow>{EntryPoint.NightVisionKey.Value}</color> to toggle Thermal Vision";
        }
    }
}