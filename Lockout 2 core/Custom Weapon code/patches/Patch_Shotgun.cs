using HarmonyLib;
using UnityEngine;
using Gear;
using Player;
using Lockout_2_core.Custom_Weapon_code;
using Agents;
using FX_EffectSystem;

namespace Lockout_2_core
{
    partial class Patch_Shotgun
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(Shotgun);
            var patchType = typeof(Patch_Shotgun);

            instance.Patch(gameType.GetMethod("Fire"), new HarmonyMethod(patchType, "Fire"));
        }

        public static bool Fire(Shotgun __instance, ref bool resetRecoilSimilarity)
        {
			Vector3 position = __instance.MuzzleAlign.position;
			if (__instance.Owner.FPSCamera.CameraRayDist < 1f)
			{
				position = __instance.Owner.FPSCamera.Position;
			}
			for (int i = 0; i < __instance.ArchetypeData.ShotgunBulletCount; i++)
			{
				float f = __instance.m_segmentSize * (float)i;
				float num = 0f;
				float num2 = 0f;
				if (i > 0)
				{
					num += (float)__instance.ArchetypeData.ShotgunConeSize * Mathf.Cos(f);
					num2 += (float)__instance.ArchetypeData.ShotgunConeSize * Mathf.Sin(f);
				}

				s_LocalRayData = new Weapon.WeaponHitData
				{
					maxRayDist = __instance.MaxRayDist,
					angOffsetX = num,
					angOffsetY = num2,
					randomSpread = (float)__instance.ArchetypeData.ShotgunBulletSpread,
					fireDir = (__instance.Owner.FPSCamera.CameraRayPos - position).normalized,
					owner = __instance.Owner
			};

				Weapon.s_weaponRayData = s_LocalRayData;

				if (Weapon.CastWeaponRay(__instance.MuzzleAlign, ref s_LocalRayData, position, -1))
				{
					s_LocalRayData.damage = __instance.ArchetypeData.GetDamageWithBoosterEffect(__instance.Owner, __instance.ItemDataBlock.inventorySlot);
					s_LocalRayData.staggerMulti = __instance.ArchetypeData.StaggerDamageMulti;
					s_LocalRayData.precisionMulti = __instance.ArchetypeData.PrecisionDamageMulti;
					s_LocalRayData.damageFalloff = __instance.ArchetypeData.DamageFalloff;
					Weapon.s_weaponRayData = s_LocalRayData;

					BulletWeapon.BulletHit(Weapon.s_weaponRayData, true, 0f, 0U);
					FX_Manager.EffectTargetPosition = Weapon.s_weaponRayData.rayHit.point;
				}
				else
				{
					FX_Manager.EffectTargetPosition = __instance.Owner.FPSCamera.CameraRayPos;
				}
				FX_Manager.PlayLocalVersion = false;
				BulletWeapon.s_tracerPool.AquireEffect().Play(null, __instance.MuzzleAlign.position, Quaternion.LookRotation(Weapon.s_weaponRayData.fireDir));
			}
			__instance.TriggerFireAnimationSequence();
			__instance.Owner.Noise = Agent.NoiseType.Shoot;
			__instance.ApplyRecoil(true);
			EX_SpriteMuzzleFlash muzzleFlash = __instance.m_muzzleFlash;
			if (muzzleFlash != null)
			{
				muzzleFlash.Play();
			}
			if (__instance.ShellCasingData != null)
			{
				//WeaponShellManager.EjectShell(__instance.ShellCasingData.ShellCasingType, __instance.ShellEjectAlign.position, __instance.ShellEjectAlign.rotation, __instance.ShellEjectAlign.forward);
			}
			__instance.FPItemHolder.DontRelax();
			__instance.Owner.Sync.RegisterFiredBullets(1);
			for (int j = 0; j < __instance.m_itemPartAnimators.Count; j++)
			{
				__instance.m_itemPartAnimators[j].CrossFadeInFixedTime("Fire", 0f, 0);
			}
			__instance.m_lastFireTime = Clock.Time;
			if (__instance.Owner.IsLocallyOwned)
			{
				PlayerAgent.LastLocalShotFiredTime = __instance.m_lastFireTime;
			}
			__instance.m_clip--;
			__instance.UpdateAmmoStatus();

            return false;
        }

		public static Weapon.WeaponHitData s_LocalRayData;
    }
}