using GameData;
using GTFO.API;
using LevelGeneration;
using Player;
using SNetwork;
using System;
using System.Collections.Generic;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace Lockout_2_core.Custom_Level_Behavior
{
    class C1_The_Crimson_King
    {
        public void Setup()
        {
            L.Error("C1 - The Crimson King setup!");
            Patch_WardenObjectiveManager.OnObjectiveComplete += OnObjectiveCompleted;
            Patch_WardenObjectiveManager.OnStartExpedition += OnFactoryBuildComplete;
            Patch_WardenObjectiveManager.OnSolvedObjectiveItem += OnCollectSmallPickup;
            Patch_LG_ComputerTerminal.OnChangedState += OnTerminalStateChange;
            Patch_LG_ComputerTerminalCommandInterpreter.RecieveCommand += TryVerifyCaptcha;
            Patch_LG_TERM_ReactorError.OnExitTerminalReactorError += SetupCaptchaPuzzle;
            Patch_GS_AfterLevel.OnLevelCleanup += OnCleanup;
        }

        public void OnFactoryBuildComplete() 
        {
            m_CaptchaKeys = new();
            foreach (var key in EntryPoint.s_Captchas.Keys) m_CaptchaKeys.Add(key);

            var rng = new System.Random().Next();

            m_CaptchaIndex = rng % m_CaptchaKeys.Count;
            L.Debug($"Random captcha seed: {rng}, captcha key count: {m_CaptchaKeys.Count}, captcha index set to {m_CaptchaIndex}");

            m_WardenTerminal = LG_LevelBuilder.Current.m_currentFloor.allZones[4].TerminalsSpawnedInZone[0];

            m_CaptchaHolder = GameObject.Instantiate<GameObject>(AssetAPI.GetLoadedAsset("Assets/Bundle/Captcha/Content/CaptchaHolder.prefab").TryCast<GameObject>(), m_WardenTerminal.transform.FindChild("Graphics/kit_ElectronicsTerminalConsole/Display"));
            m_CaptchaHolder.transform.localPosition = new Vector3(0.13f, -0.14f, -0.01f);
            m_CaptchaHolder.transform.localEulerAngles = new Vector3(1, 0, 0);
            m_CaptchaHolder.transform.localScale = new Vector3(0.2f, 0.075f, 1f);
            UpdateCaptcha(m_CaptchaIndex);
            m_CaptchaHolder.active = false;

            m_BlueScreen = GameObject.CreatePrimitive(PrimitiveType.Quad);
            m_BlueScreen.transform.parent = m_WardenTerminal.transform.FindChild("Graphics/kit_ElectronicsTerminalConsole/Display");

            m_BlueScreen.transform.localPosition = new Vector3(-0.0295f, -0.01f, -0.0005f);
            m_BlueScreen.transform.localEulerAngles = new Vector3(1, 0, 0);
            m_BlueScreen.transform.localScale = new Vector3(0.55f, 0.404f, 1f);
            m_BlueScreen.GetComponent<Collider>().enabled = false;

            var blueScreenMat = m_BlueScreen.GetComponent<MeshRenderer>().sharedMaterial;
            blueScreenMat.color = new Color(0, 0, 0.1f, 1f);
            blueScreenMat.shader = m_CaptchaHolder.GetComponent<MeshRenderer>().sharedMaterial.shader;

            if (m_WardenTerminal.m_currentState.GetIl2CppType() == Il2CppType.Of<LG_TERM_AudioLoopError>())
            {
                m_WardenTerminal.m_command.ClearOutputQueueAndScreenBuffer();
                m_WardenTerminal.AddLine($"MAINTENANCE TERMINAL {m_WardenTerminal.m_serialNumber} HAS ENCOUNTERED A FATAL ERROR. THE SYSTEM HAS BEEN HALTED TO PREVENT DAMAGE TO LOCAL FILES");
                m_WardenTerminal.AddLine($"://ERR 6428 - NULL REFERENCE EXCEPTION//: OBJECT REFERENCE NOT SET TO AN INSTANCE OF AN OBJECT", false);
                m_WardenTerminal.AddLine($"EXCEPTION IN IL2CPP-TO-MANAGED TRAMPOLINE, NOT PASSING IT INTO IL2CPP:", false);
                m_WardenTerminal.AddLine($"SYSTEM.MISSINGMETHODEXCEPTION//: METHOD NOT FOUND: void .CortexInterface_SysMonitor.Get_Status(int,eStatusType)", false);
                m_WardenTerminal.AddLine($"     at CortexInterfaceManager.CortexInterface_SysMonitor.Update () [0x00028] in <ad5be1f28c8c4553b994319bc7f16f19>:0");
                m_WardenTerminal.AddLine($"IF THIS IS YOUR FIRST TIME ENCOUNTERING THIS ERROR SCREEN, REBOOT THE TERMINAL. IF THIS SCREEN APPEARS AGAIN, FOLLOW THESE STEPS:");
                m_WardenTerminal.AddLine($"://> ENSURE ALL HARDWARE IS SECURELY AND PROPERLY INSTALLED", false);
                m_WardenTerminal.AddLine($"://> ENSURE ALL INSTALLED SOFTWARE IS COMPATIBLE WITH YOUR SYSTEM", false);
                m_WardenTerminal.AddLine($"://> CONTACT YOUR SYSTEMS ADMINISTRATOR TO CORRECT A DAMAGED SYSTEM INSTALLATION");
                m_WardenTerminal.AddLine($"TECHNICAL INFORMATION//:");
                m_WardenTerminal.AddLine($"$90/E692 6C 42 0A    JMP ($0A42)[$90:E695]   A:0000 X:FFFE Y:0000 P:envmxdiZc", false);
                m_WardenTerminal.AddLine($"$90/E695 08          PHP                     A:0000 X:FFFE Y:0000 P:envmxdiZc", false);
                m_WardenTerminal.AddLine($"$90/E696 8B          PHB                     A:0000 X:FFFE Y:0000 P:envmxdiZc", false);
                m_WardenTerminal.AddLine($"$90/E697 4B          PHK                     A:0000 X:FFFE Y:0000 P:envmxdiZc", false);
                m_WardenTerminal.AddLine($"$90/E698 AB          PLB                     A:0000 X:FFFE Y:0000 P:envmxdiZc", false);
                m_WardenTerminal.AddLine($"$90/E699 C2 30       REP #$30                A:0000 X:FFFE Y:0000 P:eNvmxdizc", false);
                m_WardenTerminal.AddLine($"$90/E69B A9 FF FF    LDA #$FFFF              A:0000 X:FFFE Y:0000 P:eNvmxdizc", false);
                m_WardenTerminal.AddLine($"$90/E69E 8D 28 0A    STA $0A28  [$90:0A28]   A:FFFF X:FFFE Y:0000 P:eNvmxdizc", false);
                m_WardenTerminal.AddLine($"$90/E6A1 8D 2A 0A    STA $0A2A  [$90:0A2A]   A:FFFF X:FFFE Y:0000 P:eNvmxdizc", false);
                m_WardenTerminal.AddLine($"$90/E6A4 8D 2C 0A    STA $0A2C  [$90:0A2C]   A:FFFF X:FFFE Y:0000 P:eNvmxdizc", false);
                m_WardenTerminal.AddLine($"*** IRQ", true);
                m_WardenTerminal.AddLine($"IT IS NOW SAFE TO RESTART YOUR DEVICE.");
                m_WardenTerminal.m_command.CustomLineText = "PRESS ANY KEY TO CONTINUE...";
            }
            else m_BlueScreen.active = false;

            if (!SNet.IsMaster) NetworkAPI.InvokeEvent("C1ClientRequestInfo", 0, SNet_ChannelType.GameOrderCritical);
        }

        public void OnObjectiveCompleted()
        {
        }

        public void OnTerminalStateChange(LG_ComputerTerminal terminal, TERM_State state)
        {
            if (terminal != m_WardenTerminal || !m_BlueScreen.active) return;
            L.Debug($"OnTerminalStateChange() TERM_State of {state}");
            NetworkAPI.InvokeEvent("C1DisableBSOD", 0);
            DisableBSOD();
        }
        public static void RecieveDisableBSOD(ulong x, byte y)
        {
            L.Debug("Recieved DisableBSOD packet!");
            Manager_CustomLevelBehavior.C1.DisableBSOD();
        }
        public void DisableBSOD()
        {
            L.Debug($"Deactivating BlueScreen and changing state to ReactorError");
            m_BlueScreen.active = false;
            m_WardenTerminal.m_command.ClearOutputQueueAndScreenBuffer();
            m_WardenTerminal.ChangeState(TERM_State.ReactorError);
            m_WardenTerminal.m_command.ResetOutput();
        }
        public void SetupCaptchaPuzzle(LG_ComputerTerminal terminal)
        {
            if (terminal != m_WardenTerminal) return;
            L.Debug("SetupCaptchaPuzzle()");

            L.Debug("Changing terminal state to awake. Does this fix the input bug?");
            m_WardenTerminal.IsWaitingForAnyKeyInLinePause = false;

            if (!m_WardenTerminal.CanInteract())
            {
                L.Debug("Cant interact with the terminal!!??!?!?!?!?! tf bro tf tf");
                m_WardenTerminal.ChangeState(TERM_State.Awake);
                L.Debug("We changed the state to awake, hopefully that fixes it i guess");
            }

            var waveData = new Il2CppSystem.Collections.Generic.List<GenericEnemyWaveData>();
            var surivivalWave = new GenericEnemyWaveData()
            {
                IntelMessage = 0,
                SpawnDelay = 1,
                TriggerAlarm = true,
                WavePopulation = 1,
                WaveSettings = 1,
            };
            waveData.Add(surivivalWave);

            L.Debug($"Triggering enemy waves! Population {waveData[0].WavePopulation}, Settings {waveData[0].WaveSettings}");
            WardenObjectiveManager.TriggerEnemyWaves(waveData, m_WardenTerminal.SpawnNode);

            L.Debug("Printing lines to terminal");
            m_WardenTerminal.AllowPressAnyKeyToContinue = false;
            m_WardenTerminal.AddLine("");
            m_WardenTerminal.AddLine("");
            m_WardenTerminal.AddLine("<color=red>PROTOCOL E_471<///Typeof(SecurityVerification)> HUMAN VERIFICATION REQUIRED</color>");
            m_WardenTerminal.AddLine("Use Command <color=orange>INPUT_VERIFY [VERIFICATION CODE 1] [VERIFICATION CODE 2]</color> to proceed", false);

            L.Debug("Adding UniqueCommand1 to terminal");
            m_WardenTerminal.m_command.AddCommand(TERM_Command.UniqueCommand1, "INPUT_VERIFY", "Input a human verification code");

            L.Debug("Activating CaptchaHolder");
            m_CaptchaHolder.active = true;
        }

        public void TryVerifyCaptcha(LG_ComputerTerminal terminal, TERM_Command command, string param1, string param2)
        {
            if (terminal != m_WardenTerminal || command != TERM_Command.UniqueCommand1) return;

            L.Debug($"TryVerifyCaptcha() Command input of {param1} {param2} | Expected input of {m_CaptchaKeys[m_CaptchaIndex]}");
            var verification = $"{param1} {param2}".ToUpper();
            var result = verification == m_CaptchaKeys[m_CaptchaIndex].ToUpper();

            L.Debug($"Is the captcha correct? {result}");
            L.Debug("Sending the result as a packet");

            var packet = new pVerifyCaptcha();
            packet.IsCaptchaCorrect = result;
            NetworkAPI.InvokeEvent("C1VerifyCaptcha", packet);

            VerifyCaptcha(result);
        }

        public static void SyncRecieveVerifyCaptcha(ulong x, pVerifyCaptcha input)
        {
            L.Debug($"Recieved verify captcha packet! Is Captcha Correct? {input.IsCaptchaCorrect}");
            Manager_CustomLevelBehavior.C1.VerifyCaptcha(input.IsCaptchaCorrect);
        }

        public void VerifyCaptcha(bool isCaptchaCorrect)
        {
            if (isCaptchaCorrect)
            {
                L.Debug("Captcha verification was successful");
                L.Debug($"{m_CaptchaCount} out of 3 rounds completed");
                m_WardenTerminal.AddLine("");
                m_WardenTerminal.AddLine("");
                m_WardenTerminal.AddLine($"Verification round <color=orange>{m_CaptchaCount} / 3</color> successful.");
                if (m_CaptchaCount < 3)
                {
                    L.Debug("Not enough captchas completed to finish the puzzle. Changing to the next captcha");
                    m_WardenTerminal.AddLine("Verification code reset. Please try again.");
                    m_CaptchaCount += 1;
                    UpdateCaptcha(m_CaptchaIndex + 1);
                }
                else
                {
                    L.Debug("Captcha puzzle is complete! Removing the terminal command");
                    m_WardenTerminal.TrySyncSetCommandRemoved(TERM_Command.UniqueCommand1);
                    m_WardenTerminal.AddLine("Verification sequence successful. Alarm sequence disabled");
                    m_WardenTerminal.AddLine($"Terminal {m_WardenTerminal.m_serialNumber} boot sequence completed.");
                    m_WardenTerminal.m_sound.Post(AK.EVENTS.HACKING_PUZZLE_SUCCESS);

                    L.Debug("Disabling the captchaHolder and activating the extraction chained puzzle");
                    m_CaptchaHolder.active = false;
                    m_AllowExtraction = true;
                    m_ExitGeo.ActivateWinCondition();

                    L.Debug("Executing EventsOnActivate from the active warden objective");
                    WardenObjectiveManager.CheckAndExecuteEventsOnTrigger(WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).EventsOnActivate, eWardenObjectiveEventTrigger.OnStart, true);
                }
            }
            else 
            {
                L.Debug("Rip bozo you fucked up. Reseting the puzzle and updating the captcha");
                m_WardenTerminal.AddLine("");
                m_WardenTerminal.AddLine("");
                m_WardenTerminal.AddLine("<color=red>Verification failed!</color>");
                m_WardenTerminal.AddLine("Verification status reset. Please try again.");
                m_CaptchaCount = 1;
                UpdateCaptcha(m_CaptchaIndex + 1);
            }
        }
        public void OnCollectSmallPickup()
        {
            L.Debug("We just picked up a small pickup! syncing this action :) :) :)");
            NetworkAPI.InvokeEvent("C1SyncSmallPickup", 0);
            UpdateSmallPickupDoor();
        }
        public static void SyncOnCollectSmallPickup(ulong x, byte y)
        {
            L.Debug("Somebody collected a small pickup, updating the objective door for this client");
            Manager_CustomLevelBehavior.C1.UpdateSmallPickupDoor();
        }
        public void UpdateSmallPickupDoor()
        {
            L.Debug("UpdateSmallPickupDoor()");
            var gatherReqCount = WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).Gather_RequiredCount;

            var objetiveSecDoor = LG_LevelBuilder.Current.m_currentFloor.allZones[3].m_sourceGate.GetComponentInChildren<LG_SecurityDoor>();
            var doorLock = objetiveSecDoor.m_locks.TryCast<LG_SecurityDoor_Locks>();

            int collectedItems = 0;
            PlayerBackpack backpack;
            foreach (var player in SNet.Slots.SlottedPlayers)
            {
                if (!player.IsInSlot) continue;

                backpack = PlayerBackpackManager.GetBackpack(player);
                collectedItems += backpack.CountPocketItem(WardenObjectiveManager.ActiveWardenObjective(LG_LayerType.MainLayer).Gather_ItemId);
            }

            doorLock.m_lockedWithNoKeyInteractionText = $"<color=red>://ERR_508: - Zone Security Lockdown://\n<color=orange>[{gatherReqCount - collectedItems}]<color=red> Unauthorized <u>Personell IDs</u> detected within the Sector Inventory Monitoring system.\nPlease contact your security administrator immediately.</color>";
            doorLock.m_intCustomMessage.m_message = doorLock.m_lockedWithNoKeyInteractionText;

            L.Debug($"Updated security door text - {collectedItems} pickups collected, {gatherReqCount - collectedItems} remaining");
        }

        public void UpdateCaptcha(int index)
        {
            L.Debug($"UpdateCaptcha({index})");
            Texture2D captchaMat;
            if (!EntryPoint.s_Captchas.TryGetValue(m_CaptchaKeys[index], out captchaMat))
            {
                index = 0;
                EntryPoint.s_Captchas.TryGetValue(m_CaptchaKeys[index], out captchaMat);
            }
            m_CaptchaHolder.GetComponent<MeshRenderer>().material.mainTexture = captchaMat;
            L.Debug($"New captcha has been set! {m_CaptchaKeys[index]}");
            m_CaptchaIndex = index;
        }

        public void OnCleanup() 
        {
            L.Error("C1 - The Crimson King Cleanup!");

            m_AllowExtraction = false;
            m_WardenTerminal = null;
            GameObject.Destroy(m_BlueScreen);
            m_BlueScreen = null;

            GameObject.Destroy(m_CaptchaHolder);
            m_CaptchaHolder = null;

            m_CaptchaKeys = null;
            m_CaptchaIndex = 0;
            m_CaptchaCount = 1;
            m_ExitGeo = null;

            Patch_WardenObjectiveManager.OnObjectiveComplete -= OnObjectiveCompleted;
            Patch_WardenObjectiveManager.OnStartExpedition -= OnFactoryBuildComplete;
            Patch_WardenObjectiveManager.OnSolvedObjectiveItem -= OnCollectSmallPickup;
            Patch_LG_ComputerTerminal.OnChangedState -= OnTerminalStateChange;
            Patch_LG_ComputerTerminalCommandInterpreter.RecieveCommand -= TryVerifyCaptcha;
            Patch_LG_TERM_ReactorError.OnExitTerminalReactorError -= SetupCaptchaPuzzle;
            Patch_GS_AfterLevel.OnLevelCleanup -= OnCleanup;
        }

        public static void ClientRequestInfo(ulong x, byte y)
        {
            L.Debug("Client request info: Sending warden terminal data to clients");

            var data = new pWardenTerminalInfo();
            data.BlueScreen = Manager_CustomLevelBehavior.C1.m_BlueScreen.active; L.Debug($"Sending BlueScreen of {Manager_CustomLevelBehavior.C1.m_BlueScreen.active}");
            data.Captcha = Manager_CustomLevelBehavior.C1.m_CaptchaHolder.active; L.Debug($"Sending Captcha of {Manager_CustomLevelBehavior.C1.m_CaptchaHolder.active}");
            data.CaptchaCount = Manager_CustomLevelBehavior.C1.m_CaptchaCount; L.Debug($"Sending CaptchaCount of {Manager_CustomLevelBehavior.C1.m_CaptchaCount}");
            data.CaptchaIndex = Manager_CustomLevelBehavior.C1.m_CaptchaIndex; L.Debug($"Sending CaptchaIndex of {Manager_CustomLevelBehavior.C1.m_CaptchaIndex}");

            NetworkAPI.InvokeEvent("C1ProvideTerminalInfo", data);
        }

        public static void OnTerminalInfoRecieved(ulong x, pWardenTerminalInfo data)
        {
            if (SNet.IsMaster) return;
            L.Debug("Recieved data from host. Setting warden terminal data from packet");

            Manager_CustomLevelBehavior.C1.m_BlueScreen.active = data.BlueScreen; L.Debug($"Blue screen set to {data.BlueScreen}");
            Manager_CustomLevelBehavior.C1.m_CaptchaHolder.active = data.Captcha; L.Debug($"Captcha holder set to {data.Captcha}");
            Manager_CustomLevelBehavior.C1.m_CaptchaCount = data.CaptchaCount; L.Debug($"Captcha count set to {data.CaptchaCount}");

            Manager_CustomLevelBehavior.C1.UpdateCaptcha(data.CaptchaIndex); L.Debug($"Captcha index set to {Manager_CustomLevelBehavior.C1.m_CaptchaIndex}");
        }

        public bool m_AllowExtraction = false;
        public LG_ComputerTerminal m_WardenTerminal;
        public GameObject m_BlueScreen;
        public GameObject m_CaptchaHolder;
        public List<string> m_CaptchaKeys = new();
        public int m_CaptchaIndex = 0;
        public int m_CaptchaCount = 1;
        public LG_LevelExitGeo m_ExitGeo;

        public struct pWardenTerminalInfo
        {
            public bool BlueScreen;
            public bool Captcha;
            public int CaptchaIndex;
            public int CaptchaCount;
        }
        public struct pVerifyCaptcha
        {
            public bool IsCaptchaCorrect;
        }
    }
}
