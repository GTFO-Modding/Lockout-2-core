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
    partial class Patch_CM_ScrollWindow
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(CM_ScrollWindow);
            var patchType = typeof(Patch_CM_ScrollWindow);

            instance.Patch(gameType.GetMethod("SelectHeader"), null, new HarmonyMethod(patchType, "SelectHeader"));
        }

        public static void SelectHeader(CM_ScrollWindow __instance, int key)
        {
            s_LastSelectedHeaderIndex = key;

            if (Patch_CM_PlayerLobbyBar.s_SwapButton == null) return;
            if (__instance.ContentItems[0].TryCast<CM_InventorySlotItem>() == null) return;

            for (var i = 0; i < __instance.ContentItems.Count; i++)
            {
                if (__instance.ContentItems[i].TryCast<CM_InventorySlotItem>().m_isSelected)
                {
                    Patch_CM_InventorySlotItem.s_LastSelectedItemIndex = i; break;
                }
            }

            InventorySlot slot;
            switch (key)
            {
                case 1: slot = InventorySlot.GearStandard; break;
                case 2: slot = InventorySlot.GearSpecial; break;
                default: slot = InventorySlot.None; break;
            }

            Patch_CM_PlayerLobbyBar.s_SwapButton.gameObject.SetActive(slot != InventorySlot.None);
        }

        public static int s_LastSelectedHeaderIndex;
    }
}