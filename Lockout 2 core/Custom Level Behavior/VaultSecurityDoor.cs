using AIGraph;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace Lockout_2_core.Custom_Level_Behavior
{
    class VaultSecurityDoor
    {
        public static void Setup(GameObject vaultDoor)
        {
            var securityDoor = vaultDoor.AddComponent<LG_SecurityDoor>();
            var door_Sync = vaultDoor.AddComponent<LG_Door_Sync>();
            var securityDoor_Anim = vaultDoor.AddComponent<LG_SecurityDoor_Anim>();
            var door_Graphics = vaultDoor.AddComponent<LG_Door_Graphics>();
            var aig_DoorInsert = vaultDoor.AddComponent<AIG_DoorInsert>();
            var navMeshObstable = vaultDoor.AddComponent<NavMeshObstacle>();
            var portalDivider = vaultDoor.AddComponent<LG_PortalDivider>();
            var securityDoor_Locks = vaultDoor.AddComponent<LG_SecurityDoor_Locks>();
            var genericTerminalItem = vaultDoor.AddComponent<LG_GenericTerminalItem>();

            var crossing = vaultDoor.transform.FindChild("Crossing").gameObject;

            var mapLookAtRevealer = crossing.transform.FindChild("MapLookatRevealer").gameObject.AddComponent<LG_DoorMapLookatRevealer>();
            mapLookAtRevealer.m_collider = mapLookAtRevealer.gameObject.GetComponent<Collider>();
            var mapLookAtRevealer_Ping = mapLookAtRevealer.gameObject.AddComponent<PlayerPingTarget>();
            mapLookAtRevealer_Ping.m_pingTargetStyle = eNavMarkerStyle.PlayerPingDoor;

            var doorBladeCuller = vaultDoor.transform.FindChild("Door").gameObject.AddComponent<LG_DoorBladeCuller>();

            var interactUseKeyItem = crossing.transform.FindChild("Interaction_Use_KeyItem").gameObject.AddComponent<Interact_RequireKeyItem>();
            interactUseKeyItem.m_colliderToOwn = interactUseKeyItem.gameObject.GetComponent<Collider>();
            interactUseKeyItem.AllowTriggerWithCarryItem = true;
            interactUseKeyItem.AbortOnDotOrDistanceDiff = true;
            interactUseKeyItem.InteractDuration = 0.6f;
            interactUseKeyItem.InteractionMessage = "Surprise";
            interactUseKeyItem.m_maxMoveDisAllowed = 2;
            interactUseKeyItem.m_minCamDotAllowed = 0.5f;
            interactUseKeyItem.m_interactors = new(0);
            interactUseKeyItem.SFXInteractCancel = 95101972;
            interactUseKeyItem.SFXInteractEnd = 2804207025;
            interactUseKeyItem.SFXInteractStart = 3005424020;
            interactUseKeyItem.SFXInteractTrigger = 0;
            interactUseKeyItem.RequireCollisionCheck = true;
            interactUseKeyItem.m_isActive = true;
            interactUseKeyItem.gameObject.layer = LayerMask.NameToLayer("Interaction");
            var interactUseKeyItem_Ping = crossing.transform.FindChild("Interaction_Use_KeyItem").gameObject.AddComponent<PlayerPingTarget>();
            interactUseKeyItem_Ping.m_pingTargetStyle = eNavMarkerStyle.PlayerPingDoor;

            var interactHack = crossing.transform.FindChild("Interaction_Hack").gameObject.AddComponent<Interact_Hack>();
            interactHack.m_colliderToOwn = interactUseKeyItem.gameObject.GetComponent<Collider>();
            interactHack.AllowTriggerWithCarryItem = true;
            interactHack.AbortOnDotOrDistanceDiff = true;
            interactHack.m_waitForWeaponTimeout = 3;
            interactHack.InteractDuration = 0.6f;
            interactHack.InteractionMessage = "Surprise";
            interactHack.m_maxMoveDisAllowed = 2;
            interactHack.m_minCamDotAllowed = 0.5f;
            interactHack.m_interactors = new(0);
            interactHack.SFXInteractCancel = 95101972;
            interactHack.SFXInteractEnd = 2804207025;
            interactHack.SFXInteractStart = 3005424020;
            interactHack.SFXInteractTrigger = 0;
            interactHack.RequireCollisionCheck = true;
            interactHack.m_isActive = true;
            interactHack.gameObject.layer = LayerMask.NameToLayer("Interaction");
            var interactHack_Ping = crossing.transform.FindChild("Interaction_Hack").gameObject.AddComponent<PlayerPingTarget>();
            interactHack_Ping.m_pingTargetStyle = eNavMarkerStyle.PlayerPingDoor;

            var interactOpenOrActivate = crossing.transform.FindChild("Interaction_Open_Or_Activate").gameObject.AddComponent<Interact_Timed>();
            interactOpenOrActivate.m_colliderToOwn = interactUseKeyItem.gameObject.GetComponent<Collider>();
            interactOpenOrActivate.AllowTriggerWithCarryItem = true;
            interactOpenOrActivate.AbortOnDotOrDistanceDiff = true;
            interactOpenOrActivate.InteractDuration = 0.6f;
            interactOpenOrActivate.InteractionMessage = "Surprise";
            interactOpenOrActivate.m_maxMoveDisAllowed = 2;
            interactOpenOrActivate.m_minCamDotAllowed = 0.5f;
            interactOpenOrActivate.m_interactors = new(0);
            interactOpenOrActivate.SFXInteractCancel = 95101972;
            interactOpenOrActivate.SFXInteractEnd = 2804207025;
            interactOpenOrActivate.SFXInteractStart = 3005424020;
            interactOpenOrActivate.SFXInteractTrigger = 0;
            interactOpenOrActivate.RequireCollisionCheck = true;
            interactOpenOrActivate.m_isActive = true;
            interactOpenOrActivate.gameObject.layer = LayerMask.NameToLayer("Interaction");
            var interactOpenOrActivate_Ping = crossing.transform.FindChild("Interaction_Open_Or_Activate").gameObject.AddComponent<PlayerPingTarget>();
            interactOpenOrActivate_Ping.m_pingTargetStyle = eNavMarkerStyle.PlayerPingDoor;

            var interactMessage = crossing.transform.FindChild("Interaction_Message").gameObject.AddComponent<Interact_MessageOnScreen>();
            interactMessage.m_colliderToOwn = interactUseKeyItem.gameObject.GetComponent<Collider>();
            interactMessage.AllowTriggerWithCarryItem = true;
            interactMessage.MessageType = eMessageOnScreenType.OjbectiveMessage;
            interactMessage.m_message = "";
            interactMessage.RequireCollisionCheck = true;
            interactMessage.m_isActive = true;
            interactMessage.gameObject.layer = LayerMask.NameToLayer("Interaction");
            var interactMessage_Ping = crossing.transform.FindChild("Interaction_Message").gameObject.AddComponent<PlayerPingTarget>();
            interactMessage_Ping.m_pingTargetStyle = eNavMarkerStyle.PlayerPingDoor;

            AIG_FreeNode freeNode;
            var freeNodes = crossing.transform.FindChild("FreeNodes").gameObject;
            aig_DoorInsert.m_nodes = new(freeNodes.transform.childCount);
            for (var i = 0; i < freeNodes.transform.childCount; i ++)
            {
                freeNode = freeNodes.transform.GetChild(i).gameObject.AddComponent<AIG_FreeNode>();
                aig_DoorInsert.m_nodes[i] = freeNode;
            }



            securityDoor.m_animComp = securityDoor_Anim;
            securityDoor.m_bioScanAlign = crossing.transform.FindChild("BioscanAlign");
            securityDoor.m_checkpointMarker = null;
            securityDoor.m_graphicsComp = door_Graphics;
            securityDoor.m_keycardLockPrefab = null;
            securityDoor.m_keypadAlign = vaultDoor.transform.FindChild("CatWalk/prop_wallPanel_d/g_prop_wallPanel_d");
            securityDoor.m_locksComp = securityDoor_Locks;
            securityDoor.m_mapLookatRevealer = mapLookAtRevealer;
            securityDoor.m_securityDoorType = eSecurityDoorType.Bulkhead;
            securityDoor.m_syncComp = door_Sync;
            securityDoor.m_terminalInterfaceComp = genericTerminalItem;

            securityDoor_Anim.m_animator = vaultDoor.GetComponent<Animator>();
            securityDoor_Anim.m_useLargeSounds = true;
            securityDoor_Anim.m_bulkheadLockPins = new(0);
            securityDoor_Anim.m_bulkheadLightStrips = new(0);

            door_Graphics.m_enabledIfActiveEnemyWave = new(0);
            door_Graphics.m_graphicalModes = new(0);
            door_Graphics.m_serialNoTexts = new(0);
            door_Graphics.m_layerSpecificsMain = new(0);
            door_Graphics.m_layerSpecificsSecondary = new(0);
            door_Graphics.m_layerSpecificsThird = new(0);

            navMeshObstable.carveOnlyStationary = true;
            navMeshObstable.carving = true;
            navMeshObstable.carvingMoveThreshold = 0.1f;
            navMeshObstable.carvingTimeToStationary = 0.5f;
            navMeshObstable.center = new(0, 4, 0);
            navMeshObstable.height = 4;
            navMeshObstable.radius = 4;
            navMeshObstable.shape = NavMeshObstacleShape.Box;
            navMeshObstable.size = new(8, 8, 2);
            navMeshObstable.velocity = new(0, 0, 0);

            portalDivider.m_inFront = vaultDoor.transform.FindChild("InFront");
            portalDivider.m_behind = vaultDoor.transform.FindChild("Behind");
            portalDivider.m_crossing = vaultDoor.transform.FindChild("Crossing");

            securityDoor_Locks.m_intCustomMessage = interactMessage;
            securityDoor_Locks.m_intHack = interactHack;
            securityDoor_Locks.m_intOpenDoor = interactOpenOrActivate;
            securityDoor_Locks.m_intUseKeyItem = interactUseKeyItem;

            doorBladeCuller.m_destroyedVersions = new(0);
            doorBladeCuller.m_destroyRenderers = new(0);
            doorBladeCuller.m_destroyRenderersShadow = new(0);
            doorBladeCuller.m_destroySpecials = new(0);
            doorBladeCuller.m_boundsMode = CullingSystem.C_UpdateCuller.ComputeBoundsMode.CustomBounds;
            doorBladeCuller.m_customBoundsSize = new(8, 8, 2);
            doorBladeCuller.m_customBoundsOffset = new(0, 4, 0);
        }
    }
}
