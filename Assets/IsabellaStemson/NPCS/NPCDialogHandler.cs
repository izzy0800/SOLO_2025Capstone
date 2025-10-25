using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogHandler : CryptidUtils
{
    [Header("Dialog Settings")]
    public string npcName = "NPC";
    [TextArea(3, 5)]
    public string[] dialogLines; 
    public bool canBeDialoguedWith = true;

    [Header("Dialog Detection")]
    [SerializeField] private float dialogRange = 2f;
    [SerializeField] private KeyCode dialogKey = KeyCode.T;
    [SerializeField] private GameObject dialogPrompt; 

    private CharacterSwitch characterSwitch;
    private NPCscript myNPCScript;
    private GameObject nearbyDialogableNPC;
    private bool showingTalkPrompt = false;

    void Start()
    {
        characterSwitch = FindObjectOfType<CharacterSwitch>();
        myNPCScript = GetComponent<NPCscript>();

        if (dialogPrompt != null)
            dialogPrompt.SetActive(false);
    }

    void Update()
    {
        // Only check for dialog if this NPC is being possessed
        if (characterSwitch == null || !characterSwitch.IsPossessing) return;
        if (characterSwitch.npc != this.gameObject) return;

        // Check for nearby NPCs to talk to
        CheckForNearbyNPCs();

        // Handle dialog input
        if (Input.GetKeyDown(dialogKey) && nearbyDialogableNPC != null)
        {
            // Hide the talk prompt when starting dialog
            if (PossessionPromptUI.Instance != null)
            {
                PossessionPromptUI.Instance.HidePrompt();
                showingTalkPrompt = false;
            }

            // Check if DialogManager exists and is not active
            if (DialogManager.Instance != null && !DialogManager.Instance.IsDialogActive)
            {
                InitiateDialog();
            }
            else if (DialogManager.Instance == null)
            {
                Debug.LogError("DialogManager not found! Add it to your Canvas!");
            }
        }
    }

    private void CheckForNearbyNPCs()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, dialogRange);

        GameObject closestNPC = null;
        float closestDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            NPCDialogHandler otherNPC = col.GetComponent<NPCDialogHandler>();
            if (otherNPC != null && otherNPC != this && otherNPC.canBeDialoguedWith)
            {
                float distance = Vector3.Distance(transform.position, otherNPC.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNPC = otherNPC.gameObject;
                }
            }
        }

        // Update the nearby NPC and show/hide prompt
        if (nearbyDialogableNPC != closestNPC)
        {
            nearbyDialogableNPC = closestNPC;

            // Use the main UI prompt system
            if (PossessionPromptUI.Instance != null)
            {
                if (nearbyDialogableNPC != null)
                {
                    // Show "Press T to Talk" prompt
                    PossessionPromptUI.Instance.ShowTalkPrompt();
                    showingTalkPrompt = true;
                }
                else if (showingTalkPrompt)
                {
                    // Hide prompt when no NPCs nearby
                    PossessionPromptUI.Instance.HidePrompt();
                    showingTalkPrompt = false;
                }
            }

            if (dialogPrompt != null)
            {
                dialogPrompt.SetActive(nearbyDialogableNPC != null);

                if (nearbyDialogableNPC != null)
                {
                    Vector3 promptPos = nearbyDialogableNPC.transform.position + Vector3.up * 2f;
                    dialogPrompt.transform.position = promptPos;
                }
            }
        }
    }

    private void InitiateDialog()
    {
        if (nearbyDialogableNPC == null) return;

        NPCDialogHandler targetNPC = nearbyDialogableNPC.GetComponent<NPCDialogHandler>();
        if (targetNPC == null || targetNPC.dialogLines == null || targetNPC.dialogLines.Length == 0) return;

        string possessedNPCName = this.npcName;

        DialogManager.Instance.StartDialog(
            targetNPC.dialogLines,
            targetNPC.npcName, 
            () => {
                Debug.Log($"Dialog between {possessedNPCName} and {targetNPC.npcName} completed!");

                if (nearbyDialogableNPC != null && PossessionPromptUI.Instance != null)
                {
                    PossessionPromptUI.Instance.ShowTalkPrompt();
                    showingTalkPrompt = true;
                }
            }
        );
    }

    void OnDisable()
    {
        if (showingTalkPrompt && PossessionPromptUI.Instance != null)
        {
            PossessionPromptUI.Instance.HidePrompt();
            showingTalkPrompt = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dialogRange);
    }
}