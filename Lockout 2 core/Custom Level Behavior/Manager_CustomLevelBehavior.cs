using LevelGeneration;
using System;

namespace Lockout_2_core.Custom_Level_Behavior
{
    class Manager_CustomLevelBehavior
    {
        public static void setup()
        {
            A1 = new A1_Reawakening();
            B1 = new B1_Exothermic();
            B3 = new B3_Inquery();
            C1 = new C1_The_Crimson_King();
            D1 = new D1_Arboretum();

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
    }
}
