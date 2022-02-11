using System;
using System.Collections.Generic;
using AIGraph;
using GameData;
using HarmonyLib;
using LevelGeneration;
using Lockout_2_core.Custom_Level_Behavior;

namespace Lockout_2_core
{
    class Patch_LG_LevelExitGeo
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_LevelExitGeo);
            var patchType = typeof(Patch_LG_LevelExitGeo);

            instance.Patch(gameType.GetMethod("ActivateWinCondition"), new HarmonyMethod(patchType, "ActivateWinCondition"));
        }

        public static bool ActivateWinCondition(LG_LevelExitGeo __instance)
        {
            if (RundownManager.ActiveExpedition.LevelLayoutData != 1004) return true;
            Manager_CustomLevelBehavior.C1.m_ExitGeo = __instance;

            if (!Manager_CustomLevelBehavior.C1.m_AllowExtraction) return false;

            return true;
        }
    }
}