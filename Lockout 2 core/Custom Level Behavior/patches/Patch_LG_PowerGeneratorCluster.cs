using HarmonyLib;
using LevelGeneration;

namespace Lockout_2_core
{
    class Patch_LG_PowerGeneratorCluster
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_PowerGeneratorCluster);
            var patchType = typeof(Patch_LG_PowerGeneratorCluster);

            instance.Patch(gameType.GetMethod("Setup"), new HarmonyMethod(patchType, "PreSetup"));
            instance.Patch(gameType.GetMethod("Setup"), null, new HarmonyMethod(patchType, "PostSetup"));
            instance.Patch(gameType.GetMethod("OnBuildDone"), new HarmonyMethod(patchType, "PreOnBuildDone"));
            instance.Patch(gameType.GetMethod("OnBuildDone"), null, new HarmonyMethod(patchType, "PostOnBuildDone"));
        }

        public static void PreSetup()
        {
            if (RundownManager.Current.m_activeExpedition.LevelLayoutData != 1004) return;
            WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).Type = eWardenObjectiveType.CentralGeneratorCluster;
        }

        public static void PostSetup()
        {
            if (RundownManager.Current.m_activeExpedition.LevelLayoutData != 1004) return;
            WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).Type = eWardenObjectiveType.GatherSmallItems;
        }

        public static void PreOnBuildDone()
        {
            if (RundownManager.Current.m_activeExpedition.LevelLayoutData != 1008) return;
            WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).Type = eWardenObjectiveType.CentralGeneratorCluster;
        }

        public static void PostOnBuildDone()
        {
            if (RundownManager.Current.m_activeExpedition.LevelLayoutData != 1008) return;
            WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).Type = eWardenObjectiveType.SpecialTerminalCommand;
        }
    }
}