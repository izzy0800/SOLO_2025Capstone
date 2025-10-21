using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    public CharacterSwitch characterSwitch;

    public Transform holdPoint;
    public float pickupRange = 4f;

    private GameObject heldObject;
    private bool canPickUp = false;
    private GameObject objectToPickUp;
    
    void Start()
    {
        if (holdPoint == null)
        {
            Debug.LogError("Holdpoint is NOT assigned in the inspector: " + gameObject.name);
        }

        if (characterSwitch == null)
        {
            characterSwitch = FindObjectOfType<CharacterSwitch>();
            if (characterSwitch == null)
            {
                Debug.LogError("CharacterSwitch not found in scene!");
            }
        }

    }

    void Update()
    {
        bool isThisNPCPossessed = (characterSwitch != null &&
                                     characterSwitch.IsPossessing &&
                                     characterSwitch.npc == this.gameObject);

        bool isPlayer = (characterSwitch != null &&
                        !characterSwitch.IsPossessing &&
                        characterSwitch.player == this.gameObject);

        if (!isThisNPCPossessed && !isPlayer)
        {
            return; 
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null && canPickUp && objectToPickUp != null)
            {
                Debug.Log($"{gameObject.name} trying to pickup {objectToPickUp.name}");
                Pickup();
            }
            else if (heldObject != null)
            {
                Debug.Log($"{gameObject.name} dropping {heldObject.name}");
                Drop();
            }
            else if (!canPickUp)
            {
                Debug.Log($"{gameObject.name}: No object in range to pick up");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup") && heldObject == null)
        {
            canPickUp = true;
            objectToPickUp = other.gameObject;
            Debug.Log($"{gameObject.name} can pick up {other.gameObject.name}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup") && other.gameObject == objectToPickUp)
        {
            canPickUp = false;
            objectToPickUp = null;
            Debug.Log($"{gameObject.name} moved away from {other.gameObject.name}");
        }
    }


    void Pickup()
    {
        if (objectToPickUp == null)
        {
            Debug.LogError("objectToPickUp is null when trying to pick up.");
            return;
        }
        if (holdPoint == null)
        {
            Debug.LogError("HoldPoint is not assigned in the inpsector.");
            return;
        }

        heldObject = objectToPickUp;

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }
        else
        {
            Debug.LogWarning($"No Rigidbody on {heldObject.name}, continuing anyway");
        }

        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        Collider[] colliders = heldObject.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        objectToPickUp = null;
        canPickUp = false;

        Debug.Log("Successfully picked up: " + heldObject.name);

    }

    void Drop()
    {

        if (heldObject == null)
        {
            Debug.LogWarning("Tried to drop but no object is currently held.");
            return;
        }

        Collider[] colliders = heldObject.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        heldObject.transform.SetParent(null);
        Debug.Log($"{gameObject.name} dropped {heldObject.name}");
        heldObject = null;
    }
}
