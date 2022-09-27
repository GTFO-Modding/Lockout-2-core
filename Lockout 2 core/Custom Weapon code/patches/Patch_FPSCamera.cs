using HarmonyLib;
using Lockout_2_core.Custom_Weapon_code;
using Lockout_2_core.Custom_Tools;
using System;

namespace Lockout_2_core
{
    class Patch_FPSCamera
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(FPSCamera);
            var patchType = typeof(Patch_FPSCamera);

            instance.Patch(gameType.GetMethod("OnEnable"), null, new HarmonyMethod(patchType, "OnEnable"));
        }

        public static void OnEnable(FPSCamera __instance)
        {
            L.Debug("FPS Camera enabled");

            if (__instance.gameObject.GetComponent<Manager_FlashbangBlinder>() == null)
            {
                var compManager_Blinder = __instance.gameObject.AddComponent<Manager_FlashbangBlinder>();
                compManager_Blinder.enabled = true;
                compManager_Blinder.GiveCamera(__instance);
            }

            if (__instance.gameObject.GetComponent<Manager_NightVision>() == null)
            {
                var compManager_NVG = __instance.gameObject.AddComponent<Manager_NightVision>();
                compManager_NVG.m_FPSCamera = __instance;
                compManager_NVG.enabled = true;
            }
        }
    }
}