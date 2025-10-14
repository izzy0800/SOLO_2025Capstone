using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [Header("Dialog UI")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogText;

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.05f;

    private string[] currentLines;
    private string currentSpeakerName;
    private int currentLineIndex;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private System.Action onDialogComplete;

    private CharacterSwitch characterSwitch;
    private PlayerMovement currentPlayerMovement;

    private static DialogManager instance;
    public static DialogManager Instance => instance;

    public bool IsDialogActive => dialogPanel != null && dialogPanel.activeSelf;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    private void Start()
    {
        characterSwitch = FindObjectOfType<CharacterSwitch>();
    }

    private void Update()
    {
        if (!IsDialogActive) return;

        // Click or press Space to continue dialog
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Complete current line immediately
                CompleteTyping();
            }
            else
            {
                // Move to next line
                NextLine();
            }
        }
    }

    public void StartDialog(string[] lines, string speakerName, System.Action onComplete = null)
    {
        // Only allow dialog if possessing
        if (characterSwitch == null || !characterSwitch.IsPossessing)
        {
            Debug.LogWarning("Cannot start dialog - not possessing an NPC!");
            return;
        }

        if (dialogPanel.activeSelf) return;

        // Disable player movement during dialog
        currentPlayerMovement = characterSwitch.npc?.GetComponent<PlayerMovement>();
        if (currentPlayerMovement != null)
            currentPlayerMovement.enabled = false;

        // Show cursor for dialog interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        onDialogComplete = onComplete;
        currentLines = lines;
        currentSpeakerName = speakerName;
        currentLineIndex = 0;

        dialogPanel.SetActive(true);

        if (speakerNameText != null)
            speakerNameText.text = speakerName;

        DisplayLine();
    }

    private void DisplayLine()
    {
        if (currentLineIndex >= currentLines.Length)
        {
            EndDialog();
            return;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentLineIndex]));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char c in line.ToCharArray())
        {
            dialogText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogText.text = currentLines[currentLineIndex];
            isTyping = false;
        }
    }

    private void NextLine()
    {
        currentLineIndex++;
        DisplayLine();
    }

    private void EndDialog()
    {
        dialogPanel.SetActive(false);

        // Re-enable player movement
        if (currentPlayerMovement != null)
            currentPlayerMovement.enabled = true;

        // Hide cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        onDialogComplete?.Invoke();
        onDialogComplete = null;
    }
}