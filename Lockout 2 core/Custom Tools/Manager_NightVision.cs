using FX_EffectSystem;
using Player;
using SNetwork;
using System;
using UnityEngine;
using UnityEngine.PostProcessing;
using AK;
using UnhollowerBaseLib;

namespace Lockout_2_core.Custom_Tools
{
    class Manager_NightVision : MonoBehaviour
    {
        public Manager_NightVision(IntPtr value) : base(value) { }

        void Awake()
        {
            Current = this;
            Patch_GS_AfterLevel.OnLevelCleanup += OnLevelCleanup;
        }

        void OnLevelCleanup()
        {
            L.Error("Cleaning up nightvision!");
            NightVisionActive = false;
            NightVisionEquipped = false;
            LocalPlayerBackpack = null;
            GearSpecialID = 0;
            NightVisionBatteryTick = 0;

            VFX_Default = null;
            VFX_NVG = null;

            try { Destroy(this); }
            catch (Exception error) { L.Error($"Shit was garbage collected, wack {error.ToString()}"); }
        }

        void Update()
        {
            if (!RundownManager.ExpeditionIsStarted)
            {
                RemoveNightVision();
                return;
            }

            if (RundownManager.ExpeditionIsStarted && LocalPlayerBackpack == null)
            {
                VFX_Default = new(
                    pointlight: LocalPlayerAgent.m_ambientPoint,
                    prelitVolumeColor: LocalPlayerAgent.FPSCamera.PrelitVolume.m_fogColor
                );

                VFX_NVG = new();
                AssignNightVision(5002);
            }
            else if (NightVisionEquipped) NightVisionUpdate();
        }


        void NightVisionUpdate()
        {
            if (Input.GetKeyDown(EntryPoint.NightVisionKey.Value) && FocusStateManager.CurrentState == eFocusState.FPS)
            {
                if (NightVisionActive) DeactivateNightVision();
                else if (LocalPlayerBackpack.AmmoStorage.ClassAmmo.AmmoInPack > 0) ActivateNightVision();
            }

            if (NightVisionActive && Time.time > NightVisionBatteryTick)
            {
                NightVisionBatteryTick = Time.time + 5f;
                LocalPlayerBackpack.AmmoStorage.ClassAmmo.AddAmmo(-2 / LocalPlayerBackpack.AmmoStorage.ClassAmmo.CostOfBullet);
                LocalPlayerBackpack.AmmoStorage.UpdateAllAmmoUI();

                NightVisionBatteryUI.UpdateSegments(LocalPlayerBackpack.AmmoStorage.ClassAmmo.AmmoInPack, LocalPlayerBackpack.AmmoStorage.ClassAmmo.AmmoMaxCap);

                if (LocalPlayerBackpack.AmmoStorage.ClassAmmo.AmmoInPack <= 0)
                {
                    LocalPlayerBackpack.AmmoStorage.ClassAmmo.AmmoInPack = 0f;
                    DeactivateNightVision();
                }
            }
        }

        void ActivateNightVision()
        {
            NightVisionActive = true;
            LocalPlayerAgent.Sound.Post(EVENTS.IMPLANTSMALLMELEECHARGEUP);
            AssignVFX(VFX_NVG);
            NightVisionBatteryUI.SetActive(true);
        }

        void DeactivateNightVision()
        {
            NightVisionActive = false;
            LocalPlayerAgent.Sound.Post(EVENTS.HUD_INFO_TEXT_DISAPPEAR);
            AssignVFX(VFX_Default);
            NightVisionBatteryUI.SetActive(false);
        }

        void AssignNightVision(uint pID)
        {
            if (PlayerBackpackManager.TryGetBackpack(SNet.LocalPlayer, out LocalPlayerBackpack))
            {
                GearSpecialID = LocalPlayerBackpack.Slots[(int)InventorySlot.GearClass].Instance.ItemDataBlock.persistentID;
                if (GearSpecialID != pID) return;

                NightVisionEquipped = true;
                NightVisionBatteryUI = new BatteryBar(5, GuiManager.Current.m_playerLayer.m_compass.transform);
                LocalPlayerAgent.Inventory.TryCast<PlayerInventoryLocal>().m_allowedScrollSlots.Remove(InventorySlot.GearClass);
            }
        }

        void RemoveNightVision()
        {
            if (LocalPlayerBackpack != null)
            {
                LocalPlayerBackpack = null;
                NightVisionEquipped = false;
            }
        }

        public void AssignVFX(VFX_Settings settings)
        {
            LocalPlayerAgent.m_ambientPoint.m_intensity = settings.m_Intensity;
            LocalPlayerAgent.m_ambientPoint.m_lightScale = settings.m_LightScale;
            LocalPlayerAgent.m_ambientPoint.m_invRangeSqr = settings.m_InverseScale;
            LocalPlayerAgent.m_ambienceLight.color = settings.m_Color;
            LocalPlayerAgent.m_ambientPoint.UpdateData();

            LocalPlayerAgent.FPSCamera.PrelitVolume.m_fogColor = settings.PrelitVolumeColor;

            //LocalPlayerAgent.FPSCamera.m_pBehavior.profile.grain.m_Settings = settings.Grain;
        }

        public PlayerAgent LocalPlayerAgent => PlayerManager.Current.m_localPlayerAgentInLevel;
        public PlayerBackpack LocalPlayerBackpack;
        public uint GearSpecialID;
        public bool NightVisionEquipped;
        public bool NightVisionActive;
        public float NightVisionBatteryTick;
        public BatteryBar NightVisionBatteryUI;
        public Il2CppStructArray<Color> NightVisionBatteryTextCol = new Color[] { Color.green };

        public static Manager_NightVision Current;

        public class VFX_Settings
        {
            public VFX_Settings() { }
            public VFX_Settings(FX_PointLight pointlight, Color prelitVolumeColor)
            {
                m_Intensity = pointlight.m_intensity;
                m_LightScale = pointlight.m_lightScale;
                m_InverseScale = pointlight.m_invRangeSqr;
                m_Color = new Color(pointlight.m_linearColor.x, pointlight.m_linearColor.y, pointlight.m_linearColor.z);

                PrelitVolumeColor = prelitVolumeColor;
            }
            public float m_Intensity = 0.35f;
            public Vector3 m_LightScale = new(400, 400, 400);
            public float m_InverseScale = 0.005f;
            public Color m_Color = new(0.0331f, 1f, 0.01f);

            public Color PrelitVolumeColor = new(0.2f, 1.01f, 0.1f, 1.0f);

            public GrainModel.Settings Grain = new()
            {
                colored = false,
                intensity = 1.5f,
                luminanceContribution = 1.0f,
                size = 0.75f
            };
        }

        public VFX_Settings VFX_Default;
        public VFX_Settings VFX_NVG;
    }
}
