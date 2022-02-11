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
    partial class Patch_InfectionSpitter
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(InfectionSpitter);
            var patchType = typeof(Patch_InfectionSpitter);

            instance.Patch(gameType.GetMethod("AssignCourseNode"), null, new HarmonyMethod(patchType, "AssignCourseNode"));
            instance.Patch(gameType.GetMethod("TryPlaySound"), null, new HarmonyMethod(patchType, "TryPlaySound"));
        }
        
        public static void AssignCourseNode(InfectionSpitter __instance)
        {
            if (RundownManager.ActiveExpedition.LevelLayoutData != 1005) return;
            var fireAlignGO = new GameObject("fireAlign");
            fireAlignGO.transform.parent = __instance.transform;
            fireAlignGO.transform.localPosition = new(0, -0.15f, 0);
        }
        public static void TryPlaySound(InfectionSpitter __instance, uint id)
        {
            if (RundownManager.ActiveExpedition.LevelLayoutData != 1005 || id != AK.EVENTS.INFECTION_SPITTER_SPIT) return;
            Vector3 firePos = __instance.transform.FindChild("fireAlign").position;
            List<PlayerAgent> agentTargets = new();

            for (var i = 0; i <= 5; i++)
            {
                agentTargets.Add(PlayerManager.PlayerAgentsInLevel[((int)__instance.Pointer + i) % PlayerManager.PlayerAgentsInLevel.Count]);
            }

            ProjectileManager.WantToFireTargeting(ProjectileType.TargetingSmall, agentTargets[0], firePos, (__instance.transform.position - firePos).normalized + new Vector3((float)new System.Random().NextDouble() * 0.3f, (float)new System.Random().NextDouble() * 0.3f, (float)new System.Random().NextDouble() * 0.3f));
            ProjectileManager.WantToFireTargeting(ProjectileType.TargetingSmall, agentTargets[1], firePos, (__instance.transform.position - firePos).normalized + new Vector3((float)new System.Random().NextDouble() * 0.3f, (float)new System.Random().NextDouble() * 0.3f, (float)new System.Random().NextDouble() * 0.3f));
            ProjectileManager.WantToFireTargeting(ProjectileType.TargetingSmall, agentTargets[2], firePos, (__instance.transform.position - firePos).normalized + new Vector3((float)new System.Random().NextDouble() * 0.3f, (float)new System.Random().NextDouble() * 0.3f, (float)new System.Random().NextDouble() * 0.3f));
            ProjectileManager.WantToFireTargeting(ProjectileType.TargetingSmall, agentTargets[3], firePos, (__instance.transform.position - firePos).normalized + new Vector3((float)new System.Random().NextDouble() * 0.3f, (float)new System.Random().NextDouble() * 0.3f, (float)new System.Random().NextDouble() * 0.3f));
            ProjectileManager.WantToFireTargeting(ProjectileType.TargetingSmall, agentTargets[4], firePos, (__instance.transform.position - firePos).normalized + new Vector3((float)new System.Random().NextDouble() * 0.3f, (float)new System.Random().NextDouble() * 0.3f, (float)new System.Random().NextDouble() * 0.3f));
        }
    }
}