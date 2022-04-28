using BepInEx;
using BepInEx.IL2CPP;
using GTFO.API;
using HarmonyLib;
using Lockout_2_core.Custom_Weapon_code;
using UnhollowerRuntimeLib;
using Il2CppSystem;
using Lockout_2_core.Custom_Tools;
using UnityEngine;
using BepInEx.Configuration;
using Lockout_2_core.Custom_Player_Behavior;
using Lockout_2_core.Custom_Level_Behavior;
using AssetShards;
using Lockout_2_core.Custom_Enemies;
using System.Collections.Generic;

namespace Lockout_2_core
{
    [BepInPlugin("com.Mccad.Lockout2Core", "Mccad.Lockout2Core", "1.0.0")]
    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    public class EntryPoint : BasePlugin
    {
        // The method that gets called when BepInEx tries to load our plugin
        public override void Load()
        {
            m_Harmony = new Harmony("com.Mccad.Lockout2Core");
            AssetAPI.OnAssetBundlesLoaded += RegisterTypes;
            AssetAPI.OnAssetBundlesLoaded += AssetAPI_OnAssetBundlesLoaded;
            AssetAPI.OnStartupAssetsLoaded += AssetAPI_OnStartupAssetsLoaded;
            RundownManager.add_OnExpeditionGameplayStarted((Action)DisableStamina.Setup);

            //Asset bundle scripts
            ClassInjector.RegisterTypeInIl2Cpp<ForcedGenClusterSetup>();

            SetupConfig(base.Config);

            //custom weapon code
            Patch_BulletWeapon.Inject(m_Harmony);
            Patch_GrenadeBase.Inject(m_Harmony);
            Patch_GlowstickInstance.Inject(m_Harmony);
            Patch_FPSCamera.Inject(m_Harmony);
            Patch_Weapon.Inject(m_Harmony);
            Patch_ItemEquippable.Inject(m_Harmony);

            //custom tools
            Patch_PlayerSync.Inject(m_Harmony);
            Patch_CM_ScrollWindowInfoBox.Inject(m_Harmony);
            Patch_LocalPlayerAgentSettings.Inject(m_Harmony);
            Patch_GS_AfterLevel.Inject(m_Harmony);

            //custom rundown page
            //Patch_RundownManager.Inject(m_Harmony); Patch is obsolete. Basegame allows this to be bypassed through datablocks now
            Patch_CM_ExpeditionIcon_New.Inject(m_Harmony);
            Patch_PUI_GameObjectives.Inject(m_Harmony);
            Patch_CM_PageMap.Inject(m_Harmony);
            Patch_CM_PageExpeditionSuccess.Inject(m_Harmony);

            //custom player behavior
            Patch_PUI_LocalPlayerStatus.Inject(m_Harmony);
            Patch_FPSCamera_3RDPersonTest.Inject(m_Harmony);
            Patch_PlayerAgent.Inject(m_Harmony);
            Patch_PLOC_Downed.Inject(m_Harmony);
            Patch_PlayerDialogManager.Inject(m_Harmony);

            //custom level behavior
            Patch_WardenObjectiveManager.Inject(m_Harmony);
            Patch_WardenObjectiveManagerExecuteEvent.Inject(m_Harmony);
            Patch_GS_StopElevatorRide.Inject(m_Harmony);
            Patch_LG_DoorButton.Inject(m_Harmony);
            Patch_LG_WardenObjective_HSUActivator_Room.Inject(m_Harmony);
            Patch_LG_HSUActivator_Core.Inject(m_Harmony);
            Patch_LG_ComputerTerminalManager.Inject(m_Harmony);
            Patch_LG_ComputerTerminalCommandInterpreter.Inject(m_Harmony);
            Patch_LG_ComputerTerminal.Inject(m_Harmony);
            Patch_LG_TERM_ReactorError.Inject(m_Harmony);
            Patch_LG_AlarmShutdownOnTerminalJob.Inject(m_Harmony);
            Patch_LG_PowerGeneratorCluster.Inject(m_Harmony);
            Patch_LG_PowerGeneratorClusterObjectiveEndSequence.Inject(m_Harmony);
            Patch_LG_LevelExitGeo.Inject(m_Harmony);
            Patch_LG_PopulateFunctionMarkersInZoneJob.Inject(m_Harmony);
            Patch_LG_WardenObjective_Reactor.Inject(m_Harmony);
            Patch_LG_LateGeomorphScanJob.Inject(m_Harmony);
            Patch_EnemyCostManager.Inject(m_Harmony);

            //custom enemies
            Patch_ES_HibernateWakeup.Inject(m_Harmony);
            Patch_ES_HitReact.Inject(m_Harmony);
            Patch_InfectionSpitter.Inject(m_Harmony);

            //custom networking
            CustomNetworking.RegisterPackets();

            //Coroutines (deprecate this shit eventually)
            CoroutineHandler.Init();



            L.Debug("This rundown is dedicated to Dex. Hey there buddy, wherever you are, thanks for everything. Through good times and bad times you were always willing to lend me a hand, even when I was a hardass you would always work through my bullshit and help me get things done. I couldn't have done any of this without you. I'm gonna miss you, man. Wish you were here.");
        }

        private void AssetAPI_OnStartupAssetsLoaded()
        {
            //VaultSecurityDoorPlug.Setup(AssetAPI.GetLoadedAsset("Assets/Bundle/SecDoor_8x8/Content/8x8DoorPlug.prefab").TryCast<GameObject>());
        }

        private static void AssetAPI_OnAssetBundlesLoaded()
        {
            PrefabAPI.CreateConsumable("assets/bundle/flashbang/content/consumable_flashbang.prefab");
            PrefabAPI.CreateConsumablePickup("assets/bundle/flashbang/content/consumable_flashbang_pickup.prefab");
            PrefabAPI.CreateConsumableInstance<Item_FlashGrenade_Throwable>("assets/bundle/flashbang/content/consumable_flashbang_instance.prefab");
            var flower = AssetAPI.GetLoadedAsset("Assets/Bundle/VenusWeed/content/flower.prefab").TryCast<GameObject>();
            var rose = flower.transform.FindChild("rose").gameObject.AddComponent<Manager_VenusWeed_Rose>();

            PrefabAPI.CreateGearComponent("Assets/Bundle/axe/content/Head_Axe.prefab", true);
            PrefabAPI.CreateGearComponent("ASSETS/BUNDLE/NIGHTVISION/CONTENT/NIGHTVISION.PREFAB", true);

            s_Captchas.Add("2JHqR5 5nGeua".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/2JHqR5 5nGeua.png").TryCast<Texture2D>());
            s_Captchas.Add("4NrcdM H7qxZZ".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/4NredM H7qxZZ.png").TryCast<Texture2D>());
            s_Captchas.Add("b6JvPI 8TS0i2".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/b6JvPI 8TS0i2.png").TryCast<Texture2D>());
            s_Captchas.Add("DE6hcC fFsImP".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/DE6hcC fFsImP.png").TryCast<Texture2D>());
            s_Captchas.Add("DQeYQQ 9LXJS5".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/DQeYQQ 9LXJSs.png").TryCast<Texture2D>());
            s_Captchas.Add("gKnLAB Wdca2d".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/gKnLAB Wdca2d.png").TryCast<Texture2D>());
            s_Captchas.Add("IWvUOZ QXCFqk".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/IWvUOZ QXCFqk.png").TryCast<Texture2D>());
            s_Captchas.Add("jofP3W K34mjW".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/jofF3W K34mjW.png").TryCast<Texture2D>());
            s_Captchas.Add("LorFfD LEdP9C".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/LorFfD LEdP9C.png").TryCast<Texture2D>());
            s_Captchas.Add("MIBnIC hCKt2Z".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/MIBnIC hCKt2Z.png").TryCast<Texture2D>());
            s_Captchas.Add("Nr9wbP Jc325C".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/Nr9wbP Jc325C.png").TryCast<Texture2D>());
            s_Captchas.Add("ojHIJU oGiuSu".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/ojHIJU oGiuSu.png").TryCast<Texture2D>());
            s_Captchas.Add("otKeEM g762JN".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/otKeEM g762JN.png").TryCast<Texture2D>());
            s_Captchas.Add("PhMofw tW7GzE".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/PhMofw tW7GzE.png").TryCast<Texture2D>());
            s_Captchas.Add("qRKdGM gRdHsJ".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/qRKdGM gRdHsJ.png").TryCast<Texture2D>());
            s_Captchas.Add("QvpKSM b9Zyz8".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/QvpKSM b9Zyz8.png").TryCast<Texture2D>());
            s_Captchas.Add("SE9swU LRogha".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/SE9swU LRogha.png").TryCast<Texture2D>());
            s_Captchas.Add("sU9Ar2 vm8TCR".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/sU9Ar2 vm8TCR.png").TryCast<Texture2D>());
            s_Captchas.Add("wBofIB iLdQLE".ToUpper(), AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/wBofIB iLdQLE.png").TryCast<Texture2D>());

            //VaultSecurityDoor.Setup(AssetAPI.GetLoadedAsset("Assets/Bundle/SecDoor_8x8/Content/SecurityDoor_8x8.prefab").TryCast<GameObject>());
            ReactorHSUPlatform.Setup(AssetAPI.GetLoadedAsset("Assets/Bundle/reactorCoolantPlatform/content/E1_CoolantHSU.prefab").TryCast<GameObject>());

            //Todo: Remove this shit! we should be doing this with unity but idk what's going wrong
            var genCluster = AssetAPI.GetLoadedAsset("Assets/AssetBundles/CustomGeomorphs/CustomRefineryHub/Content/geo_RefineryHub_HSU_MC_01.prefab").TryCast<GameObject>().GetComponentInChildren<LG_PowerGeneratorCluster>();
            genCluster.gameObject.AddComponent<ForcedGenClusterSetup>().m_GenCluster = genCluster;
        }

        private static void RegisterTypes()
        {
            ClassInjector.RegisterTypeInIl2Cpp<Manager_FlashbangBlinder>();
            ClassInjector.RegisterTypeInIl2Cpp<Manager_NightVision>();
            ClassInjector.RegisterTypeInIl2Cpp<Item_GrenadeLauncher_Projectile>();
            ClassInjector.RegisterTypeInIl2Cpp<Manager_WeaponAutoAim>();
            ClassInjector.RegisterTypeInIl2Cpp<Manager_VenusWeed_Rose>();
            ClassInjector.RegisterTypeInIl2Cpp<E1_Old_Friends>();
            ClassInjector.RegisterTypeInIl2Cpp<ThirdPersonCamTarget>();
            ClassInjector.RegisterTypeInIl2Cpp<LG_LightFadeAnimator>();
            ClassInjector.RegisterTypeInIl2Cpp<PlayerDeathManager>();
            var playerDeathManager = new GameObject("PlayerDeathManager");
            playerDeathManager.AddComponent<PlayerDeathManager>();

            Manager_CustomLevelBehavior.situp();
        }

        private static void SetupConfig(ConfigFile config)
        {
            NightVisionKey = config.Bind("Keybinds", "Toggle Night Vision", KeyCode.N);
        }

        private Harmony m_Harmony;
        public static ConfigEntry<KeyCode> NightVisionKey;

        public static Dictionary<string, Texture2D> s_Captchas = new();
    }
}
