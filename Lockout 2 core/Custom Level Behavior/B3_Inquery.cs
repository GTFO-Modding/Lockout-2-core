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

namespace Lockout_2_core.Custom_Level_Behavior
{
    class B3_Inquery
    {
        public  void Setup()
        {
            L.Error("B3 - Inquery setup");
            Patch_GS_AfterLevel.OnLevelCleanup += OnCleanup;
            Patch_LG_ComputerTerminal.OnSetupTerminalGatherTerminal += PasswordLockTerminal;
            Patch_LG_ComputerTerminalCommandInterpreter.OnGatherTerminal += OnTerminalGathered;
            Patch_LG_AlarmShutdownOnTerminalJob.OnSetupAlarmShutdownTerminal += PasswordLockTerminal;
        }

        public void PasswordLockTerminal(LG_ComputerTerminal terminal, AIG_CourseNode spawnNode)
        {
            L.Debug($"Attempting to password lock terminal {terminal.m_serialNumber}");
            if (spawnNode.m_zone != LG_LevelBuilder.Current.m_currentFloor.allZones[7]) return;
            L.Debug($"terminal {terminal.m_serialNumber} was in zone 7, proceeding");

            terminal.StartStateData.GeneratePassword = true;
            terminal.StartStateData.PasswordProtected = true;
            terminal.StartStateData.PasswordPartCount = 1;
            terminal.StartStateData.ShowPasswordLength = true;
            terminal.StartStateData.ShowPasswordPartPositions = true;

            L.Debug($"terminal {terminal.m_serialNumber} StartStateData setup");

            var imGonnaFuckingCum = new Il2CppSystem.Collections.Generic.List<Il2CppSystem.Collections.Generic.List<TerminalZoneSelectionData>>();
            var selectionDatas = new Il2CppSystem.Collections.Generic.List<TerminalZoneSelectionData>();
            imGonnaFuckingCum.Add(selectionDatas);
            var selectionData = new TerminalZoneSelectionData()
            {
                LocalIndex = eLocalZoneIndex.Zone_7,
                SeedType = eSeedType.BuildSeed,
                TerminalIndex = 0,
                StaticSeed = 0
            };

            if (terminal.m_isWardenObjective)
            {
                L.Debug("This terminal is the warden objective, so we put the password in zone_6 instead to prevent softlocks");
                selectionData.LocalIndex = eLocalZoneIndex.Zone_6;
            }

            selectionDatas.Add(selectionData);

            terminal.StartStateData.TerminalZoneSelectionDatas = imGonnaFuckingCum;

            L.Debug($"terminal {terminal.m_serialNumber} Selection datas setup");

            LG_Factory.InjectJob(new LG_TerminalPasswordLinkerJob(terminal, spawnNode.m_zone.Layer, 1, terminal.StartStateData.ShowPasswordLength, terminal.StartStateData.ShowPasswordPartPositions, terminal.StartStateData.TerminalZoneSelectionDatas), LG_Factory.BatchName.FinalLogicLinking);
            terminal.SetStartState(Il2CppSystem.Enum.Parse(Il2CppType.Of<TERM_State>(), "PasswordProtected").TryCast<Il2CppSystem.Enum>());
        }

        public void OnTerminalGathered(LG_ComputerTerminal terminal, AIG_CourseNode spawnNode)
        {
            var objective = WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer);
            L.Debug("Terminal was gathered, triggering events on activate");
            WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(objective.EventsOnActivate, eWardenObjectiveEventTrigger.OnStart, true);

            terminal.AddLine("<color=red>Critical Error://</color> User Credentials invalid. Alarm sequence activated.");
            terminal.AddLine("Please contact your system administrator. Severity:// <color=red><u>Emergency</u></color>");
        }

        public void OnCleanup() 
        {
            L.Error("B3 - Inquery cleanup!");

            Patch_GS_AfterLevel.OnLevelCleanup -= OnCleanup;
            Patch_LG_ComputerTerminal.OnSetupTerminalGatherTerminal -= PasswordLockTerminal;
            Patch_LG_ComputerTerminalCommandInterpreter.OnGatherTerminal -= OnTerminalGathered;
            Patch_LG_AlarmShutdownOnTerminalJob.OnSetupAlarmShutdownTerminal -= PasswordLockTerminal;
        }
    }
}
