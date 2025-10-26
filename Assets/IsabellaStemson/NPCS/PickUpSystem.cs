using Benjathemaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    [Header("References")]
    public CharacterSwitch characterSwitch;
    public Transform holdPoint;
    public float pickupRange = 4f;

    [Header("Giving Items")]
    [SerializeField] private KeyCode giveKey = KeyCode.G;
    [SerializeField] private float giveRange = 2f;

    private GameObject heldObject;
    private bool canPickUp = false;
    private GameObject objectToPickUp;
    private bool showingPickupPrompt = false;
    private bool showingGivePrompt = false;
    private NPCItemReceiver nearbyReceiver;

    void Start()
    {
        if (holdPoint == null)
        {
            Debug.LogError("Holdpoint is NOT assigned in the inspector: " + gameObject.name);
        }

        if (characterSwitch == null)
        {
            characterSwitch = FindObjectOfType<CharacterSwitch>();
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

        if (heldObject != null && isThisNPCPossessed)
        {
            CheckForNearbyReceivers();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null && canPickUp && objectToPickUp != null)
            {
                Pickup();
            }
            else if (heldObject != null)
            {
                if (nearbyReceiver != null)
                {
                    
                    GiveItem();
                }
                else
                {
                    
                    Drop();
                }
            }
        }

        if (heldObject != null && nearbyReceiver != null && Input.GetKeyDown(giveKey))
        {
            GiveItem();
        }

        if (heldObject != null && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    private void CheckForNearbyReceivers()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, giveRange);

        NPCItemReceiver closestReceiver = null;
        float closestDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            NPCItemReceiver receiver = col.GetComponent<NPCItemReceiver>();
            if (receiver != null && receiver.gameObject != this.gameObject)
            {
                float distance = Vector3.Distance(transform.position, receiver.transform.position);
                if (distance < closestDistance && receiver.IsInRange(transform))
                {
                    closestDistance = distance;
                    closestReceiver = receiver;
                }
            }
        }

        if (nearbyReceiver != closestReceiver)
        {
            nearbyReceiver = closestReceiver;
            UpdateGivePrompt();
        }
    }

    private void UpdateGivePrompt()
    {
        if (showingGivePrompt && nearbyReceiver == null)
        {
            if (PossessionPromptUI.Instance != null)
            {
                PossessionPromptUI.Instance.HidePrompt();
            }
            showingGivePrompt = false;
        }
        else if (nearbyReceiver != null && heldObject != null)
        {
            ItemData itemData = heldObject.GetComponent<ItemData>();
            if (itemData != null)
            {
                string promptText;
                if (nearbyReceiver.CanReceiveItem(itemData.itemType))
                {
                    promptText = $"Press E/G to Give {itemData.itemName}";
                }
                else
                {
                    NPCDialogHandler npcDialog = nearbyReceiver.GetComponent<NPCDialogHandler>();
                    string npcName = npcDialog != null ? npcDialog.npcName : nearbyReceiver.name;
                    promptText = $"Press E/G to Give {itemData.itemName}";
                }

                if (PossessionPromptUI.Instance != null)
                {
                    PossessionPromptUI.Instance.ShowGivePrompt(promptText);
                }
                showingGivePrompt = true;
            }
        }
    }

    private void GiveItem()
    {
        if (heldObject == null || nearbyReceiver == null) return;

        if (nearbyReceiver.IsShowingRejection()) return;

        ItemData itemData = heldObject.GetComponent<ItemData>();
        if (itemData != null && nearbyReceiver.CanReceiveItem(itemData.itemType))
        {
            GameObject itemToGive = heldObject;
            heldObject = null;

            if (showingGivePrompt)
            {
                if (PossessionPromptUI.Instance != null)
                {
                    PossessionPromptUI.Instance.HidePrompt();
                }
                showingGivePrompt = false;
            }

            nearbyReceiver.ReceiveItem(itemToGive);
            nearbyReceiver = null;
        }
        else
        {
            
            nearbyReceiver.ReceiveItem(heldObject);
            
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup") && heldObject == null)
        {
            canPickUp = true;
            objectToPickUp = other.gameObject;
            Debug.Log($"{gameObject.name} can pick up {other.gameObject.name}");

            bool isThisNPCPossessed = (characterSwitch != null &&
                                       characterSwitch.IsPossessing &&
                                       characterSwitch.npc == this.gameObject);

            if (isThisNPCPossessed && PossessionPromptUI.Instance != null)
            {
                if (PromptPriorityManager.Instance != null)
                {
                    if (PromptPriorityManager.Instance.RequestPrompt(
                        PromptPriorityManager.PromptType.Pickup, gameObject))
                    {
                        string itemName = objectToPickUp.name;
                        PossessionPromptUI.Instance.ShowPickupPrompt(itemName);
                        showingPickupPrompt = true;
                    }
                }
                else
                {
                    string itemName = objectToPickUp.name;
                    PossessionPromptUI.Instance.ShowPickupPrompt(itemName);
                    showingPickupPrompt = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup") && other.gameObject == objectToPickUp)
        {
            canPickUp = false;
            objectToPickUp = null;
            Debug.Log($"{gameObject.name} moved away from {other.gameObject.name}");

            if (showingPickupPrompt)
            {
                if (PromptPriorityManager.Instance != null)
                {
                    PromptPriorityManager.Instance.ReleasePrompt(gameObject);
                }
                else if (PossessionPromptUI.Instance != null)
                {
                    PossessionPromptUI.Instance.HidePrompt();
                }
                showingPickupPrompt = false;
            }
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
            Debug.LogError("HoldPoint is not assigned in the inspector.");
            return;
        }

        if (showingPickupPrompt)
        {
            if (PromptPriorityManager.Instance != null)
            {
                PromptPriorityManager.Instance.ReleasePrompt(gameObject);
            }
            else if (PossessionPromptUI.Instance != null)
            {
                PossessionPromptUI.Instance.HidePrompt();
            }
            showingPickupPrompt = false;
        }

        heldObject = objectToPickUp;

        SimpleGemsAnim gemsAnim = heldObject.GetComponent<SimpleGemsAnim>();
        if (gemsAnim != null)
            gemsAnim.enabled = false;

        heldObject.transform.parent = holdPoint;
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        heldObject.GetComponent<Collider>().enabled = false;

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

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;

        heldObject.GetComponent<Collider>().enabled = true;

        heldObject.transform.SetParent(null);

        Benjathemaker.SimpleGemsAnim gemsAnim = heldObject.GetComponent<Benjathemaker.SimpleGemsAnim>();
        if (gemsAnim != null)
        {
            gemsAnim.ResetPosition();
            gemsAnim.enabled = true;  
        }

        Debug.Log($"{gameObject.name} dropped {heldObject.name}");
        heldObject = null;
    }

    void OnDisable()
    {
        if (showingPickupPrompt)
        {
            if (PromptPriorityManager.Instance != null)
            {
                PromptPriorityManager.Instance.ReleasePrompt(gameObject);
            }
            else if (PossessionPromptUI.Instance != null)
            {
                PossessionPromptUI.Instance.HidePrompt();
            }
            showingPickupPrompt = false;
        }
    }

    public bool IsHoldingItem()
    {
        return heldObject != null;
    }

}
