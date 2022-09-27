using System;
using GameData;
using HarmonyLib;
using LevelGeneration;
using Localization;
using Lockout_2_core.Custom_Level_Behavior;
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
            instance.Patch(gameType.GetMethod("OnLocalPlayerSolvedObjectiveItem"), new HarmonyMethod(patchType, "PreOnLocalPlayerSolvedObjectiveItem"));
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

        public static bool PreOnLocalPlayerSolvedObjectiveItem()
        {
            return (s_AllowObjectiveComplete);
        }

        public static void OnLocalPlayerSolvedObjectiveItem()
        {
            OnSolvedObjectiveItem?.Invoke();
        }

        public static void CheckAndExecuteEventsOnTrigger(WardenObjectiveEventData eventToTrigger)
        {
            switch((int)eventToTrigger.Type)
            {
                case 13: Event_LockDoor_B3(eventToTrigger); break;
                case 14: Event_LockDoor_F1(eventToTrigger); break;
                case 15: Event_AddNavBeaconToBigPickups_F1(eventToTrigger); break;

                default: return;
            }
        }

        public static void Event_LockDoor_B3(WardenObjectiveEventData eventData)
        {
            if (!SNet.IsMaster) return;
            L.Debug("Custom Event 13: Setting security door to Locked!");

            if (!LG_LevelBuilder.Current.m_currentFloor.TryGetZoneByLocalIndex(eventData.DimensionIndex, eventData.Layer, eventData.LocalIndex, out LG_Zone zone))
            {
                L.Error($"Failed to find zone! Please double check your event data!\nDimensionIndex {eventData.DimensionIndex}\nLayer {eventData.Layer}\nLocalIndex to int {(int)eventData.LocalIndex}");
                return;
            }

            var zoneDoor = zone.m_sourceGate.SpawnedDoor.TryCast<LG_SecurityDoor>();
            if (zoneDoor == null)
            {
                L.Error($"Security door for zone is null! How the fuck did you manage that?\nDimensionIndex {eventData.DimensionIndex}\nLayer {eventData.Layer}\nLocalIndex to int {(int)eventData.LocalIndex}");
                return;
            }

            zoneDoor.SetupAsLockedNoKey(LocalizerGenocideReal.GenerateLocalizedText("<color=red>://ERROR: Only one Data Hub wing is accessable at a time.</color>"));
        }
        public static void Event_LockDoor_F1(WardenObjectiveEventData eventData)
        {
            if (!SNet.IsMaster) return;
            L.Debug("Custom Event 14: Closing security door and setting to Locked!");

            if (!LG_LevelBuilder.Current.m_currentFloor.TryGetZoneByLocalIndex(eventData.DimensionIndex, eventData.Layer, eventData.LocalIndex, out LG_Zone zone))
            {
                L.Error($"Failed to find zone! Please double check your event data!\nDimensionIndex {eventData.DimensionIndex}\nLayer {eventData.Layer}\nLocalIndex to int {(int)eventData.LocalIndex}");
                return;
            }

            var zoneDoor = zone.m_sourceGate.SpawnedDoor.TryCast<LG_SecurityDoor>();
            if (zoneDoor == null)
            {
                L.Error($"Security door for zone is null! How the fuck did you manage that?\nDimensionIndex {eventData.DimensionIndex}\nLayer {eventData.Layer}\nLocalIndex to int {(int)eventData.LocalIndex}");
                return;
            }

            zoneDoor.m_sync.AttemptDoorInteraction(eDoorInteractionType.Close);
            zoneDoor.SetupAsLockedNoKey(LocalizerGenocideReal.GenerateLocalizedText($"<color=orange>ZONE {(int)eventData.LocalIndex} IN EMERGENCY LOCKDOWN.\n<u><color=orange>TYPE-F</u> BIOMASS CONTAINMENT BREACH DETECTED.\n CONTACT HAZARDOUS ENVIRONMENT CONTAINMENT UNIT IMMEDIATELY</color>"));
        }
        public static void Event_AddNavBeaconToBigPickups_F1(WardenObjectiveEventData eventData)
        {
            L.Debug("Adding nav beacons to carry items in zone");

            if (!LG_LevelBuilder.Current.m_currentFloor.TryGetZoneByLocalIndex(eventData.DimensionIndex, eventData.Layer, eventData.LocalIndex, out var zone))
            {
                L.Error($"Failed to find zone! Please double check your event data!\nDimensionIndex {eventData.DimensionIndex}\nLayer {eventData.Layer}\nLocalIndex to int {(int)eventData.LocalIndex}");
                return;
            }

            foreach (var area in zone.m_areas) foreach (var carryItem in area.GetComponentsInChildren<CarryItemPickup_Core>()) carryItem.m_navMarkerPlacer.SetMarkerVisible(true);
        }

        public static bool s_AllowObjectiveComplete = true;

        public static event Action OnObjectiveComplete;
        public static event Action OnStartExpedition;
        public static event Action OnSolvedObjectiveItem;
    }
}