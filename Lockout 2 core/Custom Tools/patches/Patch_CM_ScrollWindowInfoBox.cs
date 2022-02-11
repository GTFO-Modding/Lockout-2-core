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
            if (subTitle != "Night Vision") return;
            description = description + $"\n\nPress <color=green>{EntryPoint.NightVisionKey.Value}</color> to toggle Night Vision";
        }
    }
}