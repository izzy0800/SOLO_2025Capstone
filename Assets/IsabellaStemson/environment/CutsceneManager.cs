using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [Header("Timeline Settings")]
    [SerializeField] private PlayableDirector timelineDirector;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private float delayBeforePlay = 0.5f;

    [Header("Camera Settings")]
    [SerializeField] private Camera cutsceneCamera;
    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private bool switchCameras = true;

    [Header("Player Control (3D Character)")]
    [SerializeField] private GameObject player;
    [SerializeField] private bool disablePlayerDuringCutscene = true;

    [Header("Dialogue Sync")]
    [SerializeField] private DialogueCutsceneManager dialogueManager;
    [SerializeField] private bool waitForDialogueBeforeEnding = true;
    [Tooltip("How long to wait after dialogue finishes before ending cutscene")]
    [SerializeField] private float delayAfterDialogue = 0.5f;

    [Header("Cutscene-Only Objects")]
    [Tooltip("Objects that should only be visible during the cutscene")]
    [SerializeField] private GameObject[] cutsceneOnlyObjects;

    private bool cutscenePlayed = false;
    private PlayerJitterFix playerJitterFix;
    private bool isWaitingForDialogue = false;

    private void Start()
    {
        // Get PlayerJitterFix component if it exists
        if (player != null)
        {
            playerJitterFix = player.GetComponent<PlayerJitterFix>();
            if (playerJitterFix != null)
            {
                Debug.Log("CutsceneManager: Found PlayerJitterFix component");
            }
        }

        // Hide cutscene-only objects initially
        if (cutsceneOnlyObjects != null)
        {
            foreach (var obj in cutsceneOnlyObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }

        // Setup initial camera states
        if (switchCameras)
        {
            if (cutsceneCamera != null)
            {
                cutsceneCamera.enabled = false;
            }
            if (gameplayCamera != null)
            {
                gameplayCamera.enabled = false;
            }
        }

        if (playOnStart && !cutscenePlayed)
        {
            Invoke(nameof(PlayCutscene), delayBeforePlay);
        }
    }

    public void PlayCutscene()
    {
        if (timelineDirector == null)
        {
            Debug.LogError("Timeline Director is not assigned!");
            return;
        }

        Debug.Log("CutsceneManager: Starting cutscene");

        // Show cutscene-only objects
        if (cutsceneOnlyObjects != null)
        {
            foreach (var obj in cutsceneOnlyObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    Debug.Log($"CutsceneManager: Showing cutscene object - {obj.name}");
                }
            }
        }

        // Register callback for when dialogue completes
        if (dialogueManager != null)
        {
            dialogueManager.SetDialogueCompleteCallback(OnDialogueComplete);
        }

        // Switch to cutscene camera
        if (switchCameras)
        {
            SwitchToCutsceneCamera();
        }

        // Disable player controls during cutscene
        if (disablePlayerDuringCutscene && player != null)
        {
            SetPlayerControl(false);
        }

        // Subscribe to timeline finished event
        timelineDirector.stopped += OnTimelineFinished;

        // Play the timeline
        timelineDirector.Play();
        cutscenePlayed = true;
    }

    private void OnDialogueComplete()
    {
        Debug.Log("CutsceneManager: Dialogue completed - ending cutscene early");

        // Stop the timeline and end cutscene
        if (timelineDirector != null && timelineDirector.state == PlayState.Playing)
        {
            timelineDirector.Stop();
            // Note: This will trigger OnTimelineFinished automatically
        }
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        Debug.Log("CutsceneManager: Timeline finished");

        // Check if we should wait for dialogue
        if (waitForDialogueBeforeEnding && dialogueManager != null)
        {
            if (!dialogueManager.IsDialogueComplete())
            {
                Debug.Log("CutsceneManager: Waiting for dialogue to finish...");
                isWaitingForDialogue = true;
                StartCoroutine(WaitForDialogue());
                return;
            }
        }

        // If not waiting for dialogue, end immediately
        EndCutscene();
    }

    private IEnumerator WaitForDialogue()
    {
        // Wait until dialogue is complete
        while (!dialogueManager.IsDialogueComplete())
        {
            yield return null;
        }

        Debug.Log("CutsceneManager: Dialogue finished, ending cutscene");

        // Optional delay after dialogue finishes
        if (delayAfterDialogue > 0)
        {
            yield return new WaitForSeconds(delayAfterDialogue);
        }

        isWaitingForDialogue = false;
        EndCutscene();
    }

    private void EndCutscene()
    {
        Debug.Log("CutsceneManager: Ending cutscene");

        // Hide cutscene-only objects
        if (cutsceneOnlyObjects != null)
        {
            foreach (var obj in cutsceneOnlyObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    Debug.Log($"CutsceneManager: Hiding cutscene object - {obj.name}");
                }
            }
        }

        // Switch back to gameplay camera
        if (switchCameras)
        {
            SwitchToGameplayCamera();
        }

        // Re-enable player controls
        if (disablePlayerDuringCutscene && player != null)
        {
            SetPlayerControl(true);
        }

        // Hide dialogue UI
        if (dialogueManager != null)
        {
            dialogueManager.ForceEndDialogue();
        }

        // Unsubscribe from event
        if (timelineDirector != null)
        {
            timelineDirector.stopped -= OnTimelineFinished;
        }
    }

    private void SwitchToCutsceneCamera()
    {
        if (cutsceneCamera != null)
        {
            cutsceneCamera.enabled = true;
            cutsceneCamera.tag = "MainCamera";
            Debug.Log("Switched to cutscene camera");
        }

        if (gameplayCamera != null)
        {
            gameplayCamera.enabled = false;
        }
    }

    private void SwitchToGameplayCamera()
    {
        if (gameplayCamera != null)
        {
            gameplayCamera.enabled = true;
            gameplayCamera.tag = "MainCamera";
            Debug.Log("Switched to gameplay camera");
        }

        if (cutsceneCamera != null)
        {
            cutsceneCamera.enabled = false;
        }
    }

    private void SetPlayerControl(bool enabled)
    {
        if (player == null) return;

        Debug.Log($"CutsceneManager: Setting player control to {enabled}");

        if (playerJitterFix != null)
        {
            if (enabled)
            {
                playerJitterFix.UnfreezeAfterCutscene();
            }
            else
            {
                playerJitterFix.FreezeForCutscene();
            }
        }

        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this &&
                !(script is PlayerJitterFix) &&
                !(script is Animator))
            {
                script.enabled = enabled;
            }
        }

        if (playerJitterFix == null)
        {
            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (!enabled)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }
                else
                {
                    rb.isKinematic = false;
                }
            }

            // Handle CharacterController
            var cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = enabled;
            }
        }
    }

    private void OnDestroy()
    {
        if (timelineDirector != null)
        {
            timelineDirector.stopped -= OnTimelineFinished;
        }
    }

    public void ForceEndCutscene()
    {
        if (isWaitingForDialogue)
        {
            StopAllCoroutines();
            isWaitingForDialogue = false;
        }
        EndCutscene();
    }
}