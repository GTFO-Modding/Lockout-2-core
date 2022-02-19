using System;
using HarmonyLib;
using LevelGeneration;
using Lockout_2_core.Custom_Level_Behavior;

namespace Lockout_2_core
{
    class Patch_LG_TERM_ReactorError
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_TERM_ReactorError);
            var patchType = typeof(Patch_LG_TERM_ReactorError);

            instance.Patch(gameType.GetMethod("Exit"), null, new HarmonyMethod(patchType, "Exit"));
        }

        public static void Exit(LG_TERM_ReactorError __instance)
        {
            OnExitTerminalReactorError?.Invoke(__instance.m_terminal);
        }

        public static event Action<LG_ComputerTerminal> OnExitTerminalReactorError;
    }
}