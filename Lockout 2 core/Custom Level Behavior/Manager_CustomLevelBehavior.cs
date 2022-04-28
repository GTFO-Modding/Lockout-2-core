using LevelGeneration;
using System;
using UnityEngine;

namespace Lockout_2_core.Custom_Level_Behavior
{
    class Manager_CustomLevelBehavior
    {
        public static void situp()
        {
            A1 = new A1_Reawakening();
            B1 = new B1_Exothermic();
            B3 = new B3_Inquery();
            C1 = new C1_The_Crimson_King();
            D1 = new D1_Arboretum();
            E1 = new GameObject().AddComponent<E1_Old_Friends>();
            F1 = new F1_Lockout();

            LG_Factory.add_OnFactoryBuildStart((Action)SetupCustomLevel);
        }
        
        public static void SetupCustomLevel()
        {
            
            switch (RundownManager.ActiveExpedition.LevelLayoutData)
            {
                case 1000: A1.Setup(); break;
                case 1001: B1.Setup(); break;
                case 1003: B3.Setup(); break;
                case 1004: C1.Setup(); break;
                case 1005: D1.Setup(); break;
                case 1006: E1.Setup(); break;
                case 1008: F1.Setup(); break;
            }
            
        }

        public static void UpdateObjectiveText(LG_LayerType layer, string text)
        {
            //todo: this shit should update the objective text on your hud
        }

        public static A1_Reawakening A1;
        public static B1_Exothermic B1;
        public static B3_Inquery B3;
        public static C1_The_Crimson_King C1;
        public static D1_Arboretum D1;
        public static E1_Old_Friends E1;
        public static F1_Lockout F1;
    }
}
