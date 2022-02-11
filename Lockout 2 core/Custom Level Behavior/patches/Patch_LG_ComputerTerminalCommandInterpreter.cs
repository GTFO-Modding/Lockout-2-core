using System;
using System.Collections.Generic;
using AIGraph;
using GameData;
using HarmonyLib;
using LevelGeneration;

namespace Lockout_2_core
{
    class Patch_LG_ComputerTerminalCommandInterpreter
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_ComputerTerminalCommandInterpreter);
            var patchType = typeof(Patch_LG_ComputerTerminalCommandInterpreter);

            instance.Patch(gameType.GetMethod("StartTerminalUplinkSequence"), null, new HarmonyMethod(patchType, "StartTerminalUplinkSequence"));
            instance.Patch(gameType.GetMethod("ReceiveCommand"), null, new HarmonyMethod(patchType, "ReceiveCommand"));
            instance.Patch(gameType.GetMethod("TryGetCommand"), null, new HarmonyMethod(patchType, "TryGetCommand"));
        }

        public static void StartTerminalUplinkSequence(LG_ComputerTerminalCommandInterpreter __instance)
        {
            kindaScary.Add(__instance.Pointer, __instance.OnEndOfQueue);

            __instance.OnEndOfQueue = (Action)(() => {
                L.Error("Kinda scary! Invoking OnEndOfQueue");
                kindaScary[__instance.Pointer].Invoke();

                L.Error("Getting active warden objective");
                var objective = WardenObjectiveManager.ActiveWardenObjective(__instance.m_terminal.SpawnNode.LayerType);

                L.Error("Attempting to trigger events on activate");
                WardenObjectiveManager.CheckAndExecuteEventsOnTrigger
                (
                    objective.EventsOnActivate,
                    GameData.eWardenObjectiveEventTrigger.OnStart,
                    true
                );
                L.Error("Networking hotfix");
                kindaScary.Clear();
            });
        }

        public static void ReceiveCommand(LG_ComputerTerminalCommandInterpreter __instance, TERM_Command cmd)
        {
            if (cmd != TERM_Command.WardenObjectiveGatherCommand) return;

            L.Debug($"Terminal {__instance.m_terminal.m_serialNumber} gathered!");
            OnGatherTerminal.Invoke(__instance.m_terminal, __instance.m_terminal.SpawnNode);
        }

        public static void TryGetCommand(LG_ComputerTerminalCommandInterpreter __instance, ref TERM_Command command, ref string param1, ref string param2, ref bool __result)
        {
            if (!__result) return;
            RecieveCommand.Invoke(__instance.m_terminal, command, param1, param2);
        }

        public static Dictionary<IntPtr, Il2CppSystem.Action> kindaScary = new();

        public static event Action<LG_ComputerTerminal, AIG_CourseNode> OnGatherTerminal;
        public static event Action<LG_ComputerTerminal, TERM_Command, string, string> RecieveCommand;
    }
}