using System;
using System.Collections.Generic;
using AK;
using Feedback;
using UnityEngine;
using SNetwork;
using FX_EffectSystem;
using AssetShards;
using AIGraph;
using Enemies;
using GTFO.API.Components;

namespace Lockout_2_core.Custom_Weapon_code
{
    public class Item_GrenadeLauncher_Projectile : ConsumableInstance
    {
        public Item_GrenadeLauncher_Projectile(IntPtr value) : base(value)
        { }

        public void Awake()
        {
            m_rigidbody = gameObject.GetComponent<Rigidbody>();
            m_sound = new CellSoundPlayer();

            if (pool_Explosion == null)
            {
                pool_Explosion = FX_Manager.GetEffectPool(AssetShardManager.GetLoadedAsset<GameObject>("Assets/AssetPrefabs/FX_Effects/FX_Tripmine.prefab", false));
            }
        }

        public void FixedUpdate()
        {
            if (m_rigidbody.useGravity)
            {
                var endpoint = transform.position + (m_rigidbody.velocity * Time.fixedDeltaTime);

                collision = Physics.Linecast(transform.position, endpoint);
            }
            else if (!madeNoise) 
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
                if (collision) DetonateSequence();
            }
            else if (Time.time > decayTime)
            {
                m_grenadeBase.ReplicationWrapper.Replicator.Despawn();
            }
        }

        public void OnCollisionEnter()
        {
            if (!m_rigidbody.useGravity) return;
            DetonateSequence();
        }

        public void DetonateSequence()
        {
            Detonate();
            decayTime = Time.time + 10f;
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            m_rigidbody.useGravity = false;
        }

        public void Detonate()
        {
            FX_EffectBase vfx_Explode = pool_Explosion.AquireEffect();
            vfx_Explode.Play(null, transform.position, Quaternion.LookRotation(Vector3.up));

            if (SNet.IsMaster)
            {
                DamageUtil.DoExplosionDamage(transform.position, DMGRADIUS_H, DMGVALUE_H, m_explosionTargetMask, m_explosionBlockMask, m_addForce, EXPLOSIONFORCE);
                DamageUtil.DoExplosionDamage(transform.position, DMGRADIUS_L, DMGVALUE_L, m_explosionTargetMask, m_explosionBlockMask, m_addForce, EXPLOSIONFORCE);
            }
        }

        public void MakeNoise()
        {
            List<string> listenerAlloc = new List<string>();
            Dam_EnemyDamageLimb idamageable;
            EnemyAgent listener;
            var listeners = Physics.OverlapSphere(transform.position, 50f, LayerManager.MASK_ENEMY_DAMAGABLE);
            foreach (var collider in listeners)
            {
                idamageable = collider.GetComponent<Dam_EnemyDamageLimb>();
                if (idamageable == null) continue;

                listener = idamageable.GlueTargetEnemyAgent;
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



        const float DMGRADIUS_H = 2f;
        const float DMGVALUE_H = 80.1f;
        const float DMGRADIUS_L = 5f;
        const float DMGVALUE_L = 20.1f;
        const float EXPLOSIONFORCE = 1000f;

        public int m_projCollideMask = LayerManager.MASK_ENEMY_PROJECTILE_COLLIDERS;
        public int m_explosionTargetMask = LayerManager.MASK_EXPLOSION_TARGETS;
        public int m_explosionBlockMask = LayerManager.MASK_EXPLOSION_BLOCKERS;

        public bool madeNoise = false;
        public AIG_CourseNode m_node;
        public bool collision = false;
        public bool m_addForce = true;
        public float decayTime;
        public Vector3 lastPos;
        public FeedbackPlayer m_vfxExplosion;
        public Rigidbody m_rigidbody;
        public CellSoundPlayer m_sound;
        public GrenadeBase m_grenadeBase;

        public FX_PointLight m_PointLight;
        private static FX_Pool pool_Explosion = null;
    }
}
