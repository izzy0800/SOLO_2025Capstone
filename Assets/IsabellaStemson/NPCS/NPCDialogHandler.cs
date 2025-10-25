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
    private PickUpSystem myPickupSystem;

    void Start()
    {
        characterSwitch = FindObjectOfType<CharacterSwitch>();
        myNPCScript = GetComponent<NPCscript>();
        myPickupSystem = GetComponent<PickUpSystem>(); 

        if (dialogPrompt != null)
            dialogPrompt.SetActive(false);
    }

    void Update()
    {
        if (characterSwitch == null || !characterSwitch.IsPossessing) return;
        if (characterSwitch.npc != this.gameObject) return;

        bool isHoldingItem = myPickupSystem != null && myPickupSystem.IsHoldingItem();

        
        if (isHoldingItem)
        {
            if (showingTalkPrompt)
            {
                if (PossessionPromptUI.Instance != null)
                {
                    PossessionPromptUI.Instance.HidePrompt();
                }
                showingTalkPrompt = false;
                nearbyDialogableNPC = null;
            }
            return; 
        }

        CheckForNearbyNPCs();

        if (Input.GetKeyDown(dialogKey) && nearbyDialogableNPC != null)
        {
            if (PossessionPromptUI.Instance != null)
            {
                PossessionPromptUI.Instance.HidePrompt();
                showingTalkPrompt = false;
            }

            if (DialogManager.Instance != null && !DialogManager.Instance.IsDialogActive)
            {
                InitiateDialog();
            }
        }
    }

    private void CheckForNearbyNPCs()
    {
        
        if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive)
        {
            return;
        }

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

        if (nearbyDialogableNPC != closestNPC)
        {
            nearbyDialogableNPC = closestNPC;

            if (PossessionPromptUI.Instance != null)
            {
                if (nearbyDialogableNPC != null)
                {
                    PossessionPromptUI.Instance.ShowTalkPrompt();
                    showingTalkPrompt = true;
                }
                else if (showingTalkPrompt)
                {
                    PossessionPromptUI.Instance.HidePrompt();
                    showingTalkPrompt = false;
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