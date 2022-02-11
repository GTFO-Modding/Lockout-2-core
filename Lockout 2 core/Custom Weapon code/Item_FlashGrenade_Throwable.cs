using System;
using System.Collections.Generic;
using AK;
using UnityEngine;
using FX_EffectSystem;
using Player;
using AIGraph;
using Enemies;
using GTFO.API.Components;
using GameData;

namespace Lockout_2_core.Custom_Weapon_code
{
    public class Item_FlashGrenade_Throwable : ConsumableInstance
    {
        public Item_FlashGrenade_Throwable(IntPtr value) : base(value)
        { }

        public void Awake()
        {
            m_rigidbody = gameObject.GetComponent<Rigidbody>();
            m_sound = new CellSoundPlayer();

            FX_Manager.TryAllocateFXLight(out m_Light);
            
            lifeTime = Time.time + 2f;
        }

        public override void Setup(ItemDataBlock data)
        {
            base.Setup(data);
        }

        public void FixedUpdate()
        {
            if (!madeNoise && !m_rigidbody.useGravity) 
            {
                MakeNoise();
                transform.position = Vector3.down * 100f;
                madeNoise = true;
            }
        }

        public void Update()
        {
            if (m_rigidbody.useGravity)
            {
                m_sound.UpdatePosition(transform.position);
                if (Time.time > lifeTime) DetonateSequence();
            }
            else
            {
                m_Light.m_position = fx_position;
                m_Light.SetColor((Vector4)(m_Light.m_linearColor - new Vector3(0f, 0f, 0.0000001f)));
                m_Light.m_intensity = Math.Max(0, m_Light.m_intensity - 0.5f);
                m_Light.UpdateData();
                m_Light.UpdateTransform();
                m_Light.enabled = false;

                if (Time.time < enemyStunTimer && Time.time > enemyStunTick)
                {
                    enemyStunTick += 0.5f;

                    foreach (var enemy in stunTargetGotHit)
                    {
                        if (enemy == null) continue;
                        enemy.EnemySFXData.SFX_ID_hurtBig = 0;
                        enemy.Locomotion.Hitreact.ActivateState(ES_HitreactType.Heavy, ImpactDirection.Front, false, Owner, enemy.EyePosition);
                    }
                }

                if (Time.time > decayTime)
                {
                    foreach (var enemy in stunTargetGotHit)
                    {
                        if (enemy == null) continue;
                        stunEnemyHurtSFX.TryGetValue(enemy.Pointer, out var soundEvent);
                        enemy.EnemySFXData.SFX_ID_hurtBig = soundEvent;
                    }
                    ReplicationWrapper.Replicator.Despawn();
                }
            }
        }

        public void OnCollisionEnter()
        {
            m_sound.Post(EVENTS.DECOYCANBOUNCE);
        }

        public void DetonateSequence()
        {
            Detonate();

            fx_position = transform.position;
            m_Light.SetRange(70f);
            m_Light.SetColor(Color.white);
            m_Light.m_intensity = 10f;
            m_Light.UpdateData();

            decayTime = Time.time + 10f;
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_rigidbody.useGravity = false;

            var viewPos = PlayerManager.Current.m_localPlayerAgentInLevel.FPSCamera.GetComponent<UnityEngine.Camera>().WorldToViewportPoint(this.transform.position);
            if (viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1 && viewPos.z > 0 && !Physics.Linecast(transform.position + (Vector3.up * 0.5f), PlayerManager.Current.m_localPlayerAgentInLevel.EyePosition, LayerManager.MASK_WORLD))
            {
                PlayerManager.Current.m_localPlayerAgentInLevel.FPSCamera.GetComponent<Manager_FlashbangBlinder>().BlindPlayer();
            }
        }

        public void Detonate()
        {
            stunTargetGotHit.Clear();

            List<IntPtr> stunTargetMemory = new();
            IDamageable idamageable;
            EnemyAgent stunTarget;
            ImpactDirection impactDirection;
            stunTargetGotHit.Clear();
            stunEnemyHurtSFX.Clear();

            var stunTargets = Physics.OverlapSphere(transform.position, 10f, LayerManager.MASK_ENEMY_DAMAGABLE);
            foreach (var collider in stunTargets)
            {
                idamageable = collider.GetComponent<IDamageable>();
                if (idamageable == null) continue;

                stunTarget = idamageable.Cast<Dam_EnemyDamageLimb>().GlueTargetEnemyAgent;
                if (stunTarget == null || stunTargetMemory.Contains(stunTarget.Pointer)) continue;
                stunTargetMemory.Add(stunTarget.Pointer);

                if (!Physics.Linecast(transform.position + (Vector3.up * 0.5f), stunTarget.EyePosition, LayerManager.MASK_WORLD))
                {
                    stunTargetGotHit.Add(stunTarget);
                    stunEnemyHurtSFX.Add(stunTarget.Pointer, stunTarget.EnemySFXData.SFX_ID_hurtBig);

                    impactDirection = ES_Hitreact.GetDirection(stunTarget.transform, (stunTarget.transform.position - transform.position));

                    stunTarget.Locomotion.Hitreact.ActivateState(ES_HitreactType.Heavy, impactDirection, false, Owner, stunTarget.EyePosition);
                }
            }
            enemyStunTimer = Time.time + 3f;
            enemyStunTick = Time.time + 0.5f;
        }

        public void MakeNoise()
        {
            List<string> listenerAlloc = new List<string>();
            IDamageable idamageable;
            EnemyAgent listener;
            var listeners = Physics.OverlapSphere(transform.position, 50f, LayerManager.MASK_ENEMY_DAMAGABLE);
            foreach (var collider in listeners)
            {
                idamageable = collider.GetComponent<IDamageable>();
                if (idamageable == null) continue;

                listener = idamageable.TryCast<Dam_EnemyDamageLimb>().GlueTargetEnemyAgent;
                if (listener == null || listenerAlloc.Contains(listener.gameObject.name)) continue;
                listenerAlloc.Add(listener.gameObject.name);

                if (!Physics.Linecast(transform.position, listener.EyePosition, LayerManager.MASK_WORLD))
                {
                    var noiseData = new NM_NoiseData()
                    {
                        position = listener.EyePosition,
                        node = listener.CourseNode,
                        type = NM_NoiseType.InstaDetect,
                        radiusMin = 0.01f,
                        radiusMax = 100f,
                        yScale = 1f,
                        noiseMaker = null,
                        raycastFirstNode = false,
                        includeToNeightbourAreas = false
                    };
                    NoiseManager.MakeNoise(noiseData);
                }
            }
            m_sound.Post(EVENTS.FRAGGRENADEEXPLODE);
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
        }

        public int m_projCollideMask = LayerManager.MASK_ENEMY_PROJECTILE_COLLIDERS;
        public int m_explosionTargetMask = LayerManager.MASK_EXPLOSION_TARGETS;
        public int m_explosionBlockMask = LayerManager.MASK_EXPLOSION_BLOCKERS;

        public bool madeNoise = false;
        public AIG_CourseNode m_node;
        public bool collision = false;
        public bool m_addForce = true;
        public float decayTime;
        public float lifeTime;
        public Rigidbody m_rigidbody;
        public CellSoundPlayer m_sound;
        public FX_PointLight m_Light;
        public MeshRenderer m_Renderer;

        public List<EnemyAgent> stunTargetGotHit = new();
        public Dictionary<IntPtr, uint> stunEnemyHurtSFX = new();
        public float enemyStunTimer;
        public float enemyStunTick;

        public Vector3 fx_position;
    }
}
