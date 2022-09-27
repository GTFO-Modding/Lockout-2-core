using HarmonyLib;
using UnityEngine;
using Gear;
using Player;
using Lockout_2_core.Custom_Weapon_code;
using Enemies;
using Lockout_2_core.Custom_Level_Behavior;
using Feedback;
using System.Collections.Generic;

namespace Lockout_2_core
{
    partial class Patch_ProjectileManager
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(ProjectileManager);
            var patchType = typeof(Patch_ProjectileManager);

            instance.Patch(gameType.GetMethod("SpawnProjectileType"), null, new HarmonyMethod(patchType, "SpawnProjectileType"));
        }
        
        public static void SpawnProjectileType()
        {
            foreach (var renderer in ProjectileManager.s_tempGO.GetComponentsInChildren<Renderer>())
                renderer.gameObject.AddComponent<ShadowEnemyRenderer>();
        }
    }
}