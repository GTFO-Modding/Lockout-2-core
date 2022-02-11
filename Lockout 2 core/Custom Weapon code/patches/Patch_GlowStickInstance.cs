using GameData;
using HarmonyLib;
using UnityEngine;

namespace Lockout_2_core
{
    class Patch_GlowstickInstance
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(GlowstickInstance);
            var patchType = typeof(Patch_GlowstickInstance);

            instance.Patch(gameType.GetMethod("Setup"), null, new HarmonyMethod(patchType, "Setup"));
        }

        public static void Setup(GlowstickInstance __instance, ItemDataBlock data)
        {
            if (data.persistentID != 5001) return;

            var obj_FlashGrenade = new GameObject();
            GameObject.DontDestroyOnLoad(obj_FlashGrenade);

            var comp_FlashGrenade = __instance.gameObject.AddComponent<Custom_Weapon_code.Item_FlashGrenade_Throwable>();
            comp_FlashGrenade.enabled = true;
        }
    }
}