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

    [Header("Player Control")]
    [SerializeField] private GameObject player;
    [SerializeField] private bool disablePlayerDuringCutscene = true;

    private bool cutscenePlayed = false;

    private void Start()
    {
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

        // Disable player controls during cutscene
        if (disablePlayerDuringCutscene && player != null)
        {
            SetPlayerControl(false);
        }

        timelineDirector.stopped += OnCutsceneFinished;

        timelineDirector.Play();
        cutscenePlayed = true;
    }

    private void OnCutsceneFinished(PlayableDirector director)
    {
        // Re-enable player controls
        if (disablePlayerDuringCutscene && player != null)
        {
            SetPlayerControl(true);
        }

        timelineDirector.stopped -= OnCutsceneFinished;
    }

    private void SetPlayerControl(bool enabled)
    {
        var playerController = player.GetComponent<MonoBehaviour>();
        if (playerController != null)
        {
            playerController.enabled = enabled;
        }

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = enabled;
        }
    }

    private void OnDestroy()
    {
        if (timelineDirector != null)
        {
            timelineDirector.stopped -= OnCutsceneFinished;
        }
    }
}
