using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    public CharacterSwitch characterSwitch;

    public Transform holdPoint;
    public float pickupRange = 3f;

    private GameObject heldObject;
    private bool canPickUp = false;
    private GameObject objectToPickUp;
    
    
    private PlayerMovement playerMovment;

    // Start is called before the first frame update
    void Start()
    {

      //Debug.Log("PickUpSystem script initialised on: " + gameObject.name);
        playerMovment = GetComponent<PlayerMovement>();

        if (holdPoint == null)
        {
            Debug.LogError("Holdpoint is NOT assigned in the inspector: " + gameObject.name);
        }

        if (playerMovment == null)
        {
            Debug.LogError("PlayerMovement component is missing on: " + gameObject);
        }

    }

    // Update is called once per frame 
    void Update()
    {
      //Debug.Log("PickUpSystem update is running");
               
        // This only allows pickup if the player is possessing
        if (playerMovment != null && playerMovment.allowVerticalMovement) 
            return;


        if (Input.GetKeyDown(KeyCode.E))
        {

          //Debug.Log("E key Pressed");

           

            if (heldObject == null && canPickUp)
            {
                Pickup();
            }
            else if (heldObject != null)
            {
                Drop();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
          //Debug.Log("Enter trigger with Pickup Object: " + other.name);
            canPickUp = true;
            objectToPickUp = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
          //Debug.Log("Exited trigger with Pickup object: " + other.name);
            canPickUp = false;
            objectToPickUp = null;
        }
    }


    void Pickup()
    {
      //Debug.Log("Picking up object: " + objectToPickUp.name);
      //Debug.Log("Has Rigidbody: " + (objectToPickUp.GetComponent<Rigidbody>() != null));
      //Debug.Log("Has Rigidbody in child: " + (objectToPickUp.GetComponentInChildren<Rigidbody>() != null));

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
      //Debug.Log("picking Up: " + heldObject.name); 
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }
        else
        {
            Debug.LogError("Rigidbody missing from pickup object: " + heldObject.name);
            return;
        }

        heldObject.transform.SetParent(holdPoint);
        heldObject.transform.localPosition = Vector3.zero;

    }

    void Drop()
    {

        if (heldObject == null)
        {
            Debug.LogWarning("Tried to drop but no object is currently held.");
            return;
        }

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        heldObject.transform.SetParent(null);
        heldObject = null;
    }
}
