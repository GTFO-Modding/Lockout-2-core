using HarmonyLib;
using Player;
using SNetwork;

namespace Lockout_2_core
{
    class Patch_PlayerSync
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(PlayerSync);
            var patchType = typeof(Patch_PlayerSync);

            instance.Patch(gameType.GetMethod("WantsToWieldSlot"),  new HarmonyMethod(patchType, "WantsToWieldSlot"));
        }

        public static bool WantsToWieldSlot(InventorySlot slot)
        {
            if (PlayerBackpackManager.TryGetItem(SNet.LocalPlayer, slot, out var backpackItem) && backpackItem.ItemID == 5002) return false;
            return true;
        }
    }
}