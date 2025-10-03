using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera assignedCamera;

    private void Start()
    {
        if (assignedCamera == null)
        {
            Debug.LogError($"No camera assigned to CamSwitcher on {gameObject.name}!");
        }

        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
        {
            Debug.LogError($"CamSwitcher on {gameObject.name} needs a trigger collider!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered camera zone: " + gameObject.name);

            CharacterSwitch charSwitch = FindObjectOfType<CharacterSwitch>();
            if (charSwitch != null && charSwitch.IsPossessing)
            {
                Debug.Log("Not switching camera - currently possessing NPC");
                return;
            }

            CamSwitcher[] allSwitchers = FindObjectsOfType<CamSwitcher>();
            foreach (var switcher in allSwitchers)
            {
                if (switcher != this && switcher.assignedCamera != null)
                {
                    switcher.assignedCamera.Priority = 0;
                }
            }

            assignedCamera.Priority = 10;
            Debug.Log("Set camera priority to 10");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited camera zone: " + gameObject.name);

            CharacterSwitch charSwitch = FindObjectOfType<CharacterSwitch>();
            if (charSwitch != null && charSwitch.IsPossessing)
            {
                return;
            }

            assignedCamera.Priority = 0;
            Debug.Log("Set camera priority to 0");
        }
    }

    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(1, 1, 0, 0.3f); 
            BoxCollider box = col as BoxCollider;
            if (box != null)
            {
                Gizmos.DrawWireCube(transform.position + box.center, box.size);
            }
        }
    }
}

