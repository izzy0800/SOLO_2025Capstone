using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    public string dialogueText;
    [Range(0.01f, 0.1f)]
    public float typingSpeed = 0.05f;
    public float pauseAfterLine = 1.5f;
}

public class DialogueCutsceneManager : MonoBehaviour
{
    
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue Content")]
    [SerializeField] private DialogueLine[] dialogueLines;

    [Header("Settings")]
    [SerializeField] private bool skipToEndOnClick = true;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isDialogueActive = false;
    private Coroutine typingCoroutine;

    private void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDialogueActive && skipToEndOnClick && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                SkipToEndOfLine();
            }
            else if (currentLineIndex < dialogueLines.Length)
            {
                DisplayNextLine();
            }
        }
    }

    public void StartDialogue()
    {
        if (dialogueLines.Length == 0)
        {
            Debug.LogWarning("No dialogue lines assigned!");
            return;
        }

        currentLineIndex = 0;
        isDialogueActive = true;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (currentLineIndex >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueLines[currentLineIndex];

        if (characterNameText != null)
        {
            characterNameText.text = line.characterName;
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(line));
        currentLineIndex++;
    }

    private IEnumerator TypeText(DialogueLine line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.dialogueText)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(line.typingSpeed);
        }

        isTyping = false;

        yield return new WaitForSeconds(line.pauseAfterLine);
        DisplayNextLine();
    }

    private void SkipToEndOfLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (currentLineIndex > 0 && currentLineIndex <= dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLineIndex - 1].dialogueText;
        }

        isTyping = false;
    }

    private void EndDialogue()
    {
        isDialogueActive = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public void ShowDialogueLine(int lineIndex)
    {
        if (lineIndex >= 0 && lineIndex < dialogueLines.Length)
        {
            currentLineIndex = lineIndex;
            DisplayNextLine();
        }
    }
}
