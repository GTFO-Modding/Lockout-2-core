using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Lockout_2_core.Custom_Weapon_code;
using Gear;
using Player;
using GameData;
using CellMenu;
using SNetwork;

namespace Lockout_2_core
{
    partial class Patch_GearManager
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(GearManager);
            var patchType = typeof(Patch_GearManager);

            instance.Patch(gameType.GetMethod("GetAllGearForSlot"), new HarmonyMethod(patchType, "GetAllGearForSlot"));
        }

        public static void GetAllGearForSlot(GearManager __instance, InventorySlot slot)
        {
            if (slot != InventorySlot.GearStandard && slot != InventorySlot.GearSpecial) return;

            foreach (var idRange in GearManager.Current.m_gearPerSlot[(int)slot].ToArray())
            {
                var gearOfflineID = uint.Parse(idRange.PlayfabItemInstanceId.Substring("OfflineGear_ID_".Length));

                if (!s_GearPerID.ContainsKey(gearOfflineID)) s_GearPerID.Add(gearOfflineID, idRange);

                if ((int)idRange.OfflineGearType != 2) continue;
                if (s_GearPair.ContainsValue(gearOfflineID)) continue;
                var baseGearID = gearOfflineID - 1000;

                s_GearPair.Add(baseGearID, gearOfflineID);
                GearManager.Current.m_gearPerSlot[(int)slot].Remove(idRange);
            }
        }

        public static void SwapGearVariant(InventorySlot slot, int index)
        {
            L.Debug($"SwapGearVariant({slot},{index})");
            var oldGearID = ParseGearOfflineID(GearManager.Current.m_gearPerSlot[(int)slot][index]); L.Debug(oldGearID);
            s_GearPair.TryGetValue(oldGearID, out var newGearID); L.Debug(newGearID);

            var lobbyBar = CM_PageLoadout.Current.m_playerLobbyBars[SNet.LocalPlayer.PlayerSlotIndex()];

            s_GearPerID[oldGearID].OfflineGearType = eOfflineGearType.RundownSpecificInventory;
            s_GearPerID[newGearID].OfflineGearType = eOfflineGearType.StandardInventory;
            GearManager.Current.m_gearPerSlot[(int)slot][index] = s_GearPerID[newGearID];

            lobbyBar.ShowWeaponSelectionPopup(slot, lobbyBar.m_inventorySlotItems[slot].transform);
            lobbyBar.OnWeaponSlotItemSelected(lobbyBar.m_popupScrollWindow.ContentItems[index].TryCast<CM_InventorySlotItem>());

            s_GearPair.Remove(oldGearID);
            s_GearPair.Add(newGearID, oldGearID);
        }

        public static uint ParseGearOfflineID(GearIDRange idRange)
        {
            return uint.Parse(idRange.PlayfabItemInstanceId.Substring("OfflineGear_ID_".Length));
        }

        public static Dictionary<uint, GearIDRange> s_GearPerID = new();
        public static Dictionary<uint, uint> s_GearPair = new();
    }
}