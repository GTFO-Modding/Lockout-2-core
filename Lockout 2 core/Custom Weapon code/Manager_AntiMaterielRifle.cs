using Enemies;
using GameData;
using Gear;
using LevelGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Lockout_2_core.Custom_Weapon_code
{
    class Manager_AntiMaterielRifle
    {
        public static void Fire(BulletWeapon bulletWeapon)
        {
            L.Debug("Firing the anti materiel rifle");
            Vector3 origin = bulletWeapon.Owner.FPSCamera.transform.position;
            Vector3 direction = bulletWeapon.Owner.FPSCamera.transform.forward;
            Vector3 destination = origin + (direction + (direction.normalized * 100f));

            var bulletHits = Physics.OverlapCapsule(origin, destination, 0.02f, LayerManager.Current.GetMask(new string[] { "EnemyDamagable", "Dynamic" }));
            //CoroutineHandler.Add(DrawLine(origin, destination, 1));

            List<IntPtr> hitAlloc = new();
            IDamageable idamageable;
            Dam_PlayerDamageBase playerDamage;
            Dam_EnemyDamageLimb enemyLimb;
            EnemyAgent enemyTarget;
            LG_Zone enemyZone;
            float damage;

            foreach (var hit in bulletHits)
            {
                L.Debug("Hit!");
                idamageable = hit.GetComponent<IDamageable>();
                if (idamageable == null) continue;

                playerDamage = idamageable.TryCast<Dam_PlayerDamageBase>();
                if (playerDamage != null)
                {
                    if (hitAlloc.Contains(playerDamage.Pointer)) continue;
                    L.Debug("PlayerDamage");
                    hitAlloc.Add(playerDamage.Pointer);
                    damage = Mathf.Max(bulletWeapon.ArchetypeData.Damage, (-1.25f * (Vector3.Distance(playerDamage.transform.position, origin))) + (1.25f * bulletWeapon.ArchetypeData.Damage) + 10);
                    playerDamage.BulletDamage(damage, bulletWeapon.Owner, origin, direction, -direction, true, bulletWeapon.ArchetypeData.StaggerDamageMulti, bulletWeapon.ArchetypeData.PrecisionDamageMulti);
                }

                enemyLimb = idamageable.TryCast<Dam_EnemyDamageLimb>();
                enemyTarget = enemyLimb.GlueTargetEnemyAgent;
                if (enemyTarget != null)
                {
                    if (hitAlloc.Contains(enemyTarget.Pointer)) continue;

                    enemyZone = enemyTarget.CourseNode.m_zone;
                    if (enemyZone.DimensionIndex != bulletWeapon.Owner.DimensionIndex) continue;
                    if (enemyZone.LocalIndex != eLocalZoneIndex.Zone_0 && !enemyZone.m_sourceGate.HasBeenOpenedDuringPlay) continue;

                    hitAlloc.Add(enemyTarget.Pointer);

                    damage = Mathf.Max(bulletWeapon.ArchetypeData.Damage, (-1.25f * (Vector3.Distance(enemyTarget.transform.position, origin))) + (1.25f * bulletWeapon.ArchetypeData.Damage) + 10);
                    enemyLimb.BulletDamage(damage, bulletWeapon.Owner, origin, direction, -direction, true, bulletWeapon.ArchetypeData.StaggerDamageMulti, bulletWeapon.ArchetypeData.PrecisionDamageMulti);
                }
            }
        }

        /*
        public static IEnumerator DrawLine(Vector3 origin, Vector3 destination, float duration)
        {
            float r = 3;
            float g = 2.5f;
            float b = 1.5f;

            while (true)
            {
                r -= 0.01f; g -= 0.01f; b -= 0.01f;
                Debug.DrawLine(origin, destination, new Color(r, g, b), 0.05f, true);
                yield return new WaitForSeconds(duration);
                yield break;
            }
        }
        */
    }
}
