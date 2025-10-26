using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndingManager : MonoBehaviour
{
    [Header("Required Items")]
    [SerializeField] private List<ItemRequirement> requiredDeliveries = new List<ItemRequirement>();

    [Header("Ending Cutscene")]
    [SerializeField] private GameObject endingCutscenePanel;
    [SerializeField] private TextMeshProUGUI endingText;
    [SerializeField] private float delayAfterLastDialogue = 3f; 
    [SerializeField] private float cutsceneDuration = 5f;
    [SerializeField] private string MainMenu = "MainMenu";

    [Header("Optional Effects")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;

    private static GameEndingManager instance;
    public static GameEndingManager Instance => instance;

    private HashSet<string> completedDeliveries = new HashSet<string>();

    [System.Serializable]
    public class ItemRequirement
    {
        public string npcName;
        public ItemType requiredItem;
        public bool completed = false;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Hide ending panel at start
        if (endingCutscenePanel != null)
            endingCutscenePanel.SetActive(false);

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(false);
            Color c = fadePanel.color;
            c.a = 0;
            fadePanel.color = c;
        }
    }

    void Start()
    {
        // Subscribe to all NPCItemReceivers
        NPCItemReceiver[] receivers = FindObjectsOfType<NPCItemReceiver>();
        foreach (NPCItemReceiver receiver in receivers)
        {
            NPCDialogHandler dialogHandler = receiver.GetComponent<NPCDialogHandler>();
            if (dialogHandler != null)
            {
                string npcName = dialogHandler.npcName;

                // Subscribe to the item received event
                receiver.onItemReceived.AddListener((ItemType itemType) => {
                    OnItemDelivered(npcName, itemType);
                });
            }
        }
    }

    public void OnItemDelivered(string npcName, ItemType itemType)
    {
        Debug.Log($"{npcName} received {itemType}");

        // Check if this was a required delivery
        foreach (ItemRequirement req in requiredDeliveries)
        {
            if (req.npcName == npcName && req.requiredItem == itemType && !req.completed)
            {
                req.completed = true;
                completedDeliveries.Add($"{npcName}_{itemType}");
                Debug.Log($"Required delivery completed: {npcName} - {itemType}");

                // Check if all deliveries are complete
                CheckForGameCompletion();
                break;
            }
        }
    }

    private void CheckForGameCompletion()
    {
        bool allComplete = true;
        foreach (ItemRequirement req in requiredDeliveries)
        {
            if (!req.completed)
            {
                allComplete = false;
                break;
            }
        }

        if (allComplete)
        {
            Debug.Log("All items delivered! Triggering ending...");
            TriggerEnding();
        }
        else
        {
            int completed = completedDeliveries.Count;
            int total = requiredDeliveries.Count;
            Debug.Log($"Progress: {completed}/{total} deliveries complete");
        }
    }

    private void TriggerEnding()
    {
        // Start the ending sequence
        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        Debug.Log($"Waiting {delayAfterLastDialogue} seconds for dialogue to complete...");
        yield return new WaitForSeconds(delayAfterLastDialogue);

        // Disable player controls
        DisableAllControls();

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayEndingMusic();
        }

        // Fade in
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            yield return StartCoroutine(FadeIn());
        }

        // Show ending cutscene
        if (endingCutscenePanel != null)
        {
            endingCutscenePanel.SetActive(true);

            if (endingText != null)
            {
                endingText.text = "Congratulations!\n\nYou have successfully delivered all items.\n\nThis is the end...\n\n\n\nfor now...";
            }
        }

        // Wait for cutscene duration
        yield return new WaitForSeconds(cutsceneDuration);

        // Fade out
        if (fadePanel != null)
        {
            yield return StartCoroutine(FadeOut());
        }

        // Load main menu
        LoadMainMenu();
    }

    private void DisableAllControls()
    {
        // Disable character switching
        CharacterSwitch charSwitch = FindObjectOfType<CharacterSwitch>();
        if (charSwitch != null)
        {
            charSwitch.enabled = false;
        }

        // Disable all movement
        PlayerMovement[] playerMovements = FindObjectsOfType<PlayerMovement>();
        foreach (var pm in playerMovements)
        {
            pm.enabled = false;
        }

        Move[] moves = FindObjectsOfType<Move>();
        foreach (var m in moves)
        {
            m.canMove = false;
        }

        // Disable camera controls
        CameraFollower cameraFollower = FindObjectOfType<CameraFollower>();
        if (cameraFollower != null)
        {
            cameraFollower.canLook = false;
        }

        // Hide cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0;
        Color c = fadePanel.color;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, elapsedTime / fadeInDuration);
            fadePanel.color = c;
            yield return null;
        }

        c.a = 1;
        fadePanel.color = c;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        Color c = fadePanel.color;
        c.a = 1;
        fadePanel.color = c;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Lerp(1, 1, elapsedTime / fadeOutDuration); 
            fadePanel.color = c;
            yield return null;
        }
    }

    private void LoadMainMenu()
    {
        
        if (!string.IsNullOrEmpty(MainMenu))
        {
            SceneManager.LoadScene(MainMenu);
        }
        else
        {
            
            SceneManager.LoadScene(0);
        }
    }
}
