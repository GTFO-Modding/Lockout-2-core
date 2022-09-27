using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Lockout_2_core.Custom_Weapon_code;

namespace Lockout_2_core
{
    partial class Patch_Weapon
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(Weapon);
            var patchType = typeof(Patch_Weapon);

            instance.Patch(gameType.GetMethod("CastWeaponRay", new Type[] { typeof(Transform), typeof(Weapon.WeaponHitData).MakeByRefType(), typeof(Vector3), typeof(int) }), new HarmonyMethod(patchType, "CastWeaponRay"));
        }

        public static void CastWeaponRay(ref Weapon.WeaponHitData weaponRayData)
        {
            if (weaponRayData.owner == null) return;
            if (!InputMapper.GetButtonKeyMouse(InputAction.Aim, eFocusState.FPS)) return;

            var arch = weaponRayData.owner.Inventory.WieldedItem.ArchetypeData;
            if (arch == null) return;
            if (arch.persistentID != 1012 && arch.persistentID != 1005 && arch.persistentID != 2012 && arch.persistentID != 2005) return;

            Manager_WeaponAutoAim autoaim;
            if (!AutoAims.TryGetValue(arch.persistentID, out autoaim))
            {
                autoaim = weaponRayData.owner.Inventory.WieldedItem.GetComponent<Manager_WeaponAutoAim>();
                if (autoaim == null)
                {
                    L.Error("autoaim was null!");
                    return;
                }

                AutoAims.Add(arch.persistentID, autoaim);
            }

            if (autoaim.m_Target == null) return;

            weaponRayData.fireDir = (autoaim.m_Target.AimTarget.position - weaponRayData.owner.FPSCamera.Position).normalized;
        }

        public static Dictionary<uint,Manager_WeaponAutoAim> AutoAims = new();
    }
}