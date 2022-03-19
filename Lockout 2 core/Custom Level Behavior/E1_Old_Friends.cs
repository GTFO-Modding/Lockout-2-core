using AssetShards;
using GTFO.API;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using UnhollowerRuntimeLib;
using UnityEngine;
using GameData;
using Lockout_2_core.Custom_Enemies;
using Enemies;
using System.Linq;

namespace Lockout_2_core.Custom_Level_Behavior
{
    class E1_Old_Friends : MonoBehaviour
    {
        public E1_Old_Friends(IntPtr value) : base(value) { }
        public void Setup()
        {
            L.Error("E1 - Old Friends setup");
            m_IsSetup = true;

            Patch_GS_AfterLevel.OnLevelCleanup += OnCleanup;
            Patch_LG_PopulateFunctionMarkersInZoneJob.OnTriggerFunctionBuilder += OnTriggerFunctionBuilder;
            Patch_WardenObjectiveManager.OnStartExpedition += OnLevelStart;
            Patch_LG_WardenObjective_Reactor.OnReactorInteraction += OnReactorInteraction;
            Patch_WardenObjectiveManager.OnObjectiveComplete += OnObjectiveComplete;
            OnFactoryBuildStart();

            pResetHSU = new();
            pResetHSU.isSequenceIncomplete = true;
            pResetHSU.status = eHSUActivatorStatus.WaitingForInsert;

            EventsOnReactorGood.Clear();
            EventsOnReactorWarning.Clear();
            EventsOnReactorDanger.Clear();
            EventsOnReactorEmergency.Clear();

            EventsOnReactorGood.Add(new()
            {
                Type = eWardenObjectiveEventType.SetFogSetting,
                FogSetting = 127,
                FogTransitionDuration = 30,
                Delay = 0,
                DimensionIndex = eDimensionIndex.Reality,
                WardenIntel = new()
                {
                    Id = 0,
                    UntranslatedText = "<color=orange>Reactor Core Status: <u>Good</u></color>\nReactor is at safe operating capacity.\n\nDo not allow reactor core to exceed <color=orange><u>904.00°C</u></color> under any cirucmstances."
                }
            });
            EventsOnReactorWarning.Add(new()
            {
                Type = eWardenObjectiveEventType.SetFogSetting,
                FogSetting = 128,
                FogTransitionDuration = 30,
                Delay = 0,
                DimensionIndex = eDimensionIndex.Reality,
                WardenIntel = new()
                {
                    Id = 0,
                    UntranslatedText = "<color=orange>Reactor Core Status: <u>Warning</u></color>\nReactor exceeding 33% capacity.\n\nInsert coolant into <color=orange>HEAT_EXCHANGER_DC1.</color>"
                }
            });
            EventsOnReactorDanger.Add(new()
            {
                Type = eWardenObjectiveEventType.SetFogSetting,
                FogSetting = 129,
                FogTransitionDuration = 30,
                Delay = 0,
                DimensionIndex = eDimensionIndex.Reality,
                WardenIntel = new()
                {
                    Id = 0,
                    UntranslatedText = "<color=orange>Reactor Core Status: <u>Danger</u></color>\nReactor exceeding 66% capacity.\n\nInsert coolant <color=orange>immediately</color> to prevent a <u>Resonance Cascade</u>"
                }
            });
            EventsOnReactorEmergency.Add(new()
            {
                Type = eWardenObjectiveEventType.AllLightsOff,
                Delay = 0,
                DimensionIndex = eDimensionIndex.Reality,
                WardenIntel = new()
                {
                    Id = 0,
                    UntranslatedText = "<color=orange>Reactor Core Status: <u>Emergency</u></color>\nReactor has exceeded maxmimum operating capacity.\n\n<u>Resonance Cascade imminent</u>. Seek shelter immediately"
                }
            });
            EventsOnReactorEmergency.Add(new()
            {
                Type = eWardenObjectiveEventType.SetFogSetting,
                FogSetting = 130,
                FogTransitionDuration = 30,
                Delay = 0,
                DimensionIndex = eDimensionIndex.Reality,
            });
            EventsOnReactorEmergency.Add(new()
            {
                Type = eWardenObjectiveEventType.DimensionFlashTeam,
                Duration = 9,
                Delay = 5,
                DimensionIndex = eDimensionIndex.Dimension_1,
            });
            EventsOnReactorEmergency.Add(new()
            {
                SoundID = AK.EVENTS.DESERT_BOSS_ROAR_DISTANT,
                Type = eWardenObjectiveEventType.None,
                Delay = 6.6f,
                DimensionIndex = eDimensionIndex.Dimension_1,
            });
            EventsOnReactorEmergency.Add(new()
            {
                SoundID = AK.EVENTS.DISTANT_ROAR_TANK,
                Type = eWardenObjectiveEventType.None,
                Delay = 7.1f,
                DimensionIndex = eDimensionIndex.Dimension_1,
            });

            var gregWaveData = new GenericEnemyWaveData();
            gregWaveData.IntelMessage = 0;
            gregWaveData.SpawnDelay = 0;
            gregWaveData.TriggerAlarm = true;
            gregWaveData.WavePopulation = 39;
            gregWaveData.WaveSettings = 131;


            EventsOnReactorEmergency.Add(new()
            {
                SoundID = AK.EVENTS.KDS_DEEP_VENTILATION_PROCEDURE,
                Type = eWardenObjectiveEventType.SpawnEnemyWave,
                Delay = 14.5f,
                DimensionIndex = eDimensionIndex.Reality,
                EnemyWaveData = gregWaveData
            });
        }

        private void OnObjectiveComplete()
        {
            m_ReactorHeatRateTarget = -0.01f;
        }

        public void OnFactoryBuildStart()
        {
            AssetShardManager.LoadShardAsync(AssetBundleName.Complex_Tech, AssetBundleShard.S19, null, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        public void OnTriggerFunctionBuilder()
        {
            if (m_CoolantHSU != null) return;
            L.Debug("OnTriggerFunctionBuilder! Preparing to setup HSU device");

            var termAlign = LG_LevelBuilder.Current.m_currentFloor.allZones[2].m_areas[0].transform.FindChild("Terminal_Align");
            termAlign.localPosition = new(-0.36f, -0.5f, -3.52f);
            termAlign.localEulerAngles = new(0f, 0f, 0f);

            var scanAlign = LG_LevelBuilder.Current.m_currentFloor.allZones[2].m_areas[0].transform.FindChild("SecurityScan_Align");
            scanAlign.localPosition = new(4.8f, -0.5f, 0f);
            termAlign.localEulerAngles = new(0f, 0f, 0f);

            m_CoolantHSU_Holder = GameObject.Instantiate(AssetAPI.GetLoadedAsset("Assets/Bundle/reactorCoolantPlatform/content/E1_CoolantHSU.prefab").TryCast<GameObject>(), LG_LevelBuilder.Current.m_currentFloor.allZones[2].m_areas[0].transform);
            m_CoolantHSU_Holder.name = "CoolantHSU_Holder";
            m_CoolantHSU_Holder.transform.localPosition = new(-2.35f, -5.5f, 0);
            m_CoolantHSU_Holder.transform.localEulerAngles = new(0, 90, 0);

            var waterPool = m_CoolantHSU_Holder.transform.FindChild("WaterPlane");
            var allMat = GameObject.FindObjectsOfTypeAll(UnhollowerRuntimeLib.Il2CppType.From(typeof(Material))).Select((x) => x.Cast<Material>()).ToArray();
            foreach (var mat in allMat)
            {
                if (!mat.name.Contains("service_water_plane_2x2")) continue;
                waterPool.GetComponent<Renderer>().sharedMaterial = mat;
                L.Debug("Applied water material to the plane!");
                break;
            }

            var existingObjective = WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).Type;
            WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).Type = eWardenObjectiveType.ActivateSmallHSU;
            L.Debug($"Set Objective Type to {eWardenObjectiveType.ActivateSmallHSU}");

            m_HSU_Cores = GameObject.FindObjectsOfTypeAll(UnhollowerRuntimeLib.Il2CppType.From(typeof(LG_HSUActivator_Core))).Select((x) => x.Cast<LG_HSUActivator_Core>()).ToArray();
            foreach (var instance in m_HSU_Cores)
            {
                if (instance.gameObject.name == "HSU_Activator_machine" && m_CoolantHSU == null)
                {
                    m_CoolantHSU = GameObject.Instantiate(instance, m_CoolantHSU_Holder.transform.FindChild("a_HSU"));

                    m_CoolantHSU.m_publicName = "HEAT_EXCHANGER_DC1";
                    m_CoolantHSU.SpawnNode = LG_LevelBuilder.Current.m_currentFloor.allZones[2].m_areas[0].m_courseNode;
                    m_CoolantHSU.Setup();

                    m_CoolantHSU.name = "E1_CoolantHSU";
                    m_CoolantHSU.m_terminalItem.SpawnNode = LG_LevelBuilder.Current.m_currentFloor.allZones[2].m_areas[0].m_courseNode;
                    m_CoolantHSU.transform.localPosition = new Vector3(0, 0, 0);
                    m_CoolantHSU.transform.localScale = new Vector3(1, 1, 1);
                    m_CoolantHSU.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

                    m_CoolantHSU.m_itemComingOutAlign.localScale = Vector3.zero;

                    m_CoolantHSU.m_serialNumber = 285;
                    m_CoolantHSU.m_sound = new();

                    L.Debug($"Custom LG_HSUActivator_Core {instance.name} setup!");
                }
            }

            WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).Type = existingObjective;
            L.Debug($"Set Objective Type to {existingObjective}");
            m_CoolantHSU.add_OnHSUExitSequence((Action<LG_HSUActivator_Core>)SyncOnHSUExitSequence);

            L.Debug("HSU device done!");
        }

        public void SyncOnHSUExitSequence(LG_HSUActivator_Core core)
        {
            NetworkAPI.InvokeEvent("E1SyncHSUState", 0);
            OnHSUExitSequence(0, 0);
        }

        public static void OnHSUExitSequence(ulong x, byte y)
        {
            Manager_CustomLevelBehavior.E1.m_CoolantHSU.SyncStatusChanged(ref Manager_CustomLevelBehavior.E1.pResetHSU, false);
            Manager_CustomLevelBehavior.E1.m_CoolantHSU.m_stateReplicator.SetStateUnsynced(Manager_CustomLevelBehavior.E1.pResetHSU);
            Manager_CustomLevelBehavior.E1.m_DoCoolReactor = true;

            if (Manager_CustomLevelBehavior.E1.m_ReactorCoolTimer > Time.time)
                Manager_CustomLevelBehavior.E1.m_ReactorCoolTimer += 20;
            else
                Manager_CustomLevelBehavior.E1.m_ReactorCoolTimer = Time.time + 20;
        }

        public static void ClientRequestInfo(ulong x, byte y)
        {
            if (!SNet.IsMaster) return;
            NetworkAPI.InvokeEvent("E1ProvideObjectiveInfo", new pObjectiveData() {
                DoCoolReactor = Manager_CustomLevelBehavior.E1.m_DoCoolReactor,
                DoProcessReactorStatus = Manager_CustomLevelBehavior.E1.m_DoProcessReactorStatus,
                ReactorCoolTimer = Manager_CustomLevelBehavior.E1.m_ReactorCoolTimer,
                ReactorHeatRate = Manager_CustomLevelBehavior.E1.m_ReactorHeatRate,
                ReactorHeatRateTarget = Manager_CustomLevelBehavior.E1.m_ReactorHeatRateTarget,
                ReactorState = Manager_CustomLevelBehavior.E1.m_ReactorState,
                ReactorStatus = Manager_CustomLevelBehavior.E1.m_ReactorStatus,
                ReactorStatusColor = Manager_CustomLevelBehavior.E1.m_ReactorStatusColor,
                ReactorTemperature = Manager_CustomLevelBehavior.E1.m_ReactorTemperature,
            });
        }

        public static void RecieveObjectiveInfo(ulong x, pObjectiveData data)
        {
            Manager_CustomLevelBehavior.E1.m_DoCoolReactor = data.DoCoolReactor;
            Manager_CustomLevelBehavior.E1.m_DoProcessReactorStatus = data.DoProcessReactorStatus;
            Manager_CustomLevelBehavior.E1.m_ReactorCoolTimer = data.ReactorCoolTimer;
            Manager_CustomLevelBehavior.E1.m_ReactorHeatRate = data.ReactorHeatRate;
            Manager_CustomLevelBehavior.E1.m_ReactorHeatRateTarget = data.ReactorHeatRateTarget;
            Manager_CustomLevelBehavior.E1.m_ReactorState = data.ReactorState;
            Manager_CustomLevelBehavior.E1.m_ReactorStatus = data.ReactorStatus;
            Manager_CustomLevelBehavior.E1.m_ReactorStatusColor = data.ReactorStatusColor;
            Manager_CustomLevelBehavior.E1.m_ReactorTemperature = data.ReactorTemperature;
        }

        public static void SyncTemp(ulong x, float temp)
        {
            Manager_CustomLevelBehavior.E1.m_ReactorTemperature = temp;
        }

        public void OnReactorInteraction(eReactorInteraction type)
        {
            m_ReactorState = type;

            switch (m_ReactorState)
            {
                case eReactorInteraction.Initiate_startup: m_ReactorHeatRateTarget = 2.5f; break;
                case eReactorInteraction.WaitForVerify_startup: m_ReactorHeatRateTarget = 0.2f; break;
                case eReactorInteraction.Intensify_startup: m_ReactorHeatRateTarget = 4f; break;
                case eReactorInteraction.Verify_startup: m_ReactorHeatRateTarget = 2.5f; break;
                case eReactorInteraction.Verify_fail: m_ReactorHeatRateTarget = 2.5f; break;
                case eReactorInteraction.Goto_active: m_ReactorHeatRateTarget = 1.0f; break;
                case eReactorInteraction.Goto_inactive: m_ReactorHeatRateTarget = 0.5f; break;
            }

            if (type == eReactorInteraction.Initiate_startup)
            {
                GuiManager.PlayerLayer.m_objectiveTimer.gameObject.active = true;
                m_DoProcessReactorStatus = true;

                m_ReactorStatus = eReactorCoreStatus.Cold;
                m_ReactorTemperature = 26;
                m_ReactorStatusColor = Color.gray;
            }
        }

        public void OnLevelStart()
        {
            m_ReactorStatusSound = new();
            m_CoolantHSU.PostCullingSetup();
            m_CoolantHSU.m_insertHSUInteraction.SetActive(true);

            Material blob = null;
            var allMat = GameObject.FindObjectsOfTypeAll(UnhollowerRuntimeLib.Il2CppType.From(typeof(Material))).Select((x) => x.Cast<Material>()).ToArray();
            foreach (var mat in allMat)
            {
                if (!mat.name.Contains("BlobSurface")) continue;
                blob = mat;
                break;
            }

            var world = GameObject.Find("Root_Dimension_1");
            foreach (var renderer in world.GetComponentsInChildren<Renderer>())
            {
                if (!renderer.gameObject.name.Contains("g_ground") || renderer.gameObject.name.Contains("ball") || renderer.gameObject.name.Contains("dunes")) continue;
                renderer.sharedMaterial = blob;
            }

            GameObject go;
            var room = LG_LevelBuilder.Current.m_currentFloor.allZones[10].m_areas[2].transform.FindChild("Markers/Marker").GetChild(0);
            for(var i = 0; i < room.transform.childCount; i++)
            {
                go = room.GetChild(i).gameObject;
                if (go.layer == LayerMask.NameToLayer("InvisibleWall")) go.active = false;
            }
            
            GuiManager.PlayerLayer.m_objectiveTimer.m_titleText.text = "<u>Reactor Core Status</u>";

            if (!SNet.IsMaster) NetworkAPI.InvokeEvent("E1ClientRequestInfo", 0);
        }

        public void Update()
        {
            if (!m_IsSetup) return;

            if (m_DoProcessReactorStatus)
            {
                if (SNet.IsMaster && m_ReactorUpdateTimer < Time.time)
                {
                    NetworkAPI.InvokeEvent("E1SyncTemp", m_ReactorTemperature);
                    m_ReactorUpdateTimer += 60;
                }

                m_ReactorTemperature += m_ReactorHeatRate * Time.deltaTime;

                if (m_ReactorTemperature < 94)
                {
                    if (m_ReactorStatus != eReactorCoreStatus.Cold)
                    {
                        m_ReactorStatusColor = Color.gray;
                        m_ReactorStatus = eReactorCoreStatus.Cold;
                    }
                }
                else if (m_ReactorTemperature < 486)
                {
                    if (m_ReactorStatus != eReactorCoreStatus.Good)
                    {
                        WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(EventsOnReactorGood, eWardenObjectiveEventTrigger.OnStart, true);
                        m_ReactorStatusColor = Color.white;
                        m_ReactorStatusSound.Stop();
                        m_ReactorStatus = eReactorCoreStatus.Good;
                    }
                }
                else if (m_ReactorTemperature < 695)
                {
                    if (m_ReactorStatus != eReactorCoreStatus.Warning)
                    {
                        WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(EventsOnReactorWarning, eWardenObjectiveEventTrigger.OnStart, true);
                        m_ReactorStatusColor = new(1, 0.9f, 0, 1);
                        m_ReactorStatusSound.Stop();
                        m_ReactorStatusSound.Post(AK.EVENTS.ALARM_AMBIENT_LOOP, new Vector3(0, -30, 256));
                        m_ReactorStatusSound.Post(AK.EVENTS.DECON_UNIT_MISSING_ALARM, new Vector3(0, -30, 256));
                        m_ReactorStatus = eReactorCoreStatus.Warning;
                    }
                }
                else if (m_ReactorTemperature < 904)
                {
                    if (m_ReactorStatus != eReactorCoreStatus.Danger)
                    {
                        WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(EventsOnReactorDanger, eWardenObjectiveEventTrigger.OnStart, true);
                        m_ReactorStatusColor = new(1, 0.3f, 0, 1);
                        m_ReactorStatusSound.Stop();
                        m_ReactorStatusSound.Post(AK.EVENTS.ALARM_AMBIENT_LOOP, new Vector3(0, -30, 256));
                        m_ReactorStatusSound.Post(AK.EVENTS.DECON_UNIT_MISSING_ALARM, new Vector3(0, -30, 256));
                        m_ReactorStatus = eReactorCoreStatus.Danger;
                    }
                }
                else if (m_ReactorStatus != eReactorCoreStatus.Emergency)
                {
                    WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(EventsOnReactorEmergency, eWardenObjectiveEventTrigger.OnStart, true);
                    m_ReactorStatusColor = new(1, 0, 0, 1);
                    m_ReactorStatusSound.Stop();
                    m_ReactorStatusSound.Post(AK.EVENTS.DISTANT_EXPLOSION_SEQUENCE, new Vector3(0, -30, 256));
                    m_ReactorStatusSound.Post(AK.EVENTS.ALARM_AMBIENT_LOOP, new Vector3(0, -30, 256));
                    m_ReactorStatusSound.Post(AK.EVENTS.REACTOR_POWER_LEVEL_3_TO_2_TRANSITION, new Vector3(0, -30, 256));
                    m_ReactorStatus = eReactorCoreStatus.Emergency;
                }

                if (m_DoCoolReactor)
                {
                    if (Time.time < m_ReactorCoolTimer)
                        m_ReactorHeatRate = Mathf.Lerp(m_ReactorHeatRate, -10, 0.03f);

                    else
                        m_DoCoolReactor = false;
                }
                else
                    m_ReactorHeatRate = Mathf.Lerp(m_ReactorHeatRate, m_ReactorHeatRateTarget, 0.01f);

                GuiManager.PlayerLayer.m_objectiveTimer.m_timerText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(m_ReactorStatusColor)}>{m_ReactorTemperature:0.00}°C //: {m_ReactorStatus}";
            }
        }

        public void OnCleanup() 
        {
            L.Error("E1 - Old Friends cleanup!");
            m_IsSetup = false;

            if (m_CoolantHSU != null) GameObject.Destroy(m_CoolantHSU);

            m_ReactorStatusSound = null;
            m_DoProcessReactorStatus = false;
            m_ReactorStatus = eReactorCoreStatus.Good;
            m_ReactorState = eReactorInteraction.Initiate_startup;
            m_ReactorTemperature = 0;
            m_ReactorHeatRateTarget = 0;
            m_ReactorStatusColor = Color.white;
            m_ReactorUpdateTimer = 0;

            EventsOnReactorGood.Clear();
            EventsOnReactorWarning.Clear();
            EventsOnReactorDanger.Clear();
            EventsOnReactorEmergency.Clear();

            Patch_GS_AfterLevel.OnLevelCleanup -= OnCleanup;
            Patch_LG_PopulateFunctionMarkersInZoneJob.OnTriggerFunctionBuilder -= OnTriggerFunctionBuilder;
            Patch_WardenObjectiveManager.OnStartExpedition -= OnLevelStart;
            Patch_LG_WardenObjective_Reactor.OnReactorInteraction -= OnReactorInteraction;
        }

        public bool m_IsSetup = false;
        public GameObject m_CoolantHSU_Holder;
        public LG_HSUActivator_Core m_CoolantHSU;
        public LG_HSUActivator_Core[] m_HSU_Cores;

        public CellSoundPlayer m_ReactorStatusSound;
        public bool m_DoProcessReactorStatus;
        public bool m_DoCoolReactor;
        public float m_ReactorCoolTimer;
        public eReactorCoreStatus m_ReactorStatus;
        public eReactorInteraction m_ReactorState;
        public float m_ReactorTemperature;

        public float m_ReactorHeatRateTarget;
        public float m_ReactorHeatRate;
        
        public Color m_ReactorStatusColor;

        public float m_ReactorUpdateTimer;

        public pHSUActivatorState pResetHSU;

        public Il2CppSystem.Collections.Generic.List<WardenObjectiveEventData> EventsOnReactorGood = new();
        public Il2CppSystem.Collections.Generic.List<WardenObjectiveEventData> EventsOnReactorWarning = new();
        public Il2CppSystem.Collections.Generic.List<WardenObjectiveEventData> EventsOnReactorDanger = new();
        public Il2CppSystem.Collections.Generic.List<WardenObjectiveEventData> EventsOnReactorEmergency = new();

        public enum eReactorCoreStatus
        {
            Cold,
            Good,
            Warning,
            Danger,
            Emergency
        }

        public struct pObjectiveData
        {
            public bool DoProcessReactorStatus;
            public bool DoCoolReactor;
            public float ReactorCoolTimer;
            public eReactorCoreStatus ReactorStatus;
            public eReactorInteraction ReactorState;
            public float ReactorTemperature;
            public float ReactorHeatRateTarget;
            public float ReactorHeatRate;
            public Color ReactorStatusColor;
        }
    }
}
