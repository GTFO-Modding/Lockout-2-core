using HarmonyLib;
using Lockout_2_core.Custom_Weapon_code;
using UnityEngine;

namespace Lockout_2_core
{
    class Patch_GrenadeBase
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(GrenadeBase);
            var patchType = typeof(Patch_GrenadeBase);

            instance.Patch(gameType.GetMethod("Awake"), new HarmonyMethod(patchType, "Awake"));
            instance.Patch(gameType.GetMethod("Start"), null, new HarmonyMethod(patchType, "Start"));
        }

        public static void Awake(GrenadeBase __instance)
        {
            var obj_GrenadeProjectile = new GameObject();

            var comp_GrenadeProjectile = __instance.gameObject.AddComponent<Item_GrenadeLauncher_Projectile>();
            comp_GrenadeProjectile.enabled = true;
        }

        public static void Start(GrenadeBase __instance)
        {
            __instance.enabled = false;
        }
    }
}