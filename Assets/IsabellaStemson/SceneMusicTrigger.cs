using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    public enum MusicType
    {
        Menu,
        Gameplay,
        Ending
    }

    [Header("Scene Music")]
    [SerializeField] private MusicType musicToPlay = MusicType.Gameplay;
    [SerializeField] private bool playOnStart = true;

    private void Start()
    {
        if (playOnStart)
        {
            PlaySceneMusic();
        }
    }

    public void PlaySceneMusic()
    {
        if (MusicManager.Instance == null)
        {
            Debug.LogWarning("SceneMusicTrigger: No MusicManager found in scene!");
            return;
        }

        switch (musicToPlay)
        {
            case MusicType.Menu:
                MusicManager.Instance.PlayMenuMusic();
                break;
            case MusicType.Gameplay:
                MusicManager.Instance.PlayGameplayMusic();
                break;
            case MusicType.Ending:
                MusicManager.Instance.PlayEndingMusic();
                break;
        }
    }
}
