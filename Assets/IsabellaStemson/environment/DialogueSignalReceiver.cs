using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueSignalReceiver : MonoBehaviour, INotificationReceiver
{
    [SerializeField] private DialogueCutsceneManager dialogueManager;

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is DialogueSignal dialogueSignal)
        {
            if (dialogueManager != null)
            {
                if (dialogueSignal.lineIndex >= 0)
                {
                    dialogueManager.ShowDialogueLine(dialogueSignal.lineIndex);
                }
                else
                {
                    dialogueManager.StartDialogue();
                }
            }
        }
    }
}

// Create this as a Signal Asset in your project
// Right-click in Project > Create > Playables > Signal
[System.Serializable]
public class DialogueSignal : SignalAsset
{
    public int lineIndex = -1; // -1 means start from beginning
}
