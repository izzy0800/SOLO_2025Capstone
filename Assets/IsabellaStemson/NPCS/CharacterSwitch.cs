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

    public CameraFollower firstPersonCam;

    public bool IsPossessing;
    PlayerMovement playerMovement;

    private Move currentNPCInput;

    private enum TargetType
    {
        player,
        npc
    }
   
    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerParticles = player.GetComponentInChildren<ParticleSystem>();

        SwitchToPlayer();
    }

    public void SwitchToNPC(GameObject newNPC)
    {
        //DisableAllNPCMovement();
        npc = newNPC;
        currentNPCInput = npc.GetComponent<Move>();
        currentNPCInput.canMove = true;

        Swap(false);
    }

    public void SwitchToPlayer()
    {
        Swap(true);
    }

    public void Swap(bool isPlayer)
    {
        if (isPlayer)
        {
            if (currentNPCInput != null)
            {
                currentNPCInput.canMove = false;
                firstPersonCam.canLook = false;
                currentNPCInput.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                SetRenderersVisible(currentNPCInput.gameObject, true);
                firstPersonCam.gameObject.GetComponent<CinemachineBrain>().enabled = true;

                IsPossessing = false;
            }

            player.SetActive(true);
            playerMovement.enabled = true;

        }
        else
        {
            firstPersonCam.orientation = currentNPCInput.orientation;
            firstPersonCam.canLook = true;

            SetRenderersVisible(currentNPCInput.gameObject, false);
            IsPossessing = true;

            currentNPCInput.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            firstPersonCam.gameObject.GetComponent<CinemachineBrain>().enabled = false;
            //currentNPCInput.gameObject

            playerMovement.enabled = false;
            player.SetActive(false);
        }
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
                if (!isEnabled)
                {
                    charMover.SetInput(Vector2.zero, character.transform.position + character.transform.forward, false, false);
                }
            }
            else
            {
                var playerMove = character.GetComponent<PlayerMovement>();
                if (playerMove) playerMove.enabled = isEnabled;
            }
            var rb = character.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (!isEnabled)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }
                else
                {
                    rb.isKinematic = false;
                }
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
        if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive)
        {
            // Block ALL inputs during dialog, including Tab
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("Cannot switch characters during dialog!");
            }
            return; 
        }

        if (IsPossessing && Input.GetKeyDown(KeyCode.Tab))
        {
            Swap(true);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            
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
        //Debug.Log("Disabled all zone cameras for possession");
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
                //Debug.Log($"Reactivated camera for zone: {switcher.gameObject.name}");
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

                var rb = npcScript.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    if (!rb.isKinematic)
                    {
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                    rb.isKinematic = true;
                }
            }
        }
    }

    private void ForceStopNPC(GameObject npcObject)
    {
        if (npcObject == null) return;

        var moveInput = npcObject.GetComponent<MovePlayerInput>();
        if (moveInput)
        {
            moveInput.enabled = false;
            Destroy(moveInput);
        }

        var charMover = npcObject.GetComponent<CharacterMover>();
        if (charMover)
        {
            charMover.SetInput(Vector2.zero, npcObject.transform.position, false, false);
            charMover.enabled = false;
        }

        var charController = npcObject.GetComponent<CharacterController>();
        if (charController)
        {
            charController.enabled = false;
        }

        var rb = npcObject.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        var camTarget = npcObject.GetComponent<NPCCameraTarget>();
        if (camTarget) Destroy(camTarget);
    }

}
