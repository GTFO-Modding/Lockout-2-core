using HarmonyLib;
using LevelGeneration;
using Lockout_2_core.Custom_Level_Behavior;

namespace Lockout_2_core
{
    class Patch_LG_WardenObjective_HSUActivator_Room
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_WardenObjective_HSUActivator_Room);
            var patchType = typeof(Patch_LG_WardenObjective_HSUActivator_Room);

            instance.Patch(gameType.GetMethod("OnLateBuildJob"), new HarmonyMethod(patchType, "OnLateBuildJob"));
        }

        public static bool OnLateBuildJob(LG_WardenObjective_HSUActivator_Room __instance)
        {
            if (RundownManager.ActiveExpedition.LevelLayoutData != 1001) return true;

            var existingObjective = WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).Type;

            WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).Type = eWardenObjectiveType.ActivateSmallHSU;
            Manager_CustomLevelBehavior.B1.BuildCustomLevelContent();
            WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).Type = existingObjective;

            return false;
        }
    }
}