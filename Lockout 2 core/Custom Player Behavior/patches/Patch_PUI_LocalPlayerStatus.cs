using HarmonyLib;

namespace Lockout_2_core
{
    class Patch_PUI_LocalPlayerStatus
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(PUI_LocalPlayerStatus);
            var patchType = typeof(Patch_PUI_LocalPlayerStatus);

            instance.Patch(gameType.GetMethod("UpdateStamina"), new HarmonyMethod(patchType, "UpdateStamina"));
        }

        public static bool UpdateStamina(PUI_LocalPlayerStatus __instance, float stamina)
        {
            __instance.UpdateBPM(stamina);
            return false;
        }
    }
}