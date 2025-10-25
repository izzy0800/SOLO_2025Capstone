using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    [TextArea(3, 5)]
    public string dialogueText;
    [Tooltip("How long to wait after typing finishes before auto-advancing (0 = wait for click)")]
    public float autoAdvanceDelay = 2f;
}

public class DialogueCutsceneManager : MonoBehaviour
{
    [Header("Dialog UI")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogText;

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.05f;

    [Header("Dialogue Content")]
    [SerializeField] private DialogueLine[] dialogueLines;

    private int currentLineIndex;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private System.Action onDialogueComplete;

    public bool IsDialogActive => dialogPanel != null && dialogPanel.activeSelf;

    // Check if all dialogue has been shown
    public bool IsDialogueComplete()
    {
        return !IsDialogActive || currentLineIndex >= dialogueLines.Length;
    }

    private void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
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

    // Called by Timeline Signal
    public void StartDialogue()
    {
        if (dialogueLines.Length == 0)
        {
            Debug.LogWarning("No dialogue lines assigned!");
            return;
        }

        if (dialogPanel.activeSelf) return;

        currentLineIndex = 0;
        dialogPanel.SetActive(true);

        DisplayLine();
    }

    // Register a callback for when dialogue completes
    public void SetDialogueCompleteCallback(System.Action callback)
    {
        onDialogueComplete = callback;
    }

    private void DisplayLine()
    {
        if (currentLineIndex >= dialogueLines.Length)
        {
            EndDialog();
            return;
        }

        DialogueLine currentLine = dialogueLines[currentLineIndex];

        if (speakerNameText != null)
            speakerNameText.text = currentLine.characterName;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(currentLine));
    }

    private IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char c in line.dialogueText.ToCharArray())
        {
            dialogText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;

        // Auto-advance after delay if set
        if (line.autoAdvanceDelay > 0)
        {
            yield return new WaitForSeconds(line.autoAdvanceDelay);
            NextLine();
        }
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogText.text = dialogueLines[currentLineIndex].dialogueText;
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
        Debug.Log("Cutscene dialogue ended");

        // Notify that dialogue is complete
        onDialogueComplete?.Invoke();
    }

    // Called by Timeline signals if needed
    public void ShowDialogueLine(int lineIndex)
    {
        if (lineIndex >= 0 && lineIndex < dialogueLines.Length)
        {
            currentLineIndex = lineIndex;

            if (!IsDialogActive)
            {
                if (dialogPanel != null)
                    dialogPanel.SetActive(true);
            }

            DisplayLine();
        }
    }

    // Called by CutsceneManager when cutscene ends
    public void ForceEndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;

        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }

        if (dialogText != null)
        {
            dialogText.text = "";
        }

        if (speakerNameText != null)
        {
            speakerNameText.text = "";
        }

        Debug.Log("Cutscene dialogue forcefully ended");
    }
}
