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

namespace Lockout_2_core.Custom_Level_Behavior
{
    class B1_Exothermic
    {
        public  void Setup()
        {
            L.Error("B1 - Exothermic setup");
            Patch_WardenObjectiveManager.OnObjectiveComplete += OnObjectiveCompleted;
            Patch_WardenObjectiveManager.OnStartExpedition += OnGameplayStarted;
            Patch_WardenObjectiveManagerExecuteEvent.OnExecuteEvent += OnExecuteEvent;
            Patch_GS_AfterLevel.OnLevelCleanup += OnCleanup;
            Patch_LG_ComputerTerminalManager.OnReceiveCustomCommand += OnSpecialCommandInput;

            OnFactoryBuildStart();

            pResetHSU = new();
            pResetHSU.isSequenceIncomplete = true;
            pResetHSU.status = eHSUActivatorStatus.WaitingForInsert;
        }


        public void OnLabAssetsLoaded(AsyncOperation operation)
        {
            L.Debug("Lab asset bundle loaded successfully");
        }

        public void BuildCustomLevelContent()
        {
            m_CustomLGPillar = new();
            m_CustomLGPillar.name = "CustomLevelGenIsPrettyCool";
            m_CustomLGPillar.transform.parent = LG_LevelBuilder.Current.m_currentFloor.allZones[1].m_areas[0].transform;
            m_CustomLGPillar.transform.position = new Vector3(0.0f, 6.0f, 187.0f);
            m_CustomLGPillar.transform.localScale = new Vector3(1.2f, 1.0f, 1.0f);
            m_CustomLGPillar.transform.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
            var pillar = GameObject.Instantiate(AssetAPI.GetLoadedAsset("ASSETS/ASSETPREFABS/COMPLEX/MINING/PROPS/REFINERY/REFINERY_PILLAR_400X400X2000/MINING_REFINERY_PILLAR_400X400X2000_BULK_V01.PREFAB"), m_CustomLGPillar.transform).TryCast<GameObject>();
            foreach (var prefabSpawner in pillar.GetComponentsInChildren<LG_PrefabSpawner>())
            {
                prefabSpawner.OnBuild(true);
            }

            m_HSU_Cores = GameObject.FindObjectsOfTypeAll(Il2CppType.From(typeof(LG_HSUActivator_Core))).Select((x) => x.Cast<LG_HSUActivator_Core>()).ToArray();
            m_HSU_Cores[3].gameObject.active = false;

            foreach (var instance in m_HSU_Cores)
            {
                if (instance.gameObject.name == "HSU_Activator_machine" && m_CoolantHSU == null)
                {
                    m_CoolantHSU = GameObject.Instantiate(instance, m_CustomLGPillar.transform);

                    m_CoolantHSU.m_publicName = "HEAT_EXCHANGER_DC1";
                    m_CoolantHSU.SpawnNode = LG_LevelBuilder.Current.m_currentFloor.allZones[1].m_areas[0].m_courseNode;
                    m_CoolantHSU.Setup();

                    m_CoolantHSU.name = "B1_CoolantHSU";
                    m_CoolantHSU.m_terminalItem.SpawnNode = LG_LevelBuilder.Current.m_currentFloor.allZones[1].m_areas[0].m_courseNode;
                    m_CoolantHSU.transform.localPosition = new Vector3(0.0f, 0.75f, -1.25f);
                    m_CoolantHSU.transform.localScale = new Vector3(0.87f, 1.0f, 1.0f);
                    m_CoolantHSU.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

                    m_CoolantHSU.m_itemComingOutAlign.localScale = Vector3.zero;

                    m_CoolantHSU.m_serialNumber = 742;
                    m_CoolantHSU.m_sound = new();
                    
                    L.Debug($"Custom LG_HSUActivator_Core {instance.name} setup!");
                }
            }


            m_CoolantHSU.add_OnHSUExitSequence((Action<LG_HSUActivator_Core>)OnHSUExitSequence);
            m_CoolantTerminal = LG_LevelBuilder.Current.m_currentFloor.allZones[1].TerminalsSpawnedInZone[0];
            m_CoolantTerminal.SetupAsWardenObjectiveSpecialCommand();
            m_CoolantTerminal.TrySyncSetCommandRemoved(TERM_Command.WardenObjectiveSpecialCommand);
            m_CoolantTerminal.m_sound.Post(AK.EVENTS.DOOR_ALARM);

            m_CoolantTerminal.m_command.AddOutputEmptyLine();
            m_CoolantTerminal.m_command.AddOutputEmptyLine();
            m_CoolantTerminal.m_command.AddOutput("<color=orange>WARNING: AIR DECONTAMINATION UNIT CRITICAL ERROR - COOLANT RESUPPLY NEEDED\nSEVERITY: EMERGENCY</color>");
            m_CoolantTerminal.m_command.AddOutputEmptyLine();
        }

        public void OnFactoryBuildStart()
        {
            AssetShardManager.LoadShardAsync(AssetBundleName.Complex_Tech, AssetBundleShard.S19, (Action<AsyncOperation>)OnLabAssetsLoaded, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        public void OnGameplayStarted() 
        {
            L.Debug("OnGameplayStarted()");

            m_CoolantHSU.PostCullingSetup();
            m_CoolantHSU.m_insertHSUInteraction.SetActive(true);

            if (!SNet.IsMaster) NetworkAPI.InvokeEvent("ClientRequestInfo", 0, SNet_ChannelType.GameOrderCritical);
            
            else
            {
                if (m_CoolantTerminal.m_command.m_commandHelpStrings.ContainsKey(TERM_Command.UniqueCommand1))
                    m_CoolantTerminal.m_command.m_commandHelpStrings[TERM_Command.UniqueCommand1][0] = "Halt the DeconUnit error reporting system. " + $"<color=orange>{4 - m_SuppressCount} / 4 uses remaining</color>";
            }
        }

        public void OnHSUExitSequence(LG_HSUActivator_Core core)
        {
            var text = new LocalizedText();
            text.Id = 99999;

            m_CoolantHSU.SyncStatusChanged(ref pResetHSU, false);
            m_CoolantHSU.m_stateReplicator.SetStateUnsynced(pResetHSU);
            m_CoolantCount += 1;
            L.Debug($"Congradulations, you have inserted {m_CoolantCount} coolant cells into the HS_Unit");

            if (m_CoolantCount >= 3)
            {
                L.Debug($"objective terminal ready for win condition");

                text.UntranslatedText = "Air Decontamination Unit restored to working condition. Proceed to input command [SPECIAL_COMMAND] in [ITEM_SERIAL]";
                WardenObjectiveManager.DisplayWardenIntel(LG_LayerType.MainLayer, text);

                if (m_CoolantTerminal.CommandIsRemoved(TERM_Command.WardenObjectiveSpecialCommand)) UnRemoveCommand(m_CoolantTerminal, TERM_Command.WardenObjectiveSpecialCommand);

                m_CoolantTerminal.TrySyncSetCommandRemoved(TERM_Command.UniqueCommand1);
            }
            else
            {
                text.UntranslatedText = $"Coolant tank <color=orange>{m_CoolantCount} / 3</color> inserted.";
                WardenObjectiveManager.DisplayWardenIntel(LG_LayerType.MainLayer, text);
            }

            var objData = new pObjectiveUpdate();
            objData.ProgressObjective = true;
            objData.CoolantCount = Manager_CustomLevelBehavior.B1.m_CoolantCount;

            L.Debug($"Sending objective status to clients - ProgressObjective {objData.ProgressObjective} - CoolantCount {objData.CoolantCount}");

            EnvironmentStateManager.AttemptStartFogTransition(Convert.ToUInt32(107 + m_CoolantCount), 5f, eDimensionIndex.Reality);

            NetworkAPI.InvokeEvent("B1ObjectiveStatus", objData, SNet_ChannelType.GameOrderCritical);
        }

        public void OnSpecialCommandInput()
        {
            L.Debug("Special command recieved! Does everyone see this in their console?");

            m_SuppressCount += 1;
            m_CoolantTerminal.m_sound.Stop();
            m_CoolantTerminal.m_command.m_commandHelpStrings[TERM_Command.UniqueCommand1][0] = "Halt the DeconUnit error reporting system. " + $"<color=orange>{4 - Manager_CustomLevelBehavior.B1.m_SuppressCount} / 4 uses remaining</color>";

            m_CoolantTerminal.AddLine($"<color=orange>{4 - m_SuppressCount} / 4</color> Command Usages Remaining");

            if (m_SuppressCount >= 4)
            {
                m_CoolantTerminal.AddLine("<color=red>Command [ALARMSEQ.DISABLE()] Disabled!</color>\n");
            }

            NetworkAPI.InvokeEvent("B1AlarmShutoff", new pAlarmShutoff(), SNet_ChannelType.GameOrderCritical);
        }

        public void OnExecuteEvent(WardenObjectiveEventData eventData)
        {
            if ((int)eventData.FogSetting != 5000) return;
            L.Debug("Got special event! wow!");

            m_CoolantTerminal.m_sound.Post(AK.EVENTS.DOOR_ALARM);

            if (m_SuppressCount < 4 && m_CoolantTerminal.CommandIsRemoved(TERM_Command.UniqueCommand1)) UnRemoveCommand(m_CoolantTerminal, TERM_Command.UniqueCommand1);

            m_CoolantTerminal.m_command.ClearOutputQueueAndScreenBuffer();
            m_CoolantTerminal.AddLine("<color=orange>WARNING: AIR DECONTAMINATION UNIT CRITICAL ERROR - COOLANT RESUPPLY NEEDED\nSEVERITY: EMERGENCY</color>");
        }

        public static void ClientRequestInfo(ulong x, byte y)
        {
            if (!SNet.IsMaster) return;

            L.Debug("Client is requesting objective info");

            //List<byte> removedCommands = new();
            byte terminalStatus = 0;
            var term = Manager_CustomLevelBehavior.B1.m_CoolantTerminal;

            if (term.CommandIsRemoved(TERM_Command.WardenObjectiveSpecialCommand) && !term.CommandIsRemoved(TERM_Command.UniqueCommand1)) terminalStatus = 1;
            if (!term.CommandIsRemoved(TERM_Command.WardenObjectiveSpecialCommand) && term.CommandIsRemoved(TERM_Command.UniqueCommand1)) terminalStatus = 2;
            if (term.CommandIsRemoved(TERM_Command.WardenObjectiveSpecialCommand) && term.CommandIsRemoved(TERM_Command.UniqueCommand1)) terminalStatus = 3;
            /*
            removedCommands.Add(Manager_CustomLevelBehavior.B1.m_CoolantTerminal.m_command.m_removedCommands[0]);
            removedCommands.Add(Manager_CustomLevelBehavior.B1.m_CoolantTerminal.m_command.m_removedCommands[1]);

            if (removedCommands.Contains(24) && !removedCommands.Contains(35)) terminalStatus = 1;
            if (!removedCommands.Contains(24) && removedCommands.Contains(35)) terminalStatus = 2;
            if (removedCommands.Contains(24) && removedCommands.Contains(35)) terminalStatus = 3;
            */

            L.Debug($"Sending a count of {Manager_CustomLevelBehavior.B1.m_CoolantCount}, a terminal status of {terminalStatus}, and a suppress count of {Manager_CustomLevelBehavior.B1.m_SuppressCount}");

            var objData = new pObjectiveUpdate();
            objData.ProgressObjective = false;
            objData.CoolantCount = Manager_CustomLevelBehavior.B1.m_CoolantCount;

            var termData = new pTerminalUpdate();
            termData.SuppressCount = Manager_CustomLevelBehavior.B1.m_SuppressCount;
            termData.TerminalStatus = terminalStatus;

            NetworkAPI.InvokeEvent("B1TerminalStatus", termData, SNet_ChannelType.GameOrderCritical);
            NetworkAPI.InvokeEvent("B1ObjectiveStatus", objData, SNet_ChannelType.GameOrderCritical);
        }

        public static void RecieveTerminalUpdate(ulong x, pTerminalUpdate data)
        {
            if (SNet.IsMaster) return;

            L.Debug($"Recieved a suppress count of {data.SuppressCount}");
            Manager_CustomLevelBehavior.B1.m_SuppressCount = data.SuppressCount;

            L.Debug($"Recieved a terminal status of {data.TerminalStatus}");

            switch (data.TerminalStatus)
            {
                case 0: UnRemoveCommand(Manager_CustomLevelBehavior.B1.m_CoolantTerminal, TERM_Command.WardenObjectiveSpecialCommand); UnRemoveCommand(Manager_CustomLevelBehavior.B1.m_CoolantTerminal, TERM_Command.UniqueCommand1); break;
                case 1: Manager_CustomLevelBehavior.B1.m_CoolantTerminal.TrySyncSetCommandRemoved(TERM_Command.WardenObjectiveSpecialCommand); UnRemoveCommand(Manager_CustomLevelBehavior.B1.m_CoolantTerminal, TERM_Command.UniqueCommand1); break;
                case 2: UnRemoveCommand(Manager_CustomLevelBehavior.B1.m_CoolantTerminal, TERM_Command.WardenObjectiveSpecialCommand); Manager_CustomLevelBehavior.B1.m_CoolantTerminal.TrySyncSetCommandRemoved(TERM_Command.UniqueCommand1); break;
                case 3: Manager_CustomLevelBehavior.B1.m_CoolantTerminal.TrySyncSetCommandRemoved(TERM_Command.WardenObjectiveSpecialCommand); Manager_CustomLevelBehavior.B1.m_CoolantTerminal.TrySyncSetCommandRemoved(TERM_Command.UniqueCommand1); break;
            }

            L.Debug($"{!Manager_CustomLevelBehavior.B1.m_CoolantTerminal.CommandIsRemoved(TERM_Command.UniqueCommand1)} | {Manager_CustomLevelBehavior.B1.m_CoolantTerminal.CommandIsRemoved(TERM_Command.WardenObjectiveSpecialCommand)}");

            if (Manager_CustomLevelBehavior.B1.m_CoolantTerminal.m_command.m_commandHelpStrings.ContainsKey(TERM_Command.UniqueCommand1))
                Manager_CustomLevelBehavior.B1.m_CoolantTerminal.m_command.m_commandHelpStrings[TERM_Command.UniqueCommand1][0] = "Halt the DeconUnit error reporting system. " + $"<color=orange>{4 - Manager_CustomLevelBehavior.B1.m_SuppressCount} / 4 uses remaining</color>";
        }

        public static void RecieveObjectiveUpdate(ulong x, pObjectiveUpdate data)
        {
            if (SNet.IsMaster) return;

            if (data.ProgressObjective)
            {
                var text = new LocalizedText();
                text.Id = 99999;

                L.Debug($"Congradulations, you have inserted {data.CoolantCount} coolant cells into the HS_Unit");

                if (data.CoolantCount >= 3)
                {
                    L.Debug($"objective terminal ready for win condition");

                    text.UntranslatedText = "Air Decontamination Unit restored to working condition. Proceed to input command [SPECIAL_COMMAND] in [ITEM_SERIAL]";
                    WardenObjectiveManager.DisplayWardenIntel(LG_LayerType.MainLayer, text);
                }
                else
                {
                    text.UntranslatedText = $"Coolant tank <color=orange>{data.CoolantCount} / 3</color> inserted.";
                    WardenObjectiveManager.DisplayWardenIntel(LG_LayerType.MainLayer, text);
                }
            }

            Manager_CustomLevelBehavior.B1.m_CoolantHSU.m_insertHSUInteraction.SetActive(true);

            Manager_CustomLevelBehavior.B1.m_CoolantCount = data.CoolantCount;
            L.Debug($"Updated coolant tank count - {data.CoolantCount}");
        }

        public static void RecieveAlarmShutoff(ulong x, pAlarmShutoff data)
        {
            L.Debug("Somebody just shutted off the alarm :(");
            Manager_CustomLevelBehavior.B1.m_SuppressCount += 1;
            Manager_CustomLevelBehavior.B1.m_CoolantTerminal.m_sound.Stop();
            Manager_CustomLevelBehavior.B1.m_CoolantTerminal.m_command.m_commandHelpStrings[TERM_Command.UniqueCommand1][0] = "Halt the DeconUnit error reporting system. " + $"<color=orange>{4 - Manager_CustomLevelBehavior.B1.m_SuppressCount} / 4 uses remaining</color>";

            Manager_CustomLevelBehavior.B1.m_CoolantTerminal.AddLine($"<color=orange>{4 - Manager_CustomLevelBehavior.B1.m_SuppressCount} / 4</color> Command Usages Remaining");

            if (Manager_CustomLevelBehavior.B1.m_SuppressCount >= 4)
            {
                Manager_CustomLevelBehavior.B1.m_CoolantTerminal.AddLine("<color=red>Command [ALARMSEQ.DISABLE()] Disabled!</color>\n");
            }
        }

        public void OnObjectiveCompleted()
        {
            L.Debug("Warden objective completed - Stopping terminal alarm");
            m_CoolantTerminal.m_sound.Stop();
            m_MissionComplete = true;
            foreach (var wardenEvent in WardenObjectiveManager.m_wardenObjectiveEventCoroutines)
            {
                if (wardenEvent == null)
                {
                    L.Debug("coroutine was null, skipping");
                    continue;
                }

                L.Debug("Objective is complete. coroutine genocide. This is so sad");
                CoroutineManager.StopCoroutine(wardenEvent);
            }
        }

        public static void UnRemoveCommand(LG_ComputerTerminal terminal, TERM_Command command)
        {
            var oldState = terminal.m_stateReplicator.State;
            var removedCmds = oldState.RemovedCommands;
            for (var i = 0; i < removedCmds.Length; i++)
            {
                if (removedCmds[i] == command)
                    removedCmds[i] = TERM_Command.None;
            }
            oldState.RemovedCommands = removedCmds;
            terminal.m_stateReplicator.State = oldState;
            L.Debug("Thank you flowaria, i love you <3");
        }

        public void OnCleanup() 
        {
            L.Error("B1 - Exothermic cleanup!");

            GameObject.Destroy(m_CoolantHSU);

            m_HSU_Cores = null;
            m_CustomLGPillar = null;
            m_CoolantHSU = null;
            m_SuppressCount = 0;
            m_CoolantCount = 0;
            m_MissionComplete = false;

            Patch_WardenObjectiveManager.OnObjectiveComplete -= OnObjectiveCompleted;
            Patch_WardenObjectiveManager.OnStartExpedition -= OnGameplayStarted;
            Patch_WardenObjectiveManagerExecuteEvent.OnExecuteEvent -= OnExecuteEvent;
            Patch_GS_AfterLevel.OnLevelCleanup -= OnCleanup;
            Patch_LG_ComputerTerminalManager.OnReceiveCustomCommand -= OnSpecialCommandInput;
        }

        public LG_HSUActivator_Core[] m_HSU_Cores;
        public GameObject m_CustomLGPillar;
        public LG_HSUActivator_Core m_CoolantHSU;
        public LG_ComputerTerminal m_CoolantTerminal;
        public pHSUActivatorState pResetHSU;
        public int m_CoolantCount;
        public int m_SuppressCount;
        public bool m_MissionComplete;

        public struct pObjectiveUpdate
        {
            public int CoolantCount;
            public bool ProgressObjective;
        }

        public struct pTerminalUpdate
        {
            public int SuppressCount;
            public byte TerminalStatus;
        }

        public struct pAlarmShutoff { }
    }
}
