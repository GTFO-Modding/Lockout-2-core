using System;
using UnityEngine;
using Player;
using UnityEngine.Rendering.PostProcessing;
using Il2CppInterop.Runtime.Attributes;

namespace Lockout_2_core.Custom_Weapon_code
{
    public class Manager_FlashbangBlinder : MonoBehaviour
    {
        public Manager_FlashbangBlinder(IntPtr value) : base(value)
        { }

        public void Awake()
        {
        }

        public void Update()
        {
            if (Blind && BlindProgress < Clock.Time)
            {
                Blind = false;
                ApplySettings(-8, 0, EyeAdaptation.Progressive);
            }
        }
        

        public void BlindPlayer()
        {
            for (var i = 0; i < 20; i++)
            {
                PlayerManager.Current.m_localPlayerAgentInLevel.Sound.Post(AK.EVENTS.STINGER);
            }
            
            Blind = true;
            BlindProgress = Clock.Time + 2f;
            ApplySettings(-1500, -1500, EyeAdaptation.Fixed);
        }

        public void GiveCamera(FPSCamera camera)
        {
            PlayerCamera = camera;

            UserDefaults = PlayerCamera.postProcessing.m_autoExposure;
            BlindSettings = UserDefaults;
        }

        [HideFromIl2Cpp]
        public void ApplySettings(float minLuminance, float maxLuminance, EyeAdaptation type)
        {
            PlayerCamera.postProcessing.m_autoExposure.minLuminance.value = minLuminance;
            PlayerCamera.postProcessing.m_autoExposure.maxLuminance.value = maxLuminance;
            PlayerCamera.postProcessing.m_autoExposure.eyeAdaptation.value = type;
        }

        public AutoExposure UserDefaults;
        public AutoExposure BlindSettings;
        public FPSCamera PlayerCamera;


        public bool Blind;
        public float BlindProgress = 0f;
    }
}
