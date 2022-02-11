using Gear;
using Player;

namespace Lockout_2_core.Custom_Weapon_code
{
    public class Manager_GrenadeLauncher
    {
        public static void Fire(BulletWeapon bulletWeapon)
        {
            var data = default(pItemData); data.itemID_gearCRC = 136;
            var normalized = (bulletWeapon.Owner.TargetLookDir).normalized;

            ItemReplicationManager.ThrowItem
            (
                data,
                null,
                ItemMode.Instance,
                bulletWeapon.MuzzleAlign.transform.position,
                bulletWeapon.MuzzleAlign.transform.rotation,
                normalized * 40f,
                bulletWeapon.transform.position,
                bulletWeapon.Owner.CourseNode,
                bulletWeapon.Owner
            );


            bulletWeapon.MaxRayDist = 0f;
        }
    }
}
