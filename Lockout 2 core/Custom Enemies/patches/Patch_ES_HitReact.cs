using HarmonyLib;
using UnityEngine;
using Gear;
using Player;
using Lockout_2_core.Custom_Weapon_code;
using Enemies;
using Lockout_2_core.Custom_Level_Behavior;

namespace Lockout_2_core
{
    partial class Patch_ES_HitReact
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(ES_Hitreact);
            var patchType = typeof(Patch_ES_HitReact);

            instance.Patch(gameType.GetMethod("Enter"), null, new HarmonyMethod(patchType, "Enter"));
        }

        public static void Enter(ES_HibernateWakeUp __instance)
        {
            if (__instance.m_enemyAgent.EnemyData.persistentID != 47) return;
            Manager_CustomLevelBehavior.D1.m_Boss.SyncTryClose();
        }
    }
}