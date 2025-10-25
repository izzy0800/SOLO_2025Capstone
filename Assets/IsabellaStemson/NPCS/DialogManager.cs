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

    // Store references to disable during dialog
    private Move currentNPCMove;
    private CameraFollower cameraFollower;
    private FirstPersonCamera firstPersonCamera;

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

        // Clear any default text
        if (dialogText != null)
            dialogText.text = "";

        if (speakerNameText != null)
            speakerNameText.text = "";
    }

    private void Start()
    {
        characterSwitch = FindObjectOfType<CharacterSwitch>();
        cameraFollower = FindObjectOfType<CameraFollower>();
        firstPersonCamera = FindObjectOfType<FirstPersonCamera>();
    }

    private void Update()
    {
        if (!IsDialogActive) return;

        // Block Tab key during dialog
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Cannot switch to player during dialog!");
            return; // Consume the Tab input
        }

        // Click or press Space to continue dialog
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                CompleteTyping();
            }
            else
            {
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

        // Disable ALL movement and camera controls
        DisableAllControls();

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

    private void DisableAllControls()
    {
        // Disable NPC movement
        if (characterSwitch != null && characterSwitch.npc != null)
        {
            currentNPCMove = characterSwitch.npc.GetComponent<Move>();
            if (currentNPCMove != null)
            {
                currentNPCMove.canMove = false;
            }

            // Stop any existing velocity
            Rigidbody rb = characterSwitch.npc.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Disable camera looking
        if (cameraFollower != null)
        {
            cameraFollower.canLook = false;
        }

        if (firstPersonCamera != null)
        {
            firstPersonCamera.enabled = false;
        }
    }

    private void EnableAllControls()
    {
        // Re-enable NPC movement
        if (currentNPCMove != null)
        {
            currentNPCMove.canMove = true;
        }

        // Re-enable camera looking
        if (cameraFollower != null)
        {
            cameraFollower.canLook = true;
        }

        if (firstPersonCamera != null)
        {
            firstPersonCamera.enabled = true;
        }
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
        // Clear text before hiding
        if (dialogText != null)
            dialogText.text = "";

        if (speakerNameText != null)
            speakerNameText.text = "";

        dialogPanel.SetActive(false);

        // Re-enable all controls
        EnableAllControls();

        // Hide cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        onDialogComplete?.Invoke();
        onDialogComplete = null;
    }
}