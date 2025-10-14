using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class NPCDialogHandler : CryptidUtils
{
    [Header("Dialog Settings")]
    public string npcName = "NPC";
    [TextArea(3, 5)]
    public string[] dialogLines; // Simple array of dialog lines
    public bool canBeDialoguedWith = true;

    [Header("Dialog Detection")]
    [SerializeField] private float dialogRange = 2f;
    [SerializeField] private KeyCode dialogKey = KeyCode.F;
    [SerializeField] private GameObject dialogPrompt; // Optional UI prompt

    private CharacterSwitch characterSwitch;
    private NPCscript myNPCScript;
    private GameObject nearbyDialogableNPC;

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
            // Check if SimpleDialogManager exists and is not active
            if (DialogManager.Instance != null && !DialogManager.Instance.IsDialogActive)
            {
                InitiateDialog();
            }
            else if (DialogManager.Instance == null)
            {
                Debug.LogError("SimpleDialogManager not found! Add it to your Canvas!");
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
            // Check if it's an NPC and not ourselves
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

            if (dialogPrompt != null)
            {
                dialogPrompt.SetActive(nearbyDialogableNPC != null);

                // Position prompt above the target NPC
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

        // Get the name of the possessed NPC (who is speaking)
        string possessedNPCName = this.npcName;

        // Start the dialog with the target NPC's lines
        DialogManager.Instance.StartDialog(
            targetNPC.dialogLines,
            targetNPC.npcName, // The NPC you're talking TO speaks
            () => {
                Debug.Log($"Dialog between {possessedNPCName} and {targetNPC.npcName} completed!");
            }
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dialogRange);
    }
}