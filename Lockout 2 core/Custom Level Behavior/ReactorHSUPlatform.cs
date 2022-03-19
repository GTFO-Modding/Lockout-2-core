using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Lockout_2_core.Custom_Level_Behavior
{
    class ReactorHSUPlatform
    {
        public static void Setup(GameObject GO)
        {
            var lightHolder1_F = GO.transform.FindChild("lightholder/LightHolder_1");
            var spotLightAmbient1_F = lightHolder1_F.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient1_F.m_ambientPoint = spotLightAmbient1_F.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient1_F.m_ambientScale = 0.5f;
            spotLightAmbient1_F.m_spotLight = spotLightAmbient1_F.GetComponent<Light>();
            spotLightAmbient1_F.m_category = LG_Light.LightCategory.Special;
            var lightMesh1_F = lightHolder1_F.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh1_F.m_categoryWhenStandalone = LG_Light.LightCategory.Special;
            lightMesh1_F.m_colorCurrent = Color.white;
            lightMesh1_F.m_colorDisabled = Color.black;
            lightMesh1_F.m_emitterMeshes = new(2);
            lightMesh1_F.m_emitterMeshes[0] = lightMesh1_F.GetComponent<MeshRenderer>();
            lightMesh1_F.m_emitterMeshes[1] = lightMesh1_F.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh1_F.m_isEnabled = true;

            var lightHolder2_F = GO.transform.FindChild("lightholder/LightHolder_2");
            var spotLightAmbient2_F = lightHolder2_F.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient2_F.m_ambientPoint = spotLightAmbient2_F.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient2_F.m_ambientScale = 0.5f;
            spotLightAmbient2_F.m_spotLight = spotLightAmbient2_F.GetComponent<Light>();
            spotLightAmbient2_F.m_category = LG_Light.LightCategory.Special;
            var lightMesh2_F = lightHolder2_F.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh2_F.m_categoryWhenStandalone = LG_Light.LightCategory.Special;
            lightMesh2_F.m_colorCurrent = Color.white;
            lightMesh2_F.m_colorDisabled = Color.black;
            lightMesh2_F.m_emitterMeshes = new(2);
            lightMesh2_F.m_emitterMeshes[0] = lightMesh2_F.GetComponent<MeshRenderer>();
            lightMesh2_F.m_emitterMeshes[1] = lightMesh2_F.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh2_F.m_isEnabled = true;

            var lightHolder3_F = GO.transform.FindChild("lightholder/LightHolder_3");
            var spotLightAmbient3_F = lightHolder3_F.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient3_F.m_ambientPoint = spotLightAmbient3_F.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient3_F.m_ambientScale = 0.5f;
            spotLightAmbient3_F.m_spotLight = spotLightAmbient3_F.GetComponent<Light>();
            spotLightAmbient3_F.m_category = LG_Light.LightCategory.Special;
            var lightMesh3_F = lightHolder3_F.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh3_F.m_categoryWhenStandalone = LG_Light.LightCategory.Special;
            lightMesh3_F.m_colorCurrent = Color.white;
            lightMesh3_F.m_colorDisabled = Color.black;
            lightMesh3_F.m_emitterMeshes = new(2);
            lightMesh3_F.m_emitterMeshes[0] = lightMesh3_F.GetComponent<MeshRenderer>();
            lightMesh3_F.m_emitterMeshes[1] = lightMesh3_F.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh3_F.m_isEnabled = true;

            var lightHolder4_F = GO.transform.FindChild("lightholder/LightHolder_4");
            var spotLightAmbient4_F = lightHolder4_F.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient4_F.m_ambientPoint = spotLightAmbient4_F.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient4_F.m_ambientScale = 0.5f;
            spotLightAmbient4_F.m_spotLight = spotLightAmbient4_F.GetComponent<Light>();
            spotLightAmbient4_F.m_category = LG_Light.LightCategory.Special;
            var lightMesh4_F = lightHolder4_F.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh4_F.m_categoryWhenStandalone = LG_Light.LightCategory.Special;
            lightMesh4_F.m_colorCurrent = Color.white;
            lightMesh4_F.m_colorDisabled = Color.black;
            lightMesh4_F.m_emitterMeshes = new(2);
            lightMesh4_F.m_emitterMeshes[0] = lightMesh4_F.GetComponent<MeshRenderer>();
            lightMesh4_F.m_emitterMeshes[1] = lightMesh4_F.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh4_F.m_isEnabled = true;

            var lightHolder5_F = GO.transform.FindChild("lightholder/LightHolder_5");
            var spotLightAmbient5_F = lightHolder5_F.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient5_F.m_ambientPoint = spotLightAmbient5_F.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient5_F.m_ambientScale = 0.5f;
            spotLightAmbient5_F.m_spotLight = spotLightAmbient5_F.GetComponent<Light>();
            spotLightAmbient5_F.m_category = LG_Light.LightCategory.Special;
            var lightMesh5_F = lightHolder5_F.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh5_F.m_categoryWhenStandalone = LG_Light.LightCategory.Special;
            lightMesh5_F.m_colorCurrent = Color.white;
            lightMesh5_F.m_colorDisabled = Color.black;
            lightMesh5_F.m_emitterMeshes = new(2);
            lightMesh5_F.m_emitterMeshes[0] = lightMesh5_F.GetComponent<MeshRenderer>();
            lightMesh5_F.m_emitterMeshes[1] = lightMesh5_F.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh5_F.m_isEnabled = true;
        }
    }
}
