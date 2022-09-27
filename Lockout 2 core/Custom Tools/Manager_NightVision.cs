using FX_EffectSystem;
using Player;
using SNetwork;
using System;
using UnityEngine;
using UnityEngine.PostProcessing;
using AK;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime;
using System.Linq;
using GTFO.API;

namespace Lockout_2_core.Custom_Tools
{
    class Manager_NightVision : MonoBehaviour
    {
        public Manager_NightVision(IntPtr value) : base(value) { }

        public void Awake()
        {
            Current = this;
            Patch_GS_AfterLevel.OnLevelCleanup += OnLevelCleanup;
            Patch_WardenObjectiveManager.OnStartExpedition += Setup;
        }

        public void OnLevelCleanup()
        {
            if (!m_IsSetup) return;

            L.Debug("Cleaning up nightvision");

            GameObject.Destroy(m_ThermalVision_CameraQuad);
            GameObject.Destroy(NightVisionBatteryUI.gameObject);

            m_IsSetup = false;
            NightVisionBatteryUI = null;
            m_LocalPlayerBackpack = null;
            m_NightVisionActive = false;
        }

        public void Update()
        {
            if (Input.GetKeyDown(EntryPoint.NightVisionKey.Value)
                && FocusStateManager.CurrentState == eFocusState.FPS
                && LocalPlayerBackpack.AmmoStorage.ClassAmmo.AmmoInPack > 0
                && HasNightVision) ToggleNightVision(!m_NightVisionActive);

            if (m_NightVisionActive && m_AmmoTick < Time.time) 
            {
                LocalPlayerBackpack.AmmoStorage.ClassAmmo.AddAmmo(-1);
                LocalPlayerBackpack.AmmoStorage.UpdateAllAmmoUI();
                NightVisionBatteryUI.UpdateSegments(LocalPlayerBackpack.AmmoStorage.ClassAmmo.AmmoInPack, LocalPlayerBackpack.AmmoStorage.ClassAmmo.AmmoMaxCap);
                m_AmmoTick = Time.time + 5;

                if (LocalPlayerBackpack.AmmoStorage.ClassAmmo.AmmoInPack <= 0) ToggleNightVision(true);
            }
        }

        public void Setup()
        {
            L.Debug($"m_IsSetup: {m_IsSetup} | HasNightVision {HasNightVision}");

            if (m_IsSetup || !HasNightVision) return;
            m_IsSetup = true;

            m_LocalPlayerBackpack.TryGetBackpackItem(InventorySlot.GearClass, out var nightVisionItem);
            foreach (var renderer in nightVisionItem.Instance.gameObject.GetComponentsInChildren<Renderer>()) renderer.enabled = false;

            NightVisionBatteryUI = new BatteryBar(5, GuiManager.Current.m_playerLayer.m_compass.transform);
            LocalPlayer.Inventory.TryCast<PlayerInventoryLocal>().m_allowedScrollSlots.Remove(InventorySlot.GearClass);
        }

        public void ToggleNightVision(bool status)
        {
            if (!m_IsSetup) Setup();
            if (m_ThermalVision_CameraQuad == null) BuildThermalVFX();

            m_ThermalVision_CameraQuad.active = status;
            m_NightVisionActive = status;
            NightVisionBatteryUI.SetActive(status);

            if (status) LocalPlayer.Sound.Post(EVENTS.IMPLANTSMALLMELEECHARGEUP);
            else LocalPlayer.Sound.Post(EVENTS.HUD_INFO_TEXT_DISAPPEAR);
        }

        

        public void BuildThermalVFX()
        {
            m_ThermalVision_CameraQuad = GameObject.Instantiate(AssetAPI.GetLoadedAsset("Assets/Lockout2/ThermalVisorQuad.prefab").TryCast<GameObject>(), m_FPSCamera.transform);
            m_ThermalVision_CameraQuad.active = false;
        }

        public static Manager_NightVision Current;

        public bool HasNightVision { get 
            {
                if (!PlayerBackpackManager.TryGetBackpack(SNet.LocalPlayer, out m_LocalPlayerBackpack)) return false;
                return m_LocalPlayerBackpack.Slots[(int)InventorySlot.GearClass].Instance.ItemDataBlock.persistentID == 5002;
            }
        }
        public PlayerBackpack LocalPlayerBackpack { get
            {
                if (m_LocalPlayerBackpack != null) return m_LocalPlayerBackpack;

                PlayerBackpackManager.TryGetBackpack(SNet.LocalPlayer, out m_LocalPlayerBackpack);
                return m_LocalPlayerBackpack;
            }
        }

        public PlayerAgent LocalPlayer { get => PlayerManager.GetLocalPlayerAgent(); }

        public PlayerBackpack m_LocalPlayerBackpack;
        public GameObject m_ThermalVision_CameraQuad;
        public FPSCamera m_FPSCamera;
        public float m_AmmoTick;
        public bool m_NightVisionActive;
        public bool m_IsSetup;

        public BatteryBar NightVisionBatteryUI;
        public Il2CppStructArray<Color> NightVisionBatteryTextCol = new Color[] { Color.yellow };
    }
}
