using System;
using HarmonyLib;
using LevelGeneration;

namespace Lockout_2_core
{
    class Patch_LG_ComputerTerminalManager
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_ComputerTerminalManager);
            var patchType = typeof(Patch_LG_ComputerTerminalManager);

            instance.Patch(gameType.GetMethod("WantToSendTerminalCommand"), null, new HarmonyMethod(patchType, "WantToSendTerminalCommand"));
        }

        public static void WantToSendTerminalCommand(TERM_Command command)
        {
            if (command == TERM_Command.UniqueCommand1 || command == TERM_Command.UniqueCommand2 || command == TERM_Command.UniqueCommand3 || command == TERM_Command.UniqueCommand4 || command == TERM_Command.UniqueCommand5)
                OnReceiveCustomCommand?.Invoke();
        }

        public static event Action OnReceiveCustomCommand;
    }
}