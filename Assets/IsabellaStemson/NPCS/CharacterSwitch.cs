using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Cinemachine;
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

    //private Rigidbody rb;

    private enum TargetType
    {
        player,
        npc
    }
   
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //rb.isKinematic = true;

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
        npc = newNPC;
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

        if (npc != null)
        {

            //rb.isKinematic = false;


            var npcMovement = npc.GetComponent<PlayerMovement>();
            if (npcMovement != null)
            {
                npcMovement.allowVerticalMovement = false;
            }

            var npcScript = npc.GetComponent<NPCscript>();
            if (npcScript != null)
            {
                npcScript.SetSpriteVisible(!isNPC);

                SetRenderersVisible(npc, !isNPC);

                if (isNPC && firstPersonCam != null)
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
        }

        if (!isNPC)
        {
            if (npc != null)
            {
               // rb.isKinematic = true;

                var npcScript = npc.GetComponent<NPCscript>();
                if (npcScript != null)
                {
                    npcScript.SetSpriteVisible(true);
                    SetRenderersVisible(npc, true);
                    npc.GetComponent<Rigidbody>().isKinematic = true;
                }
            }

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
        if (npc != null)
            EnableControl(npc, isNPC);

        playerMovement.allowVerticalMovement = !isNPC;
        playerParticles.gameObject.SetActive(!isNPC);
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

    private void EnableControl(GameObject character, bool isEnabled)
    {
        if (character.GetComponent<PlayerMovement>())
            character.GetComponent<PlayerMovement>().enabled = isEnabled;

        if (character.GetComponent<NPCscript>())
        {
            character.GetComponent<NPCscript>().col.isTrigger = !isEnabled;
        }

        if (character.GetComponent<PickUpSystem>())
            character.GetComponent<PickUpSystem>().enabled = isEnabled;
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
}
