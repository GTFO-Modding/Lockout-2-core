using Player;

namespace Lockout_2_core.Custom_Player_Behavior
{
    class DisableStamina
    {
        public static void Setup()
        {
            PlayerManager.Current.m_localPlayerAgentInLevel.Stamina.enabled = false;
        }
    }
}
