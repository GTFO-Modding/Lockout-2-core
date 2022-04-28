using System;
using GameData;
using HarmonyLib;
using LevelGeneration;
using SNetwork;

namespace Lockout_2_core
{
    class Patch_EnemyCostManager
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(EnemyCostManager);
            var patchType = typeof(Patch_EnemyCostManager);

            instance.Patch(gameType.GetMethod("AddCost", new Type[] { typeof(eDimensionIndex), typeof(float) }), new HarmonyMethod(patchType, "AddCost"));
        }

        public static void AddCost(ref float points)
        {
            if (s_MultiplyCost) points *= s_Multi;
        }

        public static bool s_MultiplyCost = false;
        public static float s_Multi = 1;
    }
}