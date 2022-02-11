using LevelGeneration;
using SNetwork;
using System;
using UnityEngine;

namespace Lockout_2_core.Custom_Level_Behavior
{
    class A1_Reawakening
    {
        public void Setup()
        {
            L.Error("A1 - Reawakening setup!");
            Patch_WardenObjectiveManager.OnObjectiveComplete += OnObjectiveCompleted;
            Patch_WardenObjectiveManager.OnStartExpedition += OnFactoryBuildComplete;
            Patch_GS_AfterLevel.OnLevelCleanup += OnCleanup;
        }

        public void OnFactoryBuildComplete() 
        {
            m_DoorButtons = GameObject.FindObjectsOfType<LG_DoorButton>();
            m_Terminals = GameObject.FindObjectsOfType<LG_ComputerTerminal>();
            m_WeakLock = GameObject.FindObjectsOfType<LG_WeakLock>();
            m_SecurityDoor = GameObject.FindObjectsOfType<LG_SecurityDoor>();

            if (WardenObjectiveManager.Current.CheckWardenObjectiveCompleted(LG_LayerType.MainLayer)) return;
            SetPower(false);
        }

        public void OnObjectiveCompleted()
        {
            SetPower(true);

            foreach (var button in m_DoorButtons)
            {
                var weaklock = button.GetComponentInChildren<LG_WeakLock>();
                if (weaklock == null) continue;
                L.Debug("Button was locked, turning it back off");
                button.m_enabled = false;
            }

            LG_LevelBuilder.Current.m_currentFloor.TryGetZoneByLocalIndex(eDimensionIndex.Reality, LG_LayerType.MainLayer, GameData.eLocalZoneIndex.Zone_1, out var zone1);
            var secDoor = zone1.m_sourceGate.GetComponentInChildren<LG_SecurityDoor>();
            secDoor.m_sync.AttemptDoorInteraction(eDoorInteractionType.Close);
            zone1.m_sourceGate.IsTraversable = false;
            zone1.m_sourceGate.HasBeenOpenedDuringPlay = false;
        }

        public void SetPower(bool status)
        {
            if (SNet.IsMaster) EnvironmentStateManager.AttemptSetExpeditionLightMode(status);


            foreach (var button in m_DoorButtons)
            {
                L.Debug($"LG_DoorButton set to {status}");
                button.m_enabled = status;
                button.m_anim.gameObject.active = status;
            }
            foreach (var terminal in m_Terminals)
            {
                L.Debug($"LG_ComputerTerminal set to {status}");
                terminal.transform.FindChild("Interaction").gameObject.active = status;

                var display = terminal.transform.FindChild("Graphics/kit_ElectronicsTerminalConsole/Display");
                if (display != null) display.gameObject.active = status;
            }
            foreach (var weaklock in m_WeakLock)
            {
                L.Debug($"LG_WeakLock set to {status}");
                weaklock.m_intHack.m_isActive = status;

                var display = weaklock.transform.FindChild("HackableLock/SecurityLock/g_WeakLock/Security_Display_Locked");
                if (display != null) display.gameObject.active = status;
                else
                {
                    display = weaklock.transform.FindChild("HackableLock/Security_Display_Locked");
                    if (display != null) display.gameObject.active = status;
                }
                
            }
            /*
            foreach (var secdoor in m_SecurityDoor)
            {
                secdoor.transform.FindChild("crossing/Interaction_Use_KeyItem").gameObject.active = status;
                secdoor.transform.FindChild("crossing/Interaction_Hack").gameObject.active = status;
                secdoor.transform.FindChild("crossing/Interaction_Message").gameObject.active = status;
                secdoor.transform.FindChild("crossing/Interaction_Open_Or_Activate").gameObject.active = status;
            }
            */
        }

        public void OnCleanup() 
        {
            L.Error("A1 - Reawakening Cleanup!");

            m_DoorButtons = null;
            m_Terminals = null;
            m_WeakLock = null;
            m_SecurityDoor = null;

            Patch_WardenObjectiveManager.OnObjectiveComplete -= OnObjectiveCompleted;
            Patch_WardenObjectiveManager.OnStartExpedition -= OnFactoryBuildComplete;
            Patch_GS_AfterLevel.OnLevelCleanup -= OnCleanup;
        }

        public LG_DoorButton[] m_DoorButtons;
        public LG_ComputerTerminal[] m_Terminals;
        public LG_WeakLock[] m_WeakLock;
        public LG_SecurityDoor[] m_SecurityDoor;
    }
}
