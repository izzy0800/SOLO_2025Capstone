using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera assignedCamera;
    private static CinemachineVirtualCamera currentActiveCamera;

    private void Start()
    {
        if (assignedCamera == null)
        {
            Debug.LogError($"No camera assigned to CamSwitcher on {gameObject.name}!");
            return;
        }

        assignedCamera.Priority = 0;

        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
        {
            Debug.LogError($"CamSwitcher on {gameObject.name} needs a trigger collider!");
        }

        Debug.Log($"CamSwitcher initialized on {gameObject.name} with camera {assignedCamera.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"=== CAMERA ZONE TRIGGERED ===");
            Debug.Log($"Player entered zone: {gameObject.name}");
            Debug.Log($"Assigned camera: {assignedCamera.name}");

            CharacterSwitch charSwitch = FindObjectOfType<CharacterSwitch>();
            if (charSwitch != null && charSwitch.IsPossessing)
            {
                Debug.Log("BLOCKED: Currently possessing NPC - not switching cameras");
                return;
            }

            CinemachineVirtualCamera[] allVirtualCameras = FindObjectsOfType<CinemachineVirtualCamera>();
            foreach (var cam in allVirtualCameras)
            {
                if (cam != assignedCamera && cam != charSwitch?.firstPersonCam)
                {
                    cam.Priority = 0;
                    Debug.Log($"Deactivated camera: {cam.name} (Priority set to 0)");
                }
            }

            assignedCamera.Priority = 10;
            currentActiveCamera = assignedCamera;

            StartCoroutine(ForceCinemachineUpdate());

            Debug.Log($"ACTIVATED camera: {assignedCamera.name} (Priority set to 10)");
            Debug.Log($"Camera position: {assignedCamera.transform.position}");
            Debug.Log($"=== END CAMERA SWITCH ===");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player exited zone: {gameObject.name}");

            CharacterSwitch charSwitch = FindObjectOfType<CharacterSwitch>();
            if (charSwitch != null && charSwitch.IsPossessing)
            {
                return;
            }

            if (currentActiveCamera == assignedCamera)
            {
                Debug.Log($"Player left active zone {gameObject.name}");
            }
        }
    }

    private IEnumerator ForceCinemachineUpdate()
    {
        yield return null;

        var brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            var activeVcam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
            if (activeVcam != null)
            {
                Debug.Log($"Cinemachine Brain is now using: {activeVcam.name}");
                if (activeVcam != assignedCamera)
                {
                    Debug.LogWarning($"WARNING: Brain didn't switch to our camera! Expected {assignedCamera.name}, got {activeVcam.name}");
                    Debug.LogWarning($"Check if another camera has higher priority or if there's a Live Timeline overriding");
                }
            }
        }
        else
        {
            Debug.LogError("No CinemachineBrain found on Main Camera!");
        }
    }

    public void ForceActivate()
    {
        CinemachineVirtualCamera[] allCameras = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var cam in allCameras)
        {
            if (cam != assignedCamera)
            {
                cam.Priority = 0;
            }
        }

        assignedCamera.Priority = 10;
        currentActiveCamera = assignedCamera;
        Debug.Log($"Force activated camera: {assignedCamera.name}");
    }

    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            if (currentActiveCamera == assignedCamera)
            {
                Gizmos.color = new Color(0, 1, 0, 0.3f); 
            }
            else
            {
                Gizmos.color = new Color(1, 1, 0, 0.3f); 
            }

            BoxCollider box = col as BoxCollider;
            if (box != null)
            {
                Matrix4x4 oldMatrix = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawCube(box.center, box.size);
                Gizmos.DrawWireCube(box.center, box.size);
                Gizmos.matrix = oldMatrix;
            }
        }
    }
}