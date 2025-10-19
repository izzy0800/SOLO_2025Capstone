using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Cinemachine;
using Controller;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class CharacterSwitch : CryptidUtils
{
    public GameObject player;
    public GameObject npc;
    ParticleSystem playerParticles;

    public CinemachineVirtualCamera firstPersonCam;

    private bool isControllingNPC = false;
    public bool IsPossessing => isControllingNPC;
    PlayerMovement playerMovement;

    private MovePlayerInput currentNPCInput;
    private CharacterMover currentNPCMover;

    private enum TargetType
    {
        player,
        npc
    }
   
    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerParticles = player.GetComponentInChildren<ParticleSystem>();
        if (firstPersonCam != null)
        {
            firstPersonCam.Priority = 0;
            firstPersonCam.Follow = null;
            firstPersonCam.LookAt = null;
        }

        SwitchToPlayer();
    }

    public void SwitchToNPC(GameObject newNPC)
    {
        DisableAllNPCMovement();

        npc = newNPC;

        currentNPCInput = npc.GetComponent<MovePlayerInput>();
        currentNPCMover = npc.GetComponent<CharacterMover>();

        Switch(TargetType.npc);
    }

    public void SwitchToPlayer()
    {
        Switch(TargetType.player);
    }

    private void Switch(TargetType type)
    {
        bool isNPC = type == TargetType.npc;
        isControllingNPC = isNPC;

        GameObject previousNPC = npc;

        if (!isNPC && previousNPC != null)
        {
            EnableControl(previousNPC, false);

            var npcScript = previousNPC.GetComponent<NPCscript>();
            if (npcScript != null)
            {
                npcScript.SetSpriteVisible(true);
                SetRenderersVisible(previousNPC, true);

                var rb = previousNPC.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;
            }
        }

        if (isNPC && npc != null)
        {
            var npcScript = npc.GetComponent<NPCscript>();
            if (npcScript != null)
            {
                npcScript.SetSpriteVisible(false);
                SetRenderersVisible(npc, false);

                if (firstPersonCam != null)
                {
                    Transform cameraMount = npc.transform.Find("CameraMount");
                    if (cameraMount == null)
                    {
                        GameObject mountObj = new GameObject("CameraMount");
                        mountObj.transform.SetParent(npc.transform);
                        mountObj.transform.localPosition = new Vector3(0, npcScript.height, 0.2f);
                        mountObj.transform.localRotation = Quaternion.identity;
                        cameraMount = mountObj.transform;
                    }

                    firstPersonCam.Follow = cameraMount;

                    GameObject lookTarget = GameObject.Find("FirstPersonLookTarget");
                    if (lookTarget == null)
                    {
                        lookTarget = new GameObject("FirstPersonLookTarget");
                    }
                    lookTarget.transform.SetParent(cameraMount);
                    lookTarget.transform.localPosition = new Vector3(0, 0, 5f);
                    lookTarget.transform.localRotation = Quaternion.identity;

                    firstPersonCam.LookAt = lookTarget.transform;

                    DisableAllZoneCameras();
                    firstPersonCam.Priority = 20;

                    Debug.Log($"First-person camera following: {cameraMount.name} at position {cameraMount.position}");
                }
            }

            EnableControl(npc, true);
        }

        if (!isNPC)
        {
            if (firstPersonCam != null)
            {
                firstPersonCam.Priority = 0;
                firstPersonCam.Follow = null;
                firstPersonCam.LookAt = null;

                GameObject lookTarget = GameObject.Find("FirstPersonLookTarget");
                if (lookTarget != null)
                {
                    Destroy(lookTarget);
                }
            }

            ReactivateZoneCameraForPlayer();
        }

        EnableControl(player, !isNPC);

        playerMovement.allowVerticalMovement = !isNPC;
        playerParticles.gameObject.SetActive(!isNPC);
    }

    private void EnableControl(GameObject character, bool isEnabled)
    {
        if (character == player)
        {
            var playerMove = character.GetComponent<PlayerMovement>();
            if (playerMove) playerMove.enabled = isEnabled;
        }
        else if (character == npc)
        {
            var moveInput = character.GetComponent<MovePlayerInput>();
            var charMover = character.GetComponent<CharacterMover>();

            if (moveInput && charMover)
            {
                moveInput.enabled = isEnabled;
                charMover.enabled = isEnabled;
            }
            else
            {
                var playerMove = character.GetComponent<PlayerMovement>();
                if (playerMove) playerMove.enabled = isEnabled;
            }
        }
        var npcScript = character.GetComponent<NPCscript>();
        if (npcScript && npcScript.col != null)
        {
            npcScript.col.isTrigger = !isEnabled;
        }
        var pickupSystem = character.GetComponent<PickUpSystem>();
        if (pickupSystem)
        {
            pickupSystem.enabled = isEnabled;
        }
    }

    private void SetRenderersVisible(GameObject target, bool visible)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = visible;
        }

        SpriteRenderer[] sprites = target.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in sprites)
        {
            sr.enabled = visible;
        }
    }

    private void Update()
    {
        //Pressing 'Tab' will toggle back to the player
        if (isControllingNPC && Input.GetKeyDown(KeyCode.Tab))
        {
            Switch(TargetType.player);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log($"Is Possessing: {IsPossessing}");
            Debug.Log($"FirstPersonCam Priority: {firstPersonCam.Priority}");
            Debug.Log($"FirstPersonCam Follow: {firstPersonCam.Follow}");
            Debug.Log($"FirstPersonCam Position: {firstPersonCam.transform.position}");
        }
    }

    private void DisableAllZoneCameras()
    {
        CamSwitcher[] allSwitchers = FindObjectsOfType<CamSwitcher>();
        foreach (var switcher in allSwitchers)
        {
            if (switcher.assignedCamera != null)
            {
                switcher.assignedCamera.Priority = 0;
            }
        }
        Debug.Log("Disabled all zone cameras for possession");
    }

    private void ReactivateZoneCameraForPlayer()
    {
        CamSwitcher[] allSwitchers = FindObjectsOfType<CamSwitcher>();
        foreach (var switcher in allSwitchers)
        {
            Collider zoneCollider = switcher.GetComponent<Collider>();
            if (zoneCollider != null && zoneCollider.bounds.Contains(player.transform.position))
            {
                switcher.assignedCamera.Priority = 10;
                Debug.Log($"Reactivated camera for zone: {switcher.gameObject.name}");
                break;
            }
        }
    }

    private void DisableAllNPCMovement()
    {
        NPCscript[] allNPCs = FindObjectsOfType<NPCscript>();
        foreach (var npcScript in allNPCs)
        {
            if (npcScript.gameObject != npc)
            {
                var moveInput = npcScript.GetComponent<MovePlayerInput>();
                var charMover = npcScript.GetComponent<CharacterMover>();
                var playerMove = npcScript.GetComponent<PlayerMovement>();

                if (moveInput) moveInput.enabled = false;
                if (charMover) charMover.enabled = false;
                if (playerMove) playerMove.enabled = false;
            }
        }
    }
}
