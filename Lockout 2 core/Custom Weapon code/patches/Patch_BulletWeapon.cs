using HarmonyLib;
using UnityEngine;
using Gear;
using Player;
using Lockout_2_core.Custom_Weapon_code;

namespace Lockout_2_core
{
    partial class Patch_BulletWeapon
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(BulletWeapon);
            var patchType = typeof(Patch_BulletWeapon);

            instance.Patch(gameType.GetMethod("Update"), null, new HarmonyMethod(patchType, "Update"));
            instance.Patch(gameType.GetMethod("Fire"), null, new HarmonyMethod(patchType, "Fire"));
            instance.Patch(gameType.GetMethod("OnWield"), null, new HarmonyMethod(patchType, "OnWield"));
            instance.Patch(gameType.GetMethod("OnUnWield"), null, new HarmonyMethod(patchType, "OnUnWield"));
        }

        public static void Update(BulletWeapon __instance)
        {
            if (__instance.ArchetypeData.persistentID != 1005 && __instance.ArchetypeData.persistentID != 2005) return;          //Smart Machinegun: pull ammo from reserve
            __instance.Owner.Inventory.DoReload();
        }

        public static void Fire(BulletWeapon __instance)                        //Grenade launcher: fire grenade projectile
        {
            if (__instance.ArchetypeData.persistentID == 1011 || __instance.ArchetypeData.persistentID == 2011)
                Manager_GrenadeLauncher.Fire(__instance);

            if (__instance.ArchetypeData.persistentID == 1004 || __instance.ArchetypeData.persistentID == 2004)
                Manager_AntiMaterielRifle.Fire(__instance);
        }

        public static void OnWield(BulletWeapon __instance)                        //Smart Pistol: Assign manager to weapon
        {
            if (__instance.ArchetypeData.persistentID != 1012 && __instance.ArchetypeData.persistentID != 1005 && __instance.ArchetypeData.persistentID != 2012 && __instance.ArchetypeData.persistentID != 2005) return;
            if (CheckOwner(__instance)) return;
            var autoAim = __instance.gameObject.GetComponent<Manager_WeaponAutoAim>();

            if (autoAim == null) autoAim = __instance.gameObject.AddComponent<Manager_WeaponAutoAim>();
            
            if (autoAim.m_Owner == null)
            {
                autoAim.m_BulletWeapon = __instance;
                autoAim.m_Owner = __instance.Owner;
                autoAim.m_PlayerCamera = __instance.Owner.FPSCamera.gameObject.GetComponent<Camera>();
                autoAim.SetupReticle();
            }

            autoAim.enabled = true;
        }

        public static void OnUnWield(BulletWeapon __instance)
        {
            if (__instance.ArchetypeData.persistentID != 1012 && __instance.ArchetypeData.persistentID != 1005 && __instance.ArchetypeData.persistentID != 2012 && __instance.ArchetypeData.persistentID != 2005) return;
            if (CheckOwner(__instance)) return;
            var autoAim = __instance.gameObject.GetComponent<Manager_WeaponAutoAim>();
            if (autoAim == null) return;

            autoAim.m_Reticle.AnimateScale(0,0);
            autoAim.m_HasTarget = false;
            autoAim.m_Target = null;
            autoAim.enabled = false;
        }

        public static bool CheckOwner(BulletWeapon BW)
        {
            return BW.Owner != PlayerManager.Current.m_localPlayerAgentInLevel;
        }
    }
}