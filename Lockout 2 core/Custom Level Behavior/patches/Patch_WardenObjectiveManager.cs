using System;
using GameData;
using HarmonyLib;
using LevelGeneration;
using SNetwork;

namespace Lockout_2_core
{
    class Patch_WardenObjectiveManager
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(WardenObjectiveManager);
            var patchType = typeof(Patch_WardenObjectiveManager);

            instance.Patch(gameType.GetMethod("ActivateWinCondition"), null, new HarmonyMethod(patchType, "ActivateWinCondition"));
            instance.Patch(gameType.GetMethod("OnLocalPlayerStartExpedition"), null, new HarmonyMethod(patchType, "OnLocalPlayerStartExpedition"));
            instance.Patch(gameType.GetMethod("OnLocalPlayerSolvedObjectiveItem"), null, new HarmonyMethod(patchType, "OnLocalPlayerSolvedObjectiveItem"));
            instance.Patch(gameType.GetMethod("CheckAndExecuteEventsOnTrigger", new Type[] { typeof(WardenObjectiveEventData), typeof(eWardenObjectiveEventTrigger), typeof(bool), typeof(float) }), null, new HarmonyMethod(patchType, "CheckAndExecuteEventsOnTrigger"));
        }

        public static void ActivateWinCondition()
        {
            OnObjectiveComplete?.Invoke();
        }

        public static void OnLocalPlayerStartExpedition()
        {
            OnStartExpedition?.Invoke();
        }

        public static void OnLocalPlayerSolvedObjectiveItem()
        {
            OnSolvedObjectiveItem?.Invoke();
        }

        public static void CheckAndExecuteEventsOnTrigger(WardenObjectiveEventData eventToTrigger)
        {
            if ((int)eventToTrigger.Type != 13) return;
            L.Debug("Wow, this is a custom event type! Setting security door to locked");

            LG_Zone lg_Zone;
            if (SNet.IsMaster && Builder.Current.m_currentFloor.TryGetZoneByLocalIndex(eventToTrigger.DimensionIndex, eventToTrigger.Layer, eventToTrigger.LocalIndex, out lg_Zone))
            {
                LG_SecurityDoor lg_SecurityDoor = lg_Zone.m_sourceGate.SpawnedDoor.TryCast<LG_SecurityDoor>();
                if (lg_SecurityDoor == null)
                {
                    L.Error("Sec door was null :(");
                    return;
                }
                lg_SecurityDoor.SetupAsLockedNoKey("<color=red>://ERROR: Only one Data Hub wing is accessable at a time.</color>");
            }
        }

        public static event Action OnObjectiveComplete;
        public static event Action OnStartExpedition;
        public static event Action OnSolvedObjectiveItem;
    }
}