using System;
using GameData;
using HarmonyLib;

namespace Lockout_2_core
{
    class Patch_WardenObjectiveManagerExecuteEvent
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(WardenObjectiveManager._ExcecuteEvent_d__148);
            var patchType = typeof(Patch_WardenObjectiveManagerExecuteEvent);

            instance.Patch(gameType.GetMethod("MoveNext"), null, new HarmonyMethod(patchType, "MoveNext"));
        }

        public static void MoveNext(WardenObjectiveManager._ExcecuteEvent_d__148 __instance)
        {
            /*
            if (RundownManager.ActiveExpedition.LevelLayoutData == 1001 && !Manager_CustomLevelBehavior.B1.m_EventProcessors.Contains(__instance))
            {
                if (Manager_CustomLevelBehavior.B1.m_MissionComplete)
                {
                    L.Debug("Objective is complete. ienumerator genocide. This is so sad");
                    __instance.System_IDisposable_Dispose();
                }
                else Manager_CustomLevelBehavior.B1.m_EventProcessors.Add(__instance);
            }
            */
            if (__instance.__1__state != -1) return;
            OnExecuteEvent?.Invoke(__instance.eData);
        }

        public static event Action<WardenObjectiveEventData> OnExecuteEvent;
    }
}