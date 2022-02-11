using System;
using System.Collections.Generic;
using AIGraph;
using GameData;
using HarmonyLib;
using LevelGeneration;

namespace Lockout_2_core
{
    class Patch_LG_AlarmShutdownOnTerminalJob
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_AlarmShutdownOnTerminalJob);
            var patchType = typeof(Patch_LG_AlarmShutdownOnTerminalJob);

            instance.Patch(gameType.GetMethod("Build"), null, new HarmonyMethod(patchType, "Build"));
        }

        public static void Build(LG_AlarmShutdownOnTerminalJob __instance)
        {


            L.Debug($"Terminal {__instance.m_securityDoor.LinkedComputerTerminal.m_serialNumber} is a DisableAlarm terminal! Invoking :O");
            OnSetupAlarmShutdownTerminal.Invoke(__instance.m_securityDoor.LinkedComputerTerminal, __instance.m_securityDoor.LinkedComputerTerminal.SpawnNode);
        }

        public static event Action<LG_ComputerTerminal, AIG_CourseNode> OnSetupAlarmShutdownTerminal;
    }
}