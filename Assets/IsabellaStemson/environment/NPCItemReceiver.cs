using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCItemReceiver : MonoBehaviour
{
    [Header("Receivable Items")]
    [SerializeField] private List<ItemType> acceptableItems = new List<ItemType>();
    [SerializeField] private float receiveRange = 2f;

    [Header("Responses")]
    [SerializeField] private string[] acceptDialog; 
    [SerializeField] private string[] rejectDialog; 

    [Header("Events")]
    public UnityEngine.Events.UnityEvent<ItemType> onItemReceived;
    public UnityEngine.Events.UnityEvent<ItemType> onItemRejected;

    private CharacterSwitch characterSwitch;
    private NPCDialogHandler dialogHandler;
    private bool hasReceivedItem = false; 

    void Start()
    {
        characterSwitch = FindObjectOfType<CharacterSwitch>();
        dialogHandler = GetComponent<NPCDialogHandler>();
    }

    public bool CanReceiveItem(ItemType itemType)
    {
        return acceptableItems.Contains(itemType) && !hasReceivedItem;
    }

    public void ReceiveItem(GameObject item)
    {
        ItemData itemData = item.GetComponent<ItemData>();
        if (itemData == null) return;

        if (CanReceiveItem(itemData.itemType))
        {
            Debug.Log($"{gameObject.name} received {itemData.itemName}!");
            hasReceivedItem = true;

            if (acceptDialog != null && acceptDialog.Length > 0 && DialogManager.Instance != null)
            {
                DialogManager.Instance.StartDialog(acceptDialog, GetComponent<NPCDialogHandler>()?.npcName ?? "NPC", null);
            }

            onItemReceived?.Invoke(itemData.itemType);

            Destroy(item);
        }
        else
        {
            Debug.Log($"{gameObject.name} rejected {itemData.itemName}!");

            if (rejectDialog != null && rejectDialog.Length > 0 && DialogManager.Instance != null)
            {
                DialogManager.Instance.StartDialog(rejectDialog, GetComponent<NPCDialogHandler>()?.npcName ?? "NPC", null);
            }

            onItemRejected?.Invoke(itemData.itemType);
        }
    }

    public bool IsInRange(Transform giver)
    {
        return Vector3.Distance(transform.position, giver.position) <= receiveRange;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, receiveRange);
    }
}
