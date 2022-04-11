using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using Lockout_2_core.Custom_Weapon_code;
using Player;

namespace Lockout_2_core
{
    partial class Patch_ItemEquippable
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(ItemEquippable);
            var patchType = typeof(Patch_ItemEquippable);

            instance.Patch(gameType.GetMethod("GetItemFovZoom"), null, new HarmonyMethod(patchType, "FOVMod"));
            instance.Patch(gameType.GetMethod("GetWorldCameraZoomFov"), null, new HarmonyMethod(patchType, "FOVMod"));
        }

        public static void FOVMod(ref float __result)
        {
            if (s_CamRef == null)
            {
                var player = PlayerManager.GetLocalPlayerAgent();
                if (player == null) return;

                s_CamRef = player.FPSCamera;
                if (s_CamRef == null) return;
            }

            __result *= (s_CamRef.m_camera.fieldOfView / 120);
        }

        public static FPSCamera s_CamRef;
    }
}