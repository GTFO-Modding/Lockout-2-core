using FX_EffectSystem;
using GTFO.API;
using Localization;
using Player;
using SNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Lockout_2_core.Custom_Player_Behavior
{
    public class PlayerDeathManager : MonoBehaviour
    {
        public PlayerDeathManager(IntPtr value) : base(value) { }
        public void Awake() 
        {
            Current = this; 
        }


        public void KillLocalPlayerAgent()
        {
            L.Debug("KillLocalPlayerAgent()");
            if (m_LocalPlayerDeathIncurred) return;
            m_LocalPlayerDeathIncurred = true;

            var player = PlayerManager.GetLocalPlayerAgent();
            var stateEnum = player.Locomotion.m_currentStateEnum;

            if (stateEnum != PlayerLocomotion.PLOC_State.Downed) player.Locomotion.ChangeState(PlayerLocomotion.PLOC_State.Downed);

            NetworkAPI.InvokeEvent("PlayerDeathDisableRevive", SNet.Slots.SlottedPlayers.IndexOf(player.Owner));
            SyncedDisableRevive(player);

            CoroutineHandler.Add(PlayerDeathRoutine(player));
            GuiManager.CrosshairLayer.SetVisible(false);
            PlayerDialogManager.WantToStartDialog(104U, player.CharacterID, false, true);
        }

        public IEnumerator PlayerDeathRoutine(PlayerAgent player)
        {
            var time = Time.time + 13;
            var glassCol = player.FPSCamera.HUDGlassShatter.m_material.GetColor("_GlassColor");
            var glassColPersistent = glassCol;

            while (time > Time.time)
            {
                glassCol -= new Color(0.01f, 0.04f, 0.045f, -0.0018f);
                player.FPSCamera.HUDGlassShatter.m_material.SetColor("_GlassColor", glassCol);
                player.FPSCamera.HUDGlassShatter.SetGlassShatterProgression(player.FPSCamera.HUDGlassShatter.m_currentProg + 0.08f);

                yield return null;
            }

            player.FPSCamera.HUDGlassShatter.m_material.SetColor("_GlassColor", glassColPersistent);
            Patch_FPSCamera_3RDPersonTest.ThirdPersonCamSetup();
            player.Sound.Stop();
            player.FPSCamera.ResetYawLimit();
            player.FPSCamera.ResetPitchLimit();

            NetworkAPI.InvokeEvent("PlayerDeathPostDeath", SNet.Slots.SlottedPlayers.IndexOf(player.Owner));
            SyncedPostDeath(player);
            EnvironmentStateManager.ResetFogSettingsLocal();

            player.AnimatorArms.gameObject.SetActive(false);
            player.AnimatorBody.gameObject.SetActive(false);
            PlayerManager.PlayerAgentsInLevel.Remove(player);
        }

        public void TriggerFX(Transform parent)
        {
            if (m_LinkedFX == null) return;

            var fxPool = FX_Manager.GetEffectPool(m_LinkedFX);
            m_SpawnedFX = fxPool.AquireEffect();
            m_SpawnedFX.Play(null, parent);
        }

        public void SyncedDisableRevive(PlayerAgent player)
        {
            L.Debug($"SyncedDisableRevive - PlayerAgent {player.Owner.NickName}");

            player.Sound.Post(2714892912);
            player.Sound.Post(992797716);

            player.ReviveInteraction.enabled = false;
            player.ReviveInteraction.SetBlocked(true);
            player.ReviveInteraction.SetActive(false);

            TriggerFX(player.TentacleTarget);

            if (!m_DeadPlayers.Contains(player)) m_DeadPlayers.Add(player);
            else L.Error("m_DeadPlayers already contains this player agent! Was the list cleaned up correctly?");

            if (!m_DeadPlayerIDs.Contains(player.CharacterID)) m_DeadPlayerIDs.Add(player.CharacterID);
            else L.Error("m_DeadPlayerIDs already contains this character ID! Was the list cleaned up correctly?");
        }

        public void SyncedPostDeath(PlayerAgent player)
        {
            L.Debug($"SyncedPostDeath - PlayerAgent {player.Owner.NickName}");

            if (!player.IsLocallyOwned)
            {
                L.Debug("Player is not locally owned - disabling the rig and the nav marker");
                player.RigSwitch.m_data.m_rootObject.SetActive(false);
                player.NavMarker.SetMarkerVisible(false);
            }

            foreach (var collider in player.GetComponentsInChildren<Collider>()) collider.enabled = false;

            PlayerManager.PlayerAgentsInLevel.Remove(player);
            if (Patch_FPSCamera_3RDPersonTest.s_IsThirdPerson && Patch_FPSCamera_3RDPersonTest.TargetPlayer == player)
            {
                L.Debug("Dead player is being spectated! Forcing GetNextTarget");
                Patch_FPSCamera_3RDPersonTest.GetNextTarget();
            }

            var text = new LocalizedText();
            text.UntranslatedText = $"Prisoner vital signs have been lost. Neural Interface disconnected.\n<color=orange><u>{player.Owner.NickName}</u>:// [KIA]</color>";
            text.Id = 0;

            GuiManager.PlayerLayer.m_gameEventLog.AddLogItem($"<color=#{ColorExt.ToHex(player.Owner.PlayerColor)}>{player.Owner.NickName}</color> <color=#700000><b>has died<b></color>");
            WardenObjectiveManager.DisplayWardenIntel(LevelGeneration.LG_LayerType.MainLayer, text);
        }

        public static void RecieveDisablePlayerRevive(ulong x, int slot)
        {
            L.Debug($"RecieveDisablePlayerRevive({x}, {slot})");

            SNet.Core.TryGetPlayer(x, out var sNetPlayer);
            var player = sNetPlayer.PlayerAgent.TryCast<PlayerAgent>();

            if (player == null)
            {
                L.Error("Dead player agent is null! this shouldnt happen! :(");
                return;
            }

            L.Debug($"dead player: {player}");

            Current.SyncedDisableRevive(player);
        }

        public static void RecievePostDeath(ulong x, int slot)
        {
            L.Debug($"RecievePostDeath({x}, {slot})");

            SNet.Core.TryGetPlayer(x, out var sNetPlayer);
            var player = sNetPlayer.PlayerAgent.TryCast<PlayerAgent>();

            if (player == null)
            {
                L.Error("Dead player agent is null! this shouldnt happen! :(");
                return;
            }

            L.Debug($"dead player: {player}");

            Current.SyncedPostDeath(player);
        }

        public bool m_LocalPlayerDeathIncurred = false;
        public List<PlayerAgent> m_DeadPlayers = new();
        public List<int> m_DeadPlayerIDs = new();
        public GameObject m_LinkedFX = AssetAPI.GetLoadedAsset("Assets/PrefabInstance/FXC_Blood_Arterial.prefab").TryCast<GameObject>();
        public FX_EffectBase_Poolable m_SpawnedFX;

        public static PlayerDeathManager Current;
    }
}
