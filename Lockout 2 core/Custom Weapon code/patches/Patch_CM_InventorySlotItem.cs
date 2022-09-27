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
    partial class Patch_CM_InventorySlotItem
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(CM_InventorySlotItem);
            var patchType = typeof(Patch_CM_InventorySlotItem);

            instance.Patch(gameType.GetMethod("OnBtnPress"), null, new HarmonyMethod(patchType, "OnBtnPress"));
        }

        public static void OnBtnPress(CM_InventorySlotItem __instance)
        {
            s_LastSelectedItemIndex = 0;

            for (var i = 0; i < __instance.m_parentBar.m_popupScrollWindow.ContentItems.Count; i++)
            {
                s_LastSelectedItemIndex = i;
                if (__instance.m_parentBar.m_popupScrollWindow.ContentItems[i].TryCast<CM_InventorySlotItem>().m_isSelected) break;
            }
        }

        public static int s_LastSelectedItemIndex;
    }
}