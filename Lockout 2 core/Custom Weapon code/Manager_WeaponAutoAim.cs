using Enemies;
using Gear;
using Player;
using System;
using UnityEngine;

namespace Lockout_2_core.Custom_Weapon_code
{
    class Manager_WeaponAutoAim : MonoBehaviour
    {
        public Manager_WeaponAutoAim(IntPtr value) : base(value) { }

        public void Setup() { }
        
        public void SetupReticle()
        {
            m_ReticleHolder = new GameObject();
            m_ReticleHolder.transform.SetParent(GuiManager.CrosshairLayer.CanvasTrans);
            m_Reticle = GameObject.Instantiate(GuiManager.CrosshairLayer.m_hitIndicatorFriendly, m_ReticleHolder.transform);
            m_Reticle.name = "SmartPistol_AutoAimIndicator";
            m_Reticle.transform.localScale = Vector3.zero;
            SetVFX(m_TargetedColor, m_TargetedEulerAngles);
            m_Reticle.transform.localEulerAngles = m_TargetedEulerAngles;
        }
        public void Update() 
        {
            UpdateColor();
            UpdateDetection();

            /*
            if (m_BulletWeapon.GetCurrentClip() <= 0)
            {
                if (m_HasTarget || m_Target != null)
                {
                    SetVFX(m_UnTargetedColor, m_TargetedEulerAngles);
                    m_Reticle.AnimateScale(0, 0.5f);
                    m_HasTarget = false;
                    m_Target = null;
                }
                return;
            }
            */
        }

        public void UpdateDetection()
        {
            if (m_Target != null)
            {
                m_ReticleHolder.transform.position = m_PlayerCamera.WorldToScreenPoint(m_Target.AimTarget.position);
                if (!m_HasTarget)
                {
                    m_Reticle.transform.localScale = Vector3.one * 2f;
                    m_Reticle.transform.localEulerAngles = m_TargetedEulerAngles;
                    m_Reticle.AnimateScale(1.5f, 0.13f);
                    m_HasTarget = true;
                }
            }
            else
            {
                m_Reticle.transform.localEulerAngles += new Vector3(0, 0, 5);
            }

            if (m_HasTarget && m_Target == null)
            {
                m_Reticle.AnimateScale(0, 0.5f);
                m_HasTarget = false;
            }

            if (m_DetectionTick >= Time.time) return;

            m_Target = SentryGunInstance_Detection.CheckForTargetLegacy(m_BulletWeapon.ArchetypeData, m_BulletWeapon.transform);
            m_DetectionTick = Time.time + 0.25f;
        }

        public void UpdateColor()
        {
            if (m_HasTarget)
                m_Reticle.m_hitColor = m_TargetedColor;

            else
                m_Reticle.m_hitColor = m_UnTargetedColor;

            if (!InputMapper.GetButtonKeyMouse(InputAction.Aim, eFocusState.FPS) || m_BulletWeapon.GetCurrentClip() <= 0) 
            {
                m_ReticleHolder.transform.localScale = Vector3.one * 0.5f;
                m_ReticleHolder.transform.localEulerAngles += new Vector3(0, 0, 2);
                m_Reticle.m_hitColor = m_PassiveDetection;
            }
            else
            {
                m_ReticleHolder.transform.localScale = Vector3.one;
                m_ReticleHolder.transform.localEulerAngles = Vector3.zero;
            }
                

            m_Reticle.UpdateColorsWithAlphaMul(m_Reticle.m_hitColor.a);
        }

        public void SetVFX(Color color, Vector3 euler)
        {
            m_Reticle.transform.localEulerAngles = euler;
            m_Reticle.m_hitColor = color;
            m_Reticle.UpdateColorsWithAlphaMul(1.0f);
        }

        public GameObject m_ReticleHolder;
        public CrosshairHitIndicator m_Reticle;
        public EnemyAgent m_Target;
        public Camera m_PlayerCamera;
        public float m_DetectionTick;
        public bool m_HasTarget;
        public BulletWeapon m_BulletWeapon;
        public PlayerAgent m_Owner;

        public Color m_TargetedColor = new Color(1.2f, 0.3f, 0.1f, 1.0f);
        public Color m_UnTargetedColor = new Color(0.3f, 0.1f, 0.1f, 1.0f);
        public Color m_PassiveDetection = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public Vector3 m_TargetedEulerAngles = new Vector3(0,0,45);
    }
}
