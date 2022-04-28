using LevelGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Lockout_2_core.Custom_Level_Behavior
{
    public class LG_LightFadeAnimator : MonoBehaviour
    {
        public LG_LightFadeAnimator(IntPtr value) : base(value) { }

        public void Awake() { m_Light = gameObject.GetComponent<LG_Light>(); }

        public void FadeOut()
        {
            CoroutineHandler.Add(FadeOutRoutine(0.01f));
        }

        public IEnumerator FadeOutRoutine(float rate) 
        {
            rate += (float)m_Rand.NextDouble() - 0.5f * (rate * 0.25f);

            while (m_Light.m_intensity > 0.001)
            {
                m_Light.ChangeIntensity(Mathf.Lerp(m_Light.m_intensity, 0, rate));
                yield return null;
            }

            m_Light.gameObject.SetActive(false);
            yield return null;
        }

        public LG_Light m_Light;
        public System.Random m_Rand = new();
    }
}
