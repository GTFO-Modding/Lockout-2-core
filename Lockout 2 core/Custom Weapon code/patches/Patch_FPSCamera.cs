using HarmonyLib;
using Lockout_2_core.Custom_Weapon_code;
using Lockout_2_core.Custom_Tools;

namespace Lockout_2_core
{
    class Patch_FPSCamera
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(FPSCamera);
            var patchType = typeof(Patch_FPSCamera);

            instance.Patch(gameType.GetMethod("Enable"), null, new HarmonyMethod(patchType, "Enable"));
        }

        public static void Enable(FPSCamera __instance)
        {
            L.Debug("FPS Camera enabled");
            var compManager_Blinder = __instance.gameObject.AddComponent<Manager_FlashbangBlinder>();
            compManager_Blinder.enabled = true;
            compManager_Blinder.GiveCamera(__instance);

            var compManager_NVG = __instance.gameObject.AddComponent<Manager_NightVision>();
            compManager_NVG.enabled = true;
        }
    }
}