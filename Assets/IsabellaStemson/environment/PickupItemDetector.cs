using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItemDetector : MonoBehaviour
{
    [Header("Pickup Detection")]
    [SerializeField] private float pickupRange = 2f;
    [SerializeField] private LayerMask itemLayer = -1; 
    [SerializeField] private KeyCode pickupKey = KeyCode.E;

    private CharacterSwitch characterSwitch;
    private GameObject nearestItem;
    private bool showingPickupPrompt = false;

    void Start()
    {
        characterSwitch = FindObjectOfType<CharacterSwitch>();
    }

    void Update()
    {
        if (characterSwitch == null || !characterSwitch.IsPossessing) return;
        if (characterSwitch.npc != this.gameObject) return;

        CheckForNearbyItems();

        if (Input.GetKeyDown(pickupKey) && nearestItem != null)
        {
            PickupItem();
        }
    }

    private void CheckForNearbyItems()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange, itemLayer);

        GameObject closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Pickup") || col.GetComponent<PickUpSystem>() != null)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = col.gameObject;
                }
            }
        }

        if (nearestItem != closest)
        {
            nearestItem = closest;

            if (PossessionPromptUI.Instance != null)
            {
                if (nearestItem != null)
                {
                    string itemName = nearestItem.name;
                    PossessionPromptUI.Instance.ShowPickupPrompt(itemName);
                    showingPickupPrompt = true;
                    Debug.Log($"Showing pickup prompt for: {itemName}");
                }
                else if (showingPickupPrompt)
                {
                    // Hide prompt when no items nearby
                    PossessionPromptUI.Instance.HidePrompt();
                    showingPickupPrompt = false;
                    Debug.Log("Hiding pickup prompt");
                }
            }
        }
    }

    private void PickupItem()
    {
        if (nearestItem == null) return;

        Debug.Log($"Picking up: {nearestItem.name}");

        if (PossessionPromptUI.Instance != null)
        {
            PossessionPromptUI.Instance.HidePrompt();
            showingPickupPrompt = false;
        }

        PickUpSystem pickupSystem = GetComponent<PickUpSystem>();
        if (pickupSystem != null)
        {

        }

        Destroy(nearestItem);
        nearestItem = null;
    }

    void OnDisable()
    {
        if (showingPickupPrompt && PossessionPromptUI.Instance != null)
        {
            PossessionPromptUI.Instance.HidePrompt();
            showingPickupPrompt = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
