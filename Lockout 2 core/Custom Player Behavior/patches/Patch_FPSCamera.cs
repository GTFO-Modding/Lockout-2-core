using HarmonyLib;
using Lockout_2_core.Custom_Player_Behavior;
using Player;
using SNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace Lockout_2_core
{
    class Patch_FPSCamera_3RDPersonTest
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(FPSCamera);
            var patchType = typeof(Patch_FPSCamera_3RDPersonTest);

            instance.Patch(gameType.GetMethod("UpdateLocalPos"), null, new HarmonyMethod(patchType, "UpdateLocalPos"));
        }

        public static void UpdateLocalPos(FPSCamera __instance)
        {
            if (!s_IsThirdPerson) return;
            if (s_Target == null) GetNextTarget();

            if (InputMapper.GetButtonDownKeyMouse(InputAction.Fire, eFocusState.FPS)) GetNextTarget();

            LocalPlayer.transform.localPosition = Vector3.zero;
            LocalPlayer.Locomotion.VerticalVelocity = Vector3.zero;
            LocalPlayer.Locomotion.VerticalAcceleration = Vector3.zero;

            __instance.m_camera.transform.localPosition += new Vector3(0, 0, -s_CamOrbitDist);
        }

        public static void GetNextTarget()
        {
            L.Debug("GetNextTarget()");

            CurrentSpectatedSlot = (CurrentSpectatedSlot + 1) % PlayerCount;

            s_Target = TargetPlayer.GetComponentInChildren<ThirdPersonCamTarget>();
            if (TargetPlayer != LocalPlayer)
            {
                LocalPlayer.transform.parent = s_Target.transform;
                LocalPlayer.transform.localPosition = Vector3.zero;
            }

            LocalFPSCam.transform.parent.parent.parent = s_Target.transform;
            LocalFPSCam.transform.parent.parent.localPosition = Vector3.zero;
            LocalFPSCam.transform.parent.parent.localEulerAngles = Vector3.zero;

            LocalFPSCam.m_holder.m_originalParent = s_Target.transform;
            LocalFPSCam.m_holder.ReconnectToPlayer();
        }

        public static void UnTarget()
        {
            L.Debug("GetNextTarget()");

            CurrentSpectatedSlot = 0;
            s_Target = null;
            LocalFPSCam.transform.parent.parent.parent = null;
            LocalPlayer.transform.parent = null;
        }

        public static void ThirdPersonCamSetup()
        {
            L.Debug("ThirdPersonCamSetup()");
            Patch_GS_AfterLevel.OnLevelCleanup += OnLevelCleanup;

            LocalPlayer.FPItemHolder.gameObject.SetActive(false);
            LocalPlayer.Inventory.m_flashlight.gameObject.SetActive(false);

            s_Memory_InventoryPos = GuiManager.PlayerLayer.Inventory.transform.localPosition;
            s_Memory_PlayerStatusPos = GuiManager.PlayerLayer.m_playerStatus.transform.localPosition;
            s_Memory_InteractionPos = GuiManager.InteractionLayer.CanvasTrans.localPosition;

            GuiManager.PlayerLayer.Inventory.transform.localPosition = Vector3.one * -9999;
            GuiManager.PlayerLayer.m_playerStatus.transform.localPosition = Vector3.one * -9999;
            GuiManager.InteractionLayer.CanvasTrans.localPosition = Vector3.one * -9999;
            GuiManager.CrosshairLayer.m_circleCrosshair.gameObject.SetActive(false);

            LocalFPSCam.GlassLiquids.enabled = false;
            LocalFPSCam.HUDGlassShatter.enabled = false;
            LocalFPSCam.m_fpsDamageFeedback.enabled = false;

            s_IsThirdPerson = true;
            CurrentSpectatedSlot = (LocalPlayer.PlayerSlotIndex + 1) % PlayerCount;
        }

        public static void OnLevelCleanup()
        {
            s_CurrentSpectatedSlot = 0;
            s_IsThirdPerson = false;
            s_DeadPlayerSlots.Clear();

            GuiManager.PlayerLayer.Inventory.transform.localPosition = s_Memory_InventoryPos;
            GuiManager.PlayerLayer.m_playerStatus.transform.localPosition = s_Memory_PlayerStatusPos;
            GuiManager.InteractionLayer.CanvasTrans.localPosition = s_Memory_InteractionPos;

            GuiManager.CrosshairLayer.m_circleCrosshair.gameObject.SetActive(true);

            PlayerDeathManager.Current.m_DeadPlayers.Clear();
            PlayerDeathManager.Current.m_DeadPlayerIDs.Clear();

            Patch_GS_AfterLevel.OnLevelCleanup -= OnLevelCleanup;
        }

        public static PlayerAgent LocalPlayer => PlayerManager.GetLocalPlayerAgent();
        public static FPSCamera LocalFPSCam => LocalPlayer.FPSCamera;
        public static int PlayerCount => PlayerManager.PlayerAgentsInLevel.Count;
        public static PlayerAgent TargetPlayer
        {
            get
            {
                if (s_DeadPlayerSlots.Contains(CurrentSpectatedSlot)) return null;
                else return PlayerManager.Current.GetPlayerAgentInSlot(CurrentSpectatedSlot);
            }
        }

        public static int CurrentSpectatedSlot { get => s_CurrentSpectatedSlot; 
            set 
            {
                s_CurrentSpectatedSlot = value;
                s_TargetPlayerCache = TargetPlayer;
            } 
        }
        public static int s_CurrentSpectatedSlot;

        public static PlayerAgent s_TargetPlayerCache;

        public static bool s_IsThirdPerson;
        public static List<int> s_DeadPlayerSlots = new();
        public static ThirdPersonCamTarget s_Target;

        public static float s_CamOrbitDist = 2.5f;
        public static float s_CamOrbitMin = 1f;
        public static float s_CamOrbitMax = 3.75f;

        public static Vector3 s_Memory_InventoryPos;
        public static Vector3 s_Memory_PlayerStatusPos;
        public static Vector3 s_Memory_InteractionPos;
    }
}