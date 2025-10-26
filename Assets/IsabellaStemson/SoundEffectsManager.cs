using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    [Header("Sound Effects")]
    [SerializeField] private AudioClip itemPickupSound;
    [SerializeField] private AudioClip itemDeliverySound;
    [SerializeField] private AudioClip puzzleCompleteSound;
    [SerializeField] private AudioClip dialogueOpenSound;
    [SerializeField] private AudioClip dialogueCloseSound;

    [Header("Settings")]
    [SerializeField] private float volume = 0.7f;

    private AudioSource audioSource;
    private static SoundEffectsManager instance;

    public static SoundEffectsManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
    }

    public void PlayItemPickup()
    {
        PlaySound(itemPickupSound);
    }

    public void PlayItemDelivery()
    {
        PlaySound(itemDeliverySound);
    }

    public void PlayPuzzleComplete()
    {
        PlaySound(puzzleCompleteSound);
    }

    public void PlayDialogueOpen()
    {
        PlaySound(dialogueOpenSound);
    }

    public void PlayDialogueClose()
    {
        PlaySound(dialogueCloseSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundEffectsManager: No sound clip assigned!");
            return;
        }

        audioSource.PlayOneShot(clip, volume);
        Debug.Log($"SoundEffectsManager: Playing {clip.name}");
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
}
