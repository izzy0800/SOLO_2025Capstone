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
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.05f;

    private Queue<DialogLine> currentDialog;
    private bool isTyping = false;
    private string currentSentence;
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
        currentDialog = new Queue<DialogLine>();
    }

    private void Start()
    {
        characterSwitch = FindObjectOfType<CharacterSwitch>();

        if (continueButton != null)
            continueButton.onClick.AddListener(DisplayNextSentence);
    }

    public void StartDialog(Dialog dialog, string npcSpeakerName, System.Action onComplete = null)
    {
        // Only allowed dialog if possessing
        if (characterSwitch == null || !characterSwitch.IsPossessing)
        {
            Debug.LogWarning("Cannot start dialog - not possessing an NPC!");
            return;
        }

        if (dialogPanel.activeSelf) return;

        // Disabled player movement during dialog
        currentPlayerMovement = characterSwitch.npc?.GetComponent<PlayerMovement>();
        if (currentPlayerMovement != null)
            currentPlayerMovement.enabled = false;

        // Show cursor for dialog interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        onDialogComplete = onComplete;
        dialogPanel.SetActive(true);
        currentDialog.Clear();

        foreach (DialogLine line in dialog.lines)
        {
            // Replace speaker name if it's the possessed NPC speaking
            if (line.speakerName == "[POSSESSED_NPC]")
            {
                DialogLine modifiedLine = new DialogLine();
                modifiedLine.speakerName = npcSpeakerName;
                modifiedLine.text = line.text;
                modifiedLine.choices = line.choices;
                currentDialog.Enqueue(modifiedLine);
            }
            else
            {
                currentDialog.Enqueue(line);
            }
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isTyping)
        {
            CompleteTyping();
            return;
        }

        if (currentDialog.Count == 0)
        {
            EndDialog();
            return;
        }

        DialogLine line = currentDialog.Dequeue();
        speakerNameText.text = line.speakerName;

        if (line.choices != null && line.choices.Length > 0)
        {
            DisplayChoices(line.choices);
        }
        else
        {
            ClearChoices();
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeSentence(line.text));
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        currentSentence = sentence;
        dialogText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    private void CompleteTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogText.text = currentSentence;
        isTyping = false;
    }

    private void DisplayChoices(DialogChoice[] choices)
    {
        ClearChoices();
        continueButton.gameObject.SetActive(false);

        foreach (DialogChoice choice in choices)
        {
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choicesContainer.transform);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
            Button btn = choiceButton.GetComponent<Button>();

            btn.onClick.AddListener(() => {
                OnChoiceSelected(choice);
            });
        }
    }

    private void OnChoiceSelected(DialogChoice choice)
    {
        ClearChoices();
        continueButton.gameObject.SetActive(true);

        if (choice.response != null)
        {
            currentDialog.Clear();
            foreach (DialogLine line in choice.response.lines)
            {
                currentDialog.Enqueue(line);
            }
        }

        DisplayNextSentence();
    }

    private void ClearChoices()
    {
        foreach (Transform child in choicesContainer.transform)
        {
            Destroy(child.gameObject);
        }
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
