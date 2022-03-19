using AssetShards;
using GTFO.API;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using UnhollowerRuntimeLib;
using UnityEngine;
using System.Linq;
using GameData;
using Localization;
using AIGraph;
using Lockout_2_core.Custom_Enemies;
using Enemies;

namespace Lockout_2_core.Custom_Level_Behavior
{
    class D1_Arboretum
    {
        public  void Setup()
        {
            L.Error("D1 - Arboretum setup");
            m_IsSetup = true;

            Patch_GS_AfterLevel.OnLevelCleanup += OnCleanup;
            Patch_LG_PopulateFunctionMarkersInZoneJob.OnTriggerFunctionBuilder += OnTriggerFunctionBuilder;
            Patch_WardenObjectiveManager.OnStartExpedition += OnLevelStart;
            Manager_VenusWeed_Rose.OnDead += OnBossDead;
            OnFactoryBuildStart();
        }

        public void OnFactoryBuildStart()
        {
        }

        public void OnTriggerFunctionBuilder()
        {
            if (m_GenCluster != null) return;
            L.Debug("OnTriggerFunctionBuilder! Preparing to setup generator cluster");

            m_GenCluster = GameObject.Instantiate(AssetAPI.GetLoadedAsset("Assets/Bundle/ServiceGenCluster/Content/Service_GenCluster.prefab").TryCast<GameObject>());

            m_GenCluster.transform.parent = LG_LevelBuilder.Current.m_currentFloor.allZones[1].m_areas[1].transform;
            var genComp = m_GenCluster.AddComponent<LG_PowerGeneratorCluster>();
            m_GenCluster.name = "D1_CustomGenCluster";

            var genAlignHolder = m_GenCluster.transform.FindChild("Cluster/Generators");
            genComp.m_generatorAligns = new(3);
            genComp.m_generatorAligns[0] = genAlignHolder.FindChild("A_Gen1");
            genComp.m_generatorAligns[1] = genAlignHolder.FindChild("A_Gen2");
            genComp.m_generatorAligns[2] = genAlignHolder.FindChild("A_Gen3");

            foreach (var obj in Resources.FindObjectsOfTypeAll(Il2CppType.Of<LG_PowerGenerator_Core>()))
            {
                if (obj.name != "Generator_Slot_a") continue;
                genComp.m_generatorPrefab = obj.TryCast<LG_PowerGenerator_Core>().gameObject;
                break;
            }

            genComp.PublicName = "Service_GenCluster";
            genComp.SpawnNode = LG_LevelBuilder.Current.m_currentFloor.allZones[1].m_areas[1].m_courseNode;
            genComp.m_chainedPuzzleAlignMidObjective = m_GenCluster.transform.FindChild("Base/a_Bioscan");

            m_GenCluster.transform.position = new(14.4f, 2.5f, 207f);
            m_GenCluster.transform.localEulerAngles = new(0f, 225f, 0f);

            var terminalItemComp = m_GenCluster.AddComponent<LG_GenericTerminalItem>();
            terminalItemComp.SpawnNode = genComp.SpawnNode;

            genComp.m_terminalItemComp = terminalItemComp;

            genComp.Setup();
            L.Debug("Generator cluster setup!");
        }

        public void OnLevelStart()
        {
        }

        public void OnBossDead()
        {
            var eventData = new Il2CppSystem.Collections.Generic.List<WardenObjectiveEventData>();
            eventData.Add(new()
            {
                DimensionIndex = eDimensionIndex.Reality,
                Type = eWardenObjectiveEventType.SetFogSetting,
                Delay = 0,
                FogSetting = 120,
                FogTransitionDuration = 30,
                SoundID = 3429102651,
                Trigger = eWardenObjectiveEventTrigger.OnStart
            });
            eventData.Add(new()
            {
                Type = eWardenObjectiveEventType.UnlockSecurityDoor,
                Delay = 5,
                LocalIndex = eLocalZoneIndex.Zone_4,
                DimensionIndex = eDimensionIndex.Reality,
                WardenIntel = new()
                {
                    Id = 0,
                    UntranslatedText = "Air contamination levels stabilizing. Proceed to the extraction point."
                }
            });

            WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(eventData, eWardenObjectiveEventTrigger.OnStart, true);
        }

        public void FogTransitionOnBossAlert()
        {
            var eventData = new Il2CppSystem.Collections.Generic.List<WardenObjectiveEventData>();
            eventData.Add(new()
            {
                DimensionIndex = eDimensionIndex.Reality,
                Type = eWardenObjectiveEventType.SetFogSetting,
                Delay = 3,
                FogSetting = 121,
                FogTransitionDuration = 0.25f,
                SoundID = 2591647810,
                Trigger = eWardenObjectiveEventTrigger.OnStart
            });
            eventData.Add(new()
            {
                DimensionIndex = eDimensionIndex.Reality,
                Type = eWardenObjectiveEventType.SetFogSetting,
                Delay = 3.25f,
                FogSetting = 122,
                FogTransitionDuration = 1.5f,
                SoundID = 4065651585,
                Trigger = eWardenObjectiveEventTrigger.OnStart
            });
            eventData.Add(new()
            {
                DimensionIndex = eDimensionIndex.Reality,
                Type = eWardenObjectiveEventType.SetFogSetting,
                Delay = 5,
                FogSetting = 123,
                FogTransitionDuration = 5f,
                Trigger = eWardenObjectiveEventTrigger.OnStart
            });
            eventData.Add(new()
            {
                DimensionIndex = eDimensionIndex.Reality,
                Type = eWardenObjectiveEventType.SetFogSetting,
                Delay = 10,
                FogSetting = 124,
                FogTransitionDuration = 3f,
                Trigger = eWardenObjectiveEventTrigger.OnStart
            });

            WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(eventData, eWardenObjectiveEventTrigger.OnStart, true);
        }
        public void OnCleanup() 
        {
            L.Error("D1 - Arboretum cleanup!");
            m_IsSetup = false;

            if (m_GenCluster != null) GameObject.Destroy(m_GenCluster);
            if (m_Boss != null) GameObject.Destroy(m_Boss.transform.parent.gameObject);

            Patch_GS_AfterLevel.OnLevelCleanup -= OnCleanup;
            Patch_LG_PopulateFunctionMarkersInZoneJob.OnTriggerFunctionBuilder -= OnTriggerFunctionBuilder;
            Patch_WardenObjectiveManager.OnStartExpedition -= OnLevelStart;
            Manager_VenusWeed_Rose.OnDead -= OnBossDead;
        }

        public bool m_IsSetup = false;
        public GameObject m_GenCluster;
        public Manager_VenusWeed_Rose Boss { set
            {
                if (!m_IsSetup) return;

                m_Boss = value;
                m_BossEnemy = value.transform.parent.parent.parent.GetComponent<EnemyAgent>();

                m_BossEnemy.transform.position = new(-128.136f, 2.3189f, 249.8893f);
                m_BossEnemy.transform.localEulerAngles = new(0, 180, 0);

                value.Setup();
            }}

        public Manager_VenusWeed_Rose m_Boss;
        public EnemyAgent m_BossEnemy;
    }
}
