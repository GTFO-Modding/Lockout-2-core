using AIGraph;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using System.Linq;

namespace Lockout_2_core.Custom_Level_Behavior
{
    class VaultSecurityDoorPlug
    {
        /*
        public static void Setup(GameObject plugGO)
        {
            return;
            var portalDivider = plugGO.AddComponent<LG_PortalDivider>();
            var plugDoorAlign = plugGO.transform.FindChild("Crossing/PlugGate").gameObject.AddComponent<LG_PlugDoorAlign>();

            portalDivider.m_inFront = plugGO.transform.FindChild("InFront");
            portalDivider.m_behind = plugGO.transform.FindChild("Behind");
            portalDivider.m_crossing = plugGO.transform.FindChild("Crossing");

            plugDoorAlign.m_type = LG_GateType.Large;
            plugDoorAlign.m_subComplex = Expedition.SubComplex.Refinery;

            var lightHolder1_F = plugGO.transform.FindChild("InFront/Wall/LightHolder_1");
            var spotLightAmbient1_F = lightHolder1_F.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient1_F.m_ambientPoint = spotLightAmbient1_F.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient1_F.m_ambientScale = 0.5f;
            spotLightAmbient1_F.m_spotLight = spotLightAmbient1_F.GetComponent<Light>();
            spotLightAmbient1_F.m_category = LG_Light.LightCategory.DoorImportant;
            var lightMesh1_F = lightHolder1_F.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh1_F.m_categoryWhenStandalone = LG_Light.LightCategory.DoorImportant;
            lightMesh1_F.m_colorCurrent = Color.white;
            lightMesh1_F.m_colorDisabled = Color.black;
            lightMesh1_F.m_emitterMeshes = new(2);
            lightMesh1_F.m_emitterMeshes[0] = lightMesh1_F.GetComponent<MeshRenderer>();
            lightMesh1_F.m_emitterMeshes[1] = lightMesh1_F.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh1_F.m_isEnabled = true;

            var lightHolder2_F = plugGO.transform.FindChild("InFront/Wall/LightHolder_2");
            var spotLightAmbient2_F = lightHolder2_F.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient2_F.m_ambientPoint = spotLightAmbient2_F.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient2_F.m_ambientScale = 0.5f;
            spotLightAmbient2_F.m_spotLight = spotLightAmbient2_F.GetComponent<Light>();
            spotLightAmbient2_F.m_category = LG_Light.LightCategory.DoorImportant;
            var lightMesh2_F = lightHolder2_F.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh2_F.m_categoryWhenStandalone = LG_Light.LightCategory.DoorImportant;
            lightMesh2_F.m_colorCurrent = Color.white;
            lightMesh2_F.m_colorDisabled = Color.black;
            lightMesh2_F.m_emitterMeshes = new(2);
            lightMesh2_F.m_emitterMeshes[0] = lightMesh2_F.GetComponent<MeshRenderer>();
            lightMesh2_F.m_emitterMeshes[1] = lightMesh2_F.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh2_F.m_isEnabled = true;

            var lightHolder3_F = plugGO.transform.FindChild("InFront/Wall/LightHolder_3");
            var spotLightAmbient3_F = lightHolder3_F.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient3_F.m_ambientPoint = spotLightAmbient3_F.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient3_F.m_ambientScale = 0.5f;
            spotLightAmbient3_F.m_spotLight = spotLightAmbient3_F.GetComponent<Light>();
            spotLightAmbient3_F.m_category = LG_Light.LightCategory.DoorImportant;
            var lightMesh3_F = lightHolder3_F.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh3_F.m_categoryWhenStandalone = LG_Light.LightCategory.DoorImportant;
            lightMesh3_F.m_colorCurrent = Color.white;
            lightMesh3_F.m_colorDisabled = Color.black;
            lightMesh3_F.m_emitterMeshes = new(2);
            lightMesh3_F.m_emitterMeshes[0] = lightMesh3_F.GetComponent<MeshRenderer>();
            lightMesh3_F.m_emitterMeshes[1] = lightMesh3_F.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh3_F.m_isEnabled = true;

            var lightHolder4_F = plugGO.transform.FindChild("InFront/Wall/LightHolder_4");
            var spotLightAmbient4_F = lightHolder4_F.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient4_F.m_ambientPoint = spotLightAmbient4_F.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient4_F.m_ambientScale = 0.5f;
            spotLightAmbient4_F.m_spotLight = spotLightAmbient4_F.GetComponent<Light>();
            spotLightAmbient4_F.m_category = LG_Light.LightCategory.DoorImportant;
            var lightMesh4_F = lightHolder4_F.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh4_F.m_categoryWhenStandalone = LG_Light.LightCategory.DoorImportant;
            lightMesh4_F.m_colorCurrent = Color.white;
            lightMesh4_F.m_colorDisabled = Color.black;
            lightMesh4_F.m_emitterMeshes = new(2);
            lightMesh4_F.m_emitterMeshes[0] = lightMesh4_F.GetComponent<MeshRenderer>();
            lightMesh4_F.m_emitterMeshes[1] = lightMesh4_F.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh4_F.m_isEnabled = true;

            var lightHolder5_F = plugGO.transform.FindChild("InFront/Wall/LightHolder_5");
            var spotLightAmbient5_F = lightHolder5_F.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient5_F.m_ambientPoint = spotLightAmbient5_F.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient5_F.m_ambientScale = 0.5f;
            spotLightAmbient5_F.m_spotLight = spotLightAmbient5_F.GetComponent<Light>();
            spotLightAmbient5_F.m_category = LG_Light.LightCategory.DoorImportant;
            var lightMesh5_F = lightHolder5_F.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh5_F.m_categoryWhenStandalone = LG_Light.LightCategory.DoorImportant;
            lightMesh5_F.m_colorCurrent = Color.white;
            lightMesh5_F.m_colorDisabled = Color.black;
            lightMesh5_F.m_emitterMeshes = new(2);
            lightMesh5_F.m_emitterMeshes[0] = lightMesh5_F.GetComponent<MeshRenderer>();
            lightMesh5_F.m_emitterMeshes[1] = lightMesh5_F.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh5_F.m_isEnabled = true;

            var lightHolder1_B = plugGO.transform.FindChild("Behind/Wall/LightHolder_1");
            var spotLightAmbient1_B = lightHolder1_B.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient1_B.m_ambientPoint = spotLightAmbient1_B.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient1_B.m_ambientScale = 0.5f;
            spotLightAmbient1_B.m_spotLight = spotLightAmbient1_B.GetComponent<Light>();
            spotLightAmbient1_B.m_category = LG_Light.LightCategory.DoorImportant;
            var lightMesh1_B = lightHolder1_B.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh1_B.m_categoryWhenStandalone = LG_Light.LightCategory.DoorImportant;
            lightMesh1_B.m_colorCurrent = Color.white;
            lightMesh1_B.m_colorDisabled = Color.black;
            lightMesh1_B.m_emitterMeshes = new(2);
            lightMesh1_B.m_emitterMeshes[0] = lightMesh1_B.GetComponent<MeshRenderer>();
            lightMesh1_B.m_emitterMeshes[1] = lightMesh1_B.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh1_B.m_isEnabled = true;

            var lightHolder2_B = plugGO.transform.FindChild("Behind/Wall/LightHolder_2");
            var spotLightAmbient2_B = lightHolder2_B.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient2_B.m_ambientPoint = spotLightAmbient2_B.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient2_B.m_ambientScale = 0.5f;
            spotLightAmbient2_B.m_spotLight = spotLightAmbient2_B.GetComponent<Light>();
            spotLightAmbient2_B.m_category = LG_Light.LightCategory.DoorImportant;
            var lightMesh2_B = lightHolder2_B.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh2_B.m_categoryWhenStandalone = LG_Light.LightCategory.DoorImportant;
            lightMesh2_B.m_colorCurrent = Color.white;
            lightMesh2_B.m_colorDisabled = Color.black;
            lightMesh2_B.m_emitterMeshes = new(2);
            lightMesh2_B.m_emitterMeshes[0] = lightMesh2_B.GetComponent<MeshRenderer>();
            lightMesh2_B.m_emitterMeshes[1] = lightMesh2_B.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh2_B.m_isEnabled = true;

            var lightHolder3_B = plugGO.transform.FindChild("Behind/Wall/LightHolder_3");
            var spotLightAmbient3_B = lightHolder3_B.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient3_B.m_ambientPoint = spotLightAmbient3_B.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient3_B.m_ambientScale = 0.5f;
            spotLightAmbient3_B.m_spotLight = spotLightAmbient3_B.GetComponent<Light>();
            spotLightAmbient3_B.m_category = LG_Light.LightCategory.DoorImportant;
            var lightMesh3_B = lightHolder3_B.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh3_B.m_categoryWhenStandalone = LG_Light.LightCategory.DoorImportant;
            lightMesh3_B.m_colorCurrent = Color.white;
            lightMesh3_B.m_colorDisabled = Color.black;
            lightMesh3_B.m_emitterMeshes = new(2);
            lightMesh3_B.m_emitterMeshes[0] = lightMesh3_B.GetComponent<MeshRenderer>();
            lightMesh3_B.m_emitterMeshes[1] = lightMesh3_B.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh3_B.m_isEnabled = true;

            var lightHolder4_B = plugGO.transform.FindChild("Behind/Wall/LightHolder_4");
            var spotLightAmbient4_B = lightHolder4_B.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient4_B.m_ambientPoint = spotLightAmbient4_B.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient4_B.m_ambientScale = 0.5f;
            spotLightAmbient4_B.m_spotLight = spotLightAmbient4_B.GetComponent<Light>();
            spotLightAmbient4_B.m_category = LG_Light.LightCategory.DoorImportant;
            var lightMesh4_B = lightHolder4_B.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh4_B.m_categoryWhenStandalone = LG_Light.LightCategory.DoorImportant;
            lightMesh4_B.m_colorCurrent = Color.white;
            lightMesh4_B.m_colorDisabled = Color.black;
            lightMesh4_B.m_emitterMeshes = new(2);
            lightMesh4_B.m_emitterMeshes[0] = lightMesh4_B.GetComponent<MeshRenderer>();
            lightMesh4_B.m_emitterMeshes[1] = lightMesh4_B.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh4_B.m_isEnabled = true;

            var lightHolder5_B = plugGO.transform.FindChild("Behind/Wall/LightHolder_5");
            var spotLightAmbient5_B = lightHolder5_B.transform.FindChild("LG_SpotLightAmbient").gameObject.AddComponent<LG_SpotLightAmbient>();
            spotLightAmbient5_B.m_ambientPoint = spotLightAmbient5_B.transform.FindChild("AmbientPoint").gameObject.GetComponent<Light>();
            spotLightAmbient5_B.m_ambientScale = 0.5f;
            spotLightAmbient5_B.m_spotLight = spotLightAmbient5_B.GetComponent<Light>();
            spotLightAmbient5_B.m_category = LG_Light.LightCategory.DoorImportant;
            var lightMesh5_B = lightHolder5_B.FindChild("prop_generic_light_led_rectangle_01/prop_generic_light_led_rectangle_01_1/g_prop_generic_light_led_rectangle_01_glass").gameObject.AddComponent<LG_LightEmitterMesh>();
            lightMesh5_B.m_categoryWhenStandalone = LG_Light.LightCategory.DoorImportant;
            lightMesh5_B.m_colorCurrent = Color.white;
            lightMesh5_B.m_colorDisabled = Color.black;
            lightMesh5_B.m_emitterMeshes = new(2);
            lightMesh5_B.m_emitterMeshes[0] = lightMesh5_B.GetComponent<MeshRenderer>();
            lightMesh5_B.m_emitterMeshes[1] = lightMesh5_B.transform.parent.FindChild("g_prop_generic_light_led_rectangle_01_metal").gameObject.GetComponent<MeshRenderer>();
            lightMesh5_B.m_isEnabled = true;
        }
        */
    }
}
