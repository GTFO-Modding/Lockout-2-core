using Agents;
using Enemies;
using GTFO.API;
using IRF;
using Lockout_2_core.Custom_Level_Behavior;
using SNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockout_2_core.Custom_Enemies
{
    class Manager_VenusWeed_Rose : MonoBehaviour
    {
        public Manager_VenusWeed_Rose(IntPtr value) : base(value)
        { }

        public void Awake()
        {
            Current = this;
            m_petalAnimators = new List<Animator>();
            m_Light = transform.parent.GetComponentInChildren<Light>();
            m_Sound = new();

            var animators = GetComponentsInChildren<Animator>();
            foreach (var petal in animators) m_petalAnimators.Add(petal);

            Manager_CustomLevelBehavior.D1.Boss = this;
        }

        public void SyncTryOpen()
        {
            if (m_StateTarget != roseAnimation.Shut || !m_Owner.Alive) return;
            m_Owner.Locomotion.ScoutScream.ActivateState(m_Owner.AI.Target);

            Open();
        }
        public void SyncTryClose()
        {
            if (m_StateTarget != roseAnimation.Open || !m_Owner.Alive) return;

            Shut();
            CoroutineHandler.Add(OpenShell(20));
        }

        public static void RecieveAnimPacket(ulong x, pAnimation anim)
        {
            if (SNet.IsMaster) return;
            switch(anim.m_TargetState)
            {
                case roseAnimation.Open: Current.PlayAnim(roseAnimation.Open, 0.03f, true); break;
                case roseAnimation.Shut: Current.PlayAnim(roseAnimation.Shut, 0.08f, false); break;
            }
        }

        public void Open()
        {
            PlayAnim(roseAnimation.Open, 0.03f, true);
            if (SNet.IsMaster) NetworkAPI.InvokeEvent("RoseBossSyncAnimationState", new pAnimation() { m_TargetState = roseAnimation.Open});
        }

        public void Shut()
        {
            PlayAnim(roseAnimation.Shut, 0.08f, false);
            if (SNet.IsMaster) NetworkAPI.InvokeEvent("RoseBossSyncAnimationState", new pAnimation() { m_TargetState = roseAnimation.Shut});
        }

        public void Update()
        {
            if (m_Owner != null)
            {
                if (!m_Owner.Alive)
                {
                    if (m_OwnerIsAlive)
                    {
                        OnDead?.Invoke();
                        m_Light.intensity += 10;
                        m_Light.range += 15;
                        m_OwnerIsAlive = false;
                    }

                    if (m_State == roseAnimation.Shut && m_StateTarget != roseAnimation.Open) Open();
                    if (m_State == roseAnimation.Open && m_StateTarget != roseAnimation.Wither)
                    {
                        PlayAnim(roseAnimation.Wither, 0.2f, true);
                        m_Light.intensity = 3;
                        m_Owner.Sound.Post(AK.EVENTS.SCOUT_DEATH_SCREAM);
                        m_OwnerTentacles.gameObject.active = false;
                    }
                }
                else m_Owner.transform.position = m_OwnerPosition;
            }

            if (!m_OwnerIsAlive)
            {
                if (m_Light.intensity > 0 && !m_OwnerIsAlive)
                {
                    m_Light.intensity -= 0.02f;
                    m_Light.range -= 0.05f;
                }
                else enabled = false;
            }
            
        }

        public void Setup()
        {
            transform.parent.gameObject.active = true;
            m_Owner = transform.parent.parent.parent.GetComponent<EnemyAgent>();
            m_OwnerTentacles = m_Owner.transform.FindChild("siren.prefab/root/Hips/Spine/Chest/Neck/Head/Tentacles(Clone)").GetComponent<InstancedRenderFeature>();
            m_OwnerTentacles.gameObject.active = true;
            m_OwnerPosition = new(m_Owner.transform.position.x, 1.5f, m_Owner.transform.position.z);

            transform.parent.parent = null;

            L.Debug("Flower unparented from base enemy successfully");
        }

        public void PlayAnim(roseAnimation animation = roseAnimation.Open, float petalTimeOffset = 0, bool reverseOrder = false)
        {
            m_StateTarget = animation;
            string anim = animation.ToString();

            for (var i = 0; i < m_petalAnimators.Count; i++)
            {
                CoroutineHandler.Add(PlayAnim(anim, petalTimeOffset, i, reverseOrder));
            }
        }

        public IEnumerator PlayAnim(string animation, float petalTimeOffset, int petalIndex, bool reverseOrder)
        {
            float timeDelay;
            if (!reverseOrder) timeDelay = petalIndex * petalTimeOffset;
            else timeDelay = ((m_petalAnimators.Count + 1) - petalIndex) * petalTimeOffset;

            yield return new WaitForSeconds(timeDelay);
            m_petalAnimators[petalIndex].SetTrigger(animation);
            m_State = (roseAnimation)Enum.Parse(typeof(roseAnimation), animation);
        }

        public IEnumerator OpenShell(float timeDelay)
        {
            yield return new WaitForSeconds(timeDelay * 0.25f);
            EnvironmentStateManager.AttemptLightningStrike(Vector3.down, new(0.5f, 0.1f, 0.025f, 1));
            ProjectileShotgun(25, ProjectileType.TargetingSmall, m_Owner.AI.Target.m_agent, m_Owner.EyePosition); TriggerSpitters();

             yield return new WaitForSeconds(timeDelay * 0.25f);
            EnvironmentStateManager.AttemptLightningStrike(Vector3.down, new(0.5f, 0.1f, 0.025f, 1));
            ProjectileShotgun(25, ProjectileType.TargetingSmall, m_Owner.AI.Target.m_agent, m_Owner.EyePosition); TriggerSpitters();

            yield return new WaitForSeconds(timeDelay * 0.25f);
            EnvironmentStateManager.AttemptLightningStrike(Vector3.down, new(0.5f, 0.1f, 0.025f, 1));
            ProjectileShotgun(25, ProjectileType.TargetingSmall, m_Owner.AI.Target.m_agent, m_Owner.EyePosition); TriggerSpitters();

            yield return new WaitForSeconds(timeDelay * 0.25f);
            EnvironmentStateManager.AttemptLightningStrike(Vector3.down, new(0.5f, 0.1f, 0.025f, 1));
            ProjectileShotgun(25, ProjectileType.TargetingSmall, m_Owner.AI.Target.m_agent, m_Owner.EyePosition); TriggerSpitters();
            SyncTryOpen();
        }

        public void ProjectileShotgun(int count, ProjectileType type, Agent target, Vector3 pos)
        {
            var angleSpread = 360 / count;
            for (var i = 0; i < count; i++)
            {
                ProjectileManager.WantToFireTargeting(type, target, pos, Quaternion.Euler(0, angleSpread * i, 0) * new Vector3(0, 0.25f, 1f));
            }
        }

        public void TriggerSpitters()
        {
            foreach (var spitter in InfectionSpitter.s_allSpitters)
            {
                spitter.DoExplode();
            }
        }


        public roseAnimation m_StateTarget = roseAnimation.Shut;
        public roseAnimation m_State = roseAnimation.Shut;
        public List<Animator> m_petalAnimators;
        public EnemyAgent m_Owner;
        public InstancedRenderFeature m_OwnerTentacles;
        public Light m_Light;
        public CellSoundPlayer m_Sound;
        public bool m_OwnerIsAlive = true;
        public Vector3 m_OwnerPosition;
        public enum roseAnimation
        {
            Open = 0,
            Shut = 1,
            Wither = 2
        }

        public struct pAnimation
        {
            public roseAnimation m_TargetState;
        }

        public static event Action OnDead;
        public static Manager_VenusWeed_Rose Current;
    }
}