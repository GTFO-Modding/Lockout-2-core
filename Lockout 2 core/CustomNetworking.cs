using GTFO.API;
using LevelGeneration;
using Lockout_2_core.Custom_Enemies;
using Lockout_2_core.Custom_Level_Behavior;
using Lockout_2_core.Custom_Player_Behavior;

namespace Lockout_2_core
{
    class CustomNetworking
    {
        public static void RegisterPackets()
        {
            NetworkAPI.RegisterEvent<byte>("ClientRequestInfo", B1_Exothermic.ClientRequestInfo);

            NetworkAPI.RegisterEvent<B1_Exothermic.pTerminalUpdate>("B1TerminalStatus", B1_Exothermic.RecieveTerminalUpdate);
            NetworkAPI.RegisterEvent<B1_Exothermic.pObjectiveUpdate>("B1ObjectiveStatus", B1_Exothermic.RecieveObjectiveUpdate);
            NetworkAPI.RegisterEvent<B1_Exothermic.pAlarmShutoff>("B1AlarmShutoff", B1_Exothermic.RecieveAlarmShutoff);

            NetworkAPI.RegisterEvent<byte>("C1ClientRequestInfo", C1_The_Crimson_King.ClientRequestInfo);
            NetworkAPI.RegisterEvent<C1_The_Crimson_King.pWardenTerminalInfo>("C1ProvideTerminalInfo", C1_The_Crimson_King.OnTerminalInfoRecieved);
            NetworkAPI.RegisterEvent<byte>("C1DisableBSOD", C1_The_Crimson_King.RecieveDisableBSOD);
            NetworkAPI.RegisterEvent<C1_The_Crimson_King.pVerifyCaptcha>("C1VerifyCaptcha", C1_The_Crimson_King.SyncRecieveVerifyCaptcha);
            NetworkAPI.RegisterEvent<byte>("C1SyncSmallPickup", C1_The_Crimson_King.SyncOnCollectSmallPickup);

            NetworkAPI.RegisterEvent<Manager_VenusWeed_Rose.pAnimation>("RoseBossSyncAnimationState", Manager_VenusWeed_Rose.RecieveAnimPacket);

            NetworkAPI.RegisterEvent<byte>("E1SyncHSUState", E1_Old_Friends.OnHSUExitSequence);
            NetworkAPI.RegisterEvent<pReactorInteraction>("OnAttemptInteractReactor", Patch_LG_WardenObjective_Reactor.OnAttemptInteract);
            NetworkAPI.RegisterEvent<byte>("E1ClientRequestInfo", E1_Old_Friends.ClientRequestInfo);
            NetworkAPI.RegisterEvent<E1_Old_Friends.pObjectiveData>("E1ProvideObjectiveInfo", E1_Old_Friends.RecieveObjectiveInfo);
            NetworkAPI.RegisterEvent<float>("E1SyncTemp", E1_Old_Friends.SyncTemp);

            NetworkAPI.RegisterEvent<int>("PlayerDeathDisableRevive", PlayerDeathManager.RecieveDisablePlayerRevive);
            NetworkAPI.RegisterEvent<int>("PlayerDeathPostDeath", PlayerDeathManager.RecievePostDeath);
        }
    }
}
