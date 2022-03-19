using System;
using GTFO.API;
using HarmonyLib;
using LevelGeneration;
using Lockout_2_core.Custom_Level_Behavior;

namespace Lockout_2_core
{
    class Patch_LG_WardenObjective_Reactor
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LG_WardenObjective_Reactor);
            var patchType = typeof(Patch_LG_WardenObjective_Reactor);

            instance.Patch(gameType.GetMethod("AttemptInteract", new Type[] { typeof(pReactorInteraction)}), null, new HarmonyMethod(patchType, "AttemptInteract"));
        }

        public static void AttemptInteract(pReactorInteraction interaction)
        {
            NetworkAPI.InvokeEvent("OnAttemptInteractReactor", interaction);
            OnAttemptInteract(0, interaction);
        }

        public static void OnAttemptInteract(ulong x, pReactorInteraction interaction)
        {
            OnReactorInteraction?.Invoke(interaction.type);
        }

        public static event Action<eReactorInteraction> OnReactorInteraction;
    }
}