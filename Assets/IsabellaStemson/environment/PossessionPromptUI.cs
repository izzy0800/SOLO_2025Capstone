using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PossessionPromptUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("Prompt Texts")]
    [SerializeField] private string possessText = "Press E to Possess";
    [SerializeField] private string puzzleText = "Press E to Solve Puzzle";
    [SerializeField] private string talkText = "Press T to Talk";

    private static PossessionPromptUI instance;
    public static PossessionPromptUI Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        // Hide prompt at start
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    public void ShowPrompt(bool hasPuzzle = false)
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(true);

            if (promptText != null)
            {
                // Show different text based on whether NPC has a puzzle
                promptText.text = hasPuzzle ? puzzleText : possessText;
            }
        }
    }

    public void ShowTalkPrompt()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(true);

            if (promptText != null)
            {
                promptText.text = talkText;
            }
        }
    }

    public void HidePrompt()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
    }
}
