using AIGraph;
using Enemies;
using GameData;
using LevelGeneration;
using Lockout_2_core.Custom_Player_Behavior;
using Player;
using SNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using System.IO;
using MTFO.Managers;
using Localization;

namespace Lockout_2_core.Custom_Level_Behavior
{
    class F1_Lockout
    {
        public void Setup()
        {
            L.Error("F1 - Lockout setup");

            Patch_GS_AfterLevel.OnLevelCleanup += OnCleanup;
            Patch_WardenObjectiveManager.OnStartExpedition += OnLevelStart;
            Patch_WardenObjectiveManager.OnObjectiveComplete += OnObjectiveComplete;
            Patch_WardenObjectiveManagerExecuteEvent.OnExecuteEvent += OnEventExecuted;
            Patch_LG_PowerGeneratorClusterObjectiveEndSequence.OnGenClusterAlarmStart += OnGenClusterSolved;
            OnFactoryBuildStart();

            Patch_WardenObjectiveManager.s_AllowObjectiveComplete = false;

            m_EventsOnAllDoorsSealed.Clear();
            m_EventsOnAllDoorsSealed.Add(new()
            {
                Type = eWardenObjectiveEventType.StopEnemyWaves,
                Delay = 0
            });
            m_EventsOnAllDoorsSealed.Add(new()
            {
                Type = eWardenObjectiveEventType.UnlockSecurityDoor,
                Delay = 5,
                LocalIndex = eLocalZoneIndex.Zone_9,
                Layer = LG_LayerType.MainLayer,
                DimensionIndex = eDimensionIndex.Reality,
                WardenIntel = new() { UntranslatedText = "Biomass containment protocols have been executed successfully. Door to [ZONE_9] unlocked." }
            });
            m_EventsOnAllDoorsSealed.Add(new()
            {
                Type = eWardenObjectiveEventType.None,
                Delay = 15,
                WardenIntel = new() { UntranslatedText = "Proceed to [ZONE_9] and restore power to the <color=orange>Hydrostasis Penitentiary System</color>" }
            });

            m_EventsOnGeneratorSolved.Clear();
            m_EventsOnGeneratorSolved.Add(new()
            {
                Type = eWardenObjectiveEventType.UnlockSecurityDoor,
                Delay = 0,
                LocalIndex = eLocalZoneIndex.Zone_4,
                Layer = LG_LayerType.MainLayer,
                DimensionIndex = eDimensionIndex.Reality,
                WardenIntel = new() { UntranslatedText = "Power to local sector has been restored. Utilize the <color=orange>Terminal Maintenance System</color> to seal the <color=orange>Air Lock</color> to each BioLab sector." }
            });
            m_EventsOnGeneratorSolved.Add(new()
            {
                Type = eWardenObjectiveEventType.OpenSecurityDoor,
                Delay = 10,
                LocalIndex = eLocalZoneIndex.Zone_6,
                Layer = LG_LayerType.MainLayer,
                DimensionIndex = eDimensionIndex.Reality,
                WardenIntel = new() { UntranslatedText = "<color=orange>Emergency:// Security door to [ZONE_6] has been breached" }
            });
            m_EventsOnGeneratorSolved.Add(new()
            {
                Type = eWardenObjectiveEventType.OpenSecurityDoor,
                Delay = 20,
                LocalIndex = eLocalZoneIndex.Zone_8,
                Layer = LG_LayerType.MainLayer,
                DimensionIndex = eDimensionIndex.Reality,
                WardenIntel = new() { UntranslatedText = "<color=orange>Emergency:// Security door to [ZONE_8] has been breached" }
            });

            m_EventsOnGeneratorScanComplete.Clear();
            m_EventsOnGeneratorScanComplete.Add(new()
            {
                Type = eWardenObjectiveEventType.SpawnEnemyWave,
                Delay = 0,
                Layer = LG_LayerType.MainLayer,
                DimensionIndex = eDimensionIndex.Reality,
                EnemyWaveData = new()
                {
                    TriggerAlarm = true,
                    WavePopulation = 43,
                    WaveSettings = 139,
                    SpawnDelay = 0
                }
            });
            m_EventsOnGeneratorScanComplete.Add(new()
            {
                Type = eWardenObjectiveEventType.UpdateCustomSubObjective,
                Delay = 6,
                Layer = LG_LayerType.MainLayer,
                DimensionIndex = eDimensionIndex.Reality,
                CustomSubObjective = 4269828933,
                CustomSubObjectiveHeader = 4269828934,
                WardenIntel = new() { UntranslatedText = "Locate the <color=orange>BioLab Control Terminal(s)</color> in [ZONE_4], [ZONE_6], and [ZONE_8] and input the command <color=orange><u>BIOSEC.FORCE_PROTOCOL(0XF)</u></color>" }
            });
        }


        public void OnFactoryBuildStart()
        {
        }

        public void OnLevelStart()
        {
            m_DoorButtons = GameObject.FindObjectsOfType<LG_DoorButton>();
            m_Terminals = GameObject.FindObjectsOfType<LG_ComputerTerminal>();
            m_WeakLock = GameObject.FindObjectsOfType<LG_WeakLock>();

            LG_LevelBuilder.Current.m_currentFloor.allZones[3].m_sourceGate.SpawnedDoor.TryCast<LG_SecurityDoor>().ForceOpenSecurityDoor();
            LG_LevelBuilder.Current.m_currentFloor.allZones[5].m_sourceGate.SpawnedDoor.TryCast<LG_SecurityDoor>().ForceOpenSecurityDoor();
            LG_LevelBuilder.Current.m_currentFloor.allZones[7].m_sourceGate.SpawnedDoor.TryCast<LG_SecurityDoor>().ForceOpenSecurityDoor();

            m_GenCluster = LG_LevelBuilder.Current.m_currentFloor.allZones[2].m_areas[1].GetComponentInChildren<LG_PowerGeneratorCluster>();
            m_GenCluster.m_chainedPuzzleMidObjective.add_OnPuzzleSolved((Action)(() => 
            {
                WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(m_EventsOnGeneratorScanComplete, eWardenObjectiveEventTrigger.None, true);
                Patch_EnemyCostManager.s_MultiplyCost = true;
                Patch_EnemyCostManager.s_Multi = 0.8f;
                Patch_WardenObjectiveManager.s_AllowObjectiveComplete = true;
            }));

            m_Lights_z3_z4.Clear();
            foreach (var lg_light in LG_LevelBuilder.Current.m_currentFloor.allZones[3].m_lightsInZone) m_Lights_z3_z4.Add(lg_light.gameObject.AddComponent<LG_LightFadeAnimator>());
            foreach (var lg_light in LG_LevelBuilder.Current.m_currentFloor.allZones[4].m_lightsInZone) m_Lights_z3_z4.Add(lg_light.gameObject.AddComponent<LG_LightFadeAnimator>());

            m_Lights_z5_z6.Clear();
            foreach (var lg_light in LG_LevelBuilder.Current.m_currentFloor.allZones[5].m_lightsInZone) m_Lights_z5_z6.Add(lg_light.gameObject.AddComponent<LG_LightFadeAnimator>());
            foreach (var lg_light in LG_LevelBuilder.Current.m_currentFloor.allZones[6].m_lightsInZone) m_Lights_z5_z6.Add(lg_light.gameObject.AddComponent<LG_LightFadeAnimator>());

            m_Lights_z7_z8.Clear();
            foreach (var lg_light in LG_LevelBuilder.Current.m_currentFloor.allZones[7].m_lightsInZone) m_Lights_z7_z8.Add(lg_light.gameObject.AddComponent<LG_LightFadeAnimator>());
            foreach (var lg_light in LG_LevelBuilder.Current.m_currentFloor.allZones[8].m_lightsInZone) m_Lights_z7_z8.Add(lg_light.gameObject.AddComponent<LG_LightFadeAnimator>());

            SetPower(false);

            var objTerm = LG_LevelBuilder.Current.m_currentFloor.allZones[9].TerminalsSpawnedInZone[0];
            var jsonContent = JsonSerializer.Deserialize<List<IWillFuckingKillAllLocalizers>>(File.ReadAllText(ConfigManager.CustomPath + @"\f1output.json"));
            var PCO = new Il2CppSystem.Collections.Generic.List<TerminalOutput>();
            foreach (var entry in jsonContent)
            {
                var commandOutput = new TerminalOutput();

                commandOutput.LineType = entry.lineType;
                commandOutput.Output = new() { UntranslatedText = entry.Output };
                commandOutput.Time = entry.Time;

                PCO.Add(commandOutput);
            }

            objTerm.m_command.m_commandPostOutputMap.Add(TERM_Command.WardenObjectiveSpecialCommand, PCO);
        }

        public void OnGenClusterSolved()
        {
            SetPower(true);
            Patch_LG_DoorButton.s_Override = true;
            m_GenCluster.m_sound.Post(574196021);
            WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(m_EventsOnGeneratorSolved, eWardenObjectiveEventTrigger.None, true);
        }

        public void SetPower(bool status)
        {
            if (SNet.IsMaster) EnvironmentStateManager.AttemptSetExpeditionLightMode(status);


            foreach (var button in m_DoorButtons)
            {
                if (status == false)
                {
                    button.m_enabled = false;
                    button.m_anim.gameObject.active = false;
                }
                else
                {
                    button.m_anim.gameObject.active = true;
                    var weaklock = button.GetComponentInChildren<LG_WeakLock>();
                    if (weaklock == null) button.m_enabled = true;
                    else if (weaklock.Status == eWeakLockStatus.Unlocked) button.m_enabled = true;
                }
            }
            foreach (var terminal in m_Terminals)
            {
                terminal.transform.FindChild("Interaction").gameObject.active = status;

                var display = terminal.transform.FindChild("Graphics/kit_ElectronicsTerminalConsole/Display");
                if (display != null) display.gameObject.active = status;
            }
            foreach (var weaklock in m_WeakLock)
            {
                weaklock.m_intHack.m_isActive = status;

                var display = weaklock.transform.FindChild("HackableLock/SecurityLock/g_WeakLock/Security_Display_Locked");
                if (display != null) display.gameObject.active = status;
                else
                {
                    display = weaklock.transform.FindChild("HackableLock/Security_Display_Locked");
                    if (display != null) display.gameObject.active = status;
                }
            }
        }

        public void CloseDoorToZone(int localIndex)
        {
            L.Debug($"Attempting to seal door to zone {localIndex}");

            var zone = LG_LevelBuilder.Current.m_currentFloor.allZones[localIndex];
            var secDoor = zone.m_sourceGate.SpawnedDoor.TryCast<LG_SecurityDoor>();
            secDoor.m_sync.AttemptDoorInteraction(eDoorInteractionType.Close);
            secDoor.m_locks.SetupAsLockedNoKey(LocalizerGenocideReal.GenerateLocalizedText($"<color=orange>ZONE {localIndex} IN EMERGENCY LOCKDOWN.\n<u><color=orange>TYPE-F</u> BIOMASS CONTAINMENT BREACH DETECTED.\n CONTACT HAZARDOUS ENVIRONMENT CONTAINMENT UNIT IMMEDIATELY</color>"));
            zone.m_sourceGate.IsTraversable = false;
            zone.m_sourceGate.HasBeenOpenedDuringPlay = false;
        }

        private void OnObjectiveComplete()
        {
        }

        public void OnEventExecuted(WardenObjectiveEventData eData)
        {
            if ((int)eData.Type != 14 || m_SealedZones.Contains((int)eData.LocalIndex)) return;
            switch((int)eData.LocalIndex)
            {
                case 3: foreach(var light in m_Lights_z3_z4) { light.FadeOut(); }; break;
                case 5: foreach(var light in m_Lights_z5_z6) { light.FadeOut(); }; break;
                case 7: foreach(var light in m_Lights_z7_z8) { light.FadeOut(); }; break;
            }
            
            m_SealedZones.Add((int)eData.LocalIndex);
            CoroutineHandler.Add(ExterminateZones(new() { (int)eData.LocalIndex, (int)eData.LocalIndex + 1 }));

            if (!m_SealedZones.Contains(3) || !m_SealedZones.Contains(5) || !m_SealedZones.Contains(7)) return;
            WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(m_EventsOnAllDoorsSealed, eWardenObjectiveEventTrigger.None, true);

            foreach (var mevent in Mastermind.Current.m_events) 
            {
                Mastermind.Current.RemoveEvent(mevent);
                L.Debug($"Removed mastermind event {mevent.EventID}");
            }

            m_GenCluster.m_sound.Post(602369431);
        }

        public IEnumerator ExterminateZones(List<int> localIndexesToKill)
        {
            yield return new WaitForSeconds(8);

            var playerTargets = new List<PlayerAgent>();
            var includeLocalPlayer = false;
            var allTargetsDead = false;

            foreach (var player in PlayerManager.PlayerAgentsInLevel)
            {
                if (!localIndexesToKill.Contains((int)player.CourseNode.m_zone.LocalIndex)) continue;
                playerTargets.Add(player);
                if (player == PlayerManager.GetLocalPlayerAgent()) includeLocalPlayer = true;
                Patch_PLOC_Downed.s_PermaDeath = includeLocalPlayer;
            }

            if (includeLocalPlayer) AttemptLocalFogTransition();

            yield return new WaitForSeconds(8);

            while (!allTargetsDead)
            {
                allTargetsDead = true;
                foreach (var player in playerTargets)
                {
                    if (!PlayerManager.PlayerAgentsInLevel.Contains(player)) continue;
                    if (SNet.IsMaster) player.Damage.NoAirDamage(0.75f);

                    if (player.Locomotion.m_currentStateEnum == PlayerLocomotion.PLOC_State.Downed && player == PlayerManager.GetLocalPlayerAgent())
                    {
                        PlayerDeathManager.Current.KillLocalPlayerAgent();
                        Patch_PLOC_Downed.s_PermaDeath = false;
                    }

                    allTargetsDead = false;
                }
                yield return new WaitForSeconds(2.0f);
            }

            yield return new WaitForSeconds(4);

            List<EnemyAgent> targets = new();

            foreach (var localIndex in localIndexesToKill)
                foreach (var node in LG_LevelBuilder.Current.m_currentFloor.allZones[localIndex].m_courseNodes)
                {
                    foreach (var enemy in node.m_enemiesInNode.ToArray())
                    {
                        if (SNet.IsMaster)
                        {
                            enemy.Damage.Health = 0;
                            enemy.Damage.InstantDead(true);
                        }
                        enemy.EnemySFXData.SFX_ID_die = 0;
                    }
                }
             

            yield return null;
        }

        public void AttemptLocalFogTransition()
        {
            var fogState = default(FogState);

            fogState.FogDataID = 132;
            fogState.FogTransitionDuration = 30;
            fogState.StartTime = Clock.ExpeditionProgressionTime;
            fogState.StartFogSettings = PlayerManager.GetLocalPlayerAgent().FPSCamera.PrelitVolume.GetFogSettings();

            EnvironmentStateManager.Current.m_stateReplicator.State.FogStates[0] = fogState;
            EnvironmentStateManager.Current.UpdateFogSettingsForState(EnvironmentStateManager.Current.m_stateReplicator.State);
        }

        public void OnCleanup() 
        {
            L.Error("F1 - Lockout cleanup!");

            m_DoorButtons = null;
            m_Terminals = null;
            m_WeakLock = null;
            Patch_LG_DoorButton.s_Override = false;

            m_SealedZones.Clear();
            m_EventsOnAllDoorsSealed.Clear();
            m_GenCluster = null;

            m_Lights_z3_z4.Clear();
            m_Lights_z5_z6.Clear();
            m_Lights_z7_z8.Clear();

            Patch_EnemyCostManager.s_MultiplyCost = false;
            Patch_EnemyCostManager.s_Multi = 1f;
            Patch_WardenObjectiveManager.s_AllowObjectiveComplete = true;

            Patch_GS_AfterLevel.OnLevelCleanup -= OnCleanup;
            Patch_WardenObjectiveManager.OnStartExpedition -= OnLevelStart;
            Patch_WardenObjectiveManager.OnObjectiveComplete -= OnObjectiveComplete;
            Patch_WardenObjectiveManagerExecuteEvent.OnExecuteEvent -= OnEventExecuted;
            Patch_LG_PowerGeneratorClusterObjectiveEndSequence.OnGenClusterAlarmStart -= OnGenClusterSolved;
        }

        public List<int> m_SealedZones = new();
        public Il2CppSystem.Collections.Generic.List<WardenObjectiveEventData> m_EventsOnAllDoorsSealed = new();
        public Il2CppSystem.Collections.Generic.List<WardenObjectiveEventData> m_EventsOnGeneratorSolved = new();
        public Il2CppSystem.Collections.Generic.List<WardenObjectiveEventData> m_EventsOnGeneratorScanComplete = new();

        public List<LG_LightFadeAnimator> m_Lights_z3_z4 = new();
        public List<LG_LightFadeAnimator> m_Lights_z5_z6 = new();
        public List<LG_LightFadeAnimator> m_Lights_z7_z8 = new();

        public LG_DoorButton[] m_DoorButtons;
        public LG_ComputerTerminal[] m_Terminals;
        public LG_WeakLock[] m_WeakLock;

        public LG_PowerGeneratorCluster m_GenCluster;
    }
}
