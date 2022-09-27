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
    partial class Patch_CM_PlayerLobbyBar
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(CM_PlayerLobbyBar);
            var patchType = typeof(Patch_CM_PlayerLobbyBar);

            instance.Patch(gameType.GetMethod("ShowWeaponSelectionPopup"), null, new HarmonyMethod(patchType, "ShowWeaponSelectionPopup"));
        }

        public static void ShowWeaponSelectionPopup(CM_PlayerLobbyBar __instance, InventorySlot slot)
        {
            if (slot != InventorySlot.GearStandard && slot != InventorySlot.GearSpecial) return;

            CM_ScrollWindowInfoBox infoBox;

            L.Debug("Adding swap button");
            infoBox = __instance.m_popupScrollWindow.InfoBox;
            s_SwapButton = GameObject.Instantiate(CM_PageLoadout.Current.m_copyLobbyIdButton.gameObject, infoBox.transform).GetComponent<CM_Item>();
            s_SwapButton.m_texts[0].SetText("Switch Weapon Variant");
            s_SwapButton.transform.localPosition = new(-300, -320, -1);

            s_SwapButton.OnBtnPressCallback = null;
            s_SwapButton.add_OnBtnPressCallback((Action<int>)((id) =>
            {
                L.Debug("SwapButton Pressed");
                InventorySlot slot;
                switch(Patch_CM_ScrollWindow.s_LastSelectedHeaderIndex)
                {
                    case 1: slot = InventorySlot.GearStandard; break;
                    case 2: slot = InventorySlot.GearSpecial; break;
                    default: slot = InventorySlot.None; break;
                }

                L.Debug(slot);

                if (slot == InventorySlot.None) return;
                for (var i = 0; i < __instance.m_popupScrollWindow.ContentItems.Count; i++)
                {
                    if (i != Patch_CM_InventorySlotItem.s_LastSelectedItemIndex) continue;
                    Patch_GearManager.SwapGearVariant(slot, i);
                    break;
                }


            }));
        }

        public static CM_Item s_SwapButton;
    }
}