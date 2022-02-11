using System;
using AIGraph;
using HarmonyLib;
using LevelGeneration;

namespace Lockout_2_core
{
    class Patch_LG_ComputerTerminal
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_ComputerTerminal);
            var patchType = typeof(Patch_LG_ComputerTerminal);

            instance.Patch(gameType.GetMethod("SetupAsWardenObjectiveGatherTerminal"), null, new HarmonyMethod(patchType, "SetupAsWardenObjectiveGatherTerminal"));
            instance.Patch(gameType.GetMethod("SetupAsWardenObjectiveSpecialCommand"), null, new HarmonyMethod(patchType, "SetupAsWardenObjectiveSpecialCommand"));
            instance.Patch(gameType.GetMethod("SetupAsWardenObjectiveTerminalUplink"), null, new HarmonyMethod(patchType, "SetupAsWardenObjectiveTerminalUplink"));
            instance.Patch(gameType.GetMethod("SetupAsWardenObjectiveCorruptedTerminalUplink"), null, new HarmonyMethod(patchType, "SetupAsWardenObjectiveCorruptedTerminalUplink"));
            instance.Patch(gameType.GetMethod("ChangeState", new Type[] { typeof(TERM_State) }), null, new HarmonyMethod(patchType, "ChangeState"));
        }

        public static void SetupAsWardenObjectiveSpecialCommand(LG_ComputerTerminal __instance)
        {
            AIG_CourseNode node;
            if (__instance.SpawnNode != null) node = __instance.SpawnNode;
            else
            {
                L.Error($"Terminal {__instance.m_serialNumber} has a null spawn node!");
                node = LG_LevelBuilder.Current.m_currentFloor.allZones[0].m_courseNodes[0];
            }

            OnSetupSpecialCommandTerminal?.Invoke(__instance, node);
        }

        public static void SetupAsWardenObjectiveGatherTerminal(LG_ComputerTerminal __instance)
        {
            AIG_CourseNode node;
            if (__instance.SpawnNode != null) node = __instance.SpawnNode;
            else
            {
                L.Error($"Terminal {__instance.m_serialNumber} has a null spawn node!");
                node = LG_LevelBuilder.Current.m_currentFloor.allZones[0].m_courseNodes[0];
            }
            OnSetupTerminalGatherTerminal?.Invoke(__instance, node);
        }

        public static void SetupAsWardenObjectiveTerminalUplink(LG_ComputerTerminal __instance)
        {
            AIG_CourseNode node;
            if (__instance.SpawnNode != null) node = __instance.SpawnNode;
            else
            {
                L.Error($"Terminal {__instance.m_serialNumber} has a null spawn node!");
                node = LG_LevelBuilder.Current.m_currentFloor.allZones[0].m_courseNodes[0];
            }
            OnSetupUplinkTerminal?.Invoke(__instance, node);
        }

        public static void SetupAsWardenObjectiveCorruptedTerminalUplink(LG_ComputerTerminal __instance)
        {
            AIG_CourseNode node;
            if (__instance.SpawnNode != null) node = __instance.SpawnNode;
            else
            {
                L.Error($"Terminal {__instance.m_serialNumber} has a null spawn node!");
                node = LG_LevelBuilder.Current.m_currentFloor.allZones[0].m_courseNodes[0];
            }
            OnSetupCorruptedUplinkTerminal?.Invoke(__instance, node);
        }

        public static void ChangeState(LG_ComputerTerminal __instance, TERM_State state)
        {
            OnChangedState?.Invoke(__instance, state);
        }

        public static event Action<LG_ComputerTerminal, AIG_CourseNode> OnSetupTerminalGatherTerminal;
        public static event Action<LG_ComputerTerminal, AIG_CourseNode> OnSetupSpecialCommandTerminal;
        public static event Action<LG_ComputerTerminal, AIG_CourseNode> OnSetupUplinkTerminal;
        public static event Action<LG_ComputerTerminal, AIG_CourseNode> OnSetupCorruptedUplinkTerminal;
        public static event Action<LG_ComputerTerminal, TERM_State> OnChangedState;
    }
}