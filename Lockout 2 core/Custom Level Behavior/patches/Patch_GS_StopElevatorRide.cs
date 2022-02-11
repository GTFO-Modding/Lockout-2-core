using System;
using HarmonyLib;

namespace Lockout_2_core
{
    class Patch_GS_StopElevatorRide
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(GS_StopElevatorRide);
            var patchType = typeof(Patch_GS_StopElevatorRide);

            instance.Patch(gameType.GetMethod("Enter"), null,  new HarmonyMethod(patchType, "Enter"));
        }

        public static void Enter()
        {
            OnStopElevatorRide?.Invoke();
        }

        public static event Action OnStopElevatorRide;
    }
}