using HarmonyLib;
using Lockout_2_core.Custom_Tools;
using Player;

namespace Lockout_2_core
{
    class Patch_LocalPlayerAgentSettings
    {
        public static void Inject(Harmony instance)
        {
            var gameType = typeof(LocalPlayerAgentSettings);
            var patchType = typeof(Patch_LocalPlayerAgentSettings);

            instance.Patch(gameType.GetMethod("UpdateBlendTowardsTargetFogSetting"), new HarmonyMethod(patchType, "UpdateBlendTowardsTargetFogSettingPre"));
            instance.Patch(gameType.GetMethod("UpdateBlendTowardsTargetFogSetting"), null, new HarmonyMethod(patchType, "UpdateBlendTowardsTargetFogSettingPost"));
        }

        public static void UpdateBlendTowardsTargetFogSettingPre()
        {
            if (!Manager_NightVision.Current.NightVisionActive) return;

            L.Debug("Prefix: Setting the current VFX to default");
            Manager_NightVision.Current.AssignVFX(Manager_NightVision.Current.VFX_Default);
        }

        public static void UpdateBlendTowardsTargetFogSettingPost(LocalPlayerAgentSettings __instance)
        {
            if (!Manager_NightVision.Current.NightVisionEquipped) return;

            L.Debug($"Postfix: Assigning the current fog color to VFX_Default\n{Manager_NightVision.Current.VFX_Default.PrelitVolumeColor} -> {PlayerManager.Current.m_localPlayerAgentInLevel.FPSCamera.PrelitVolume.m_fogColor}");
            Manager_NightVision.Current.VFX_Default.PrelitVolumeColor = PlayerManager.Current.m_localPlayerAgentInLevel.FPSCamera.PrelitVolume.m_fogColor;

            if (Manager_NightVision.Current.NightVisionActive)
            {
                L.Debug("Postfix: Nightvision is on, setting the VFX to NVG");
                Manager_NightVision.Current.AssignVFX(Manager_NightVision.Current.VFX_NVG);
            }
        }
    }
}