using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptPriorityManager : MonoBehaviour
{
    public enum PromptType
    {
        None = 0,
        Puzzle = 1,    
        Possess = 2,
        Talk = 3,
        Pickup = 4     
    }

    private static PromptPriorityManager instance;
    public static PromptPriorityManager Instance => instance;

    private PromptType currentPrompt = PromptType.None;
    private GameObject currentPromptSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public bool RequestPrompt(PromptType type, GameObject source)
    {
        if (currentPrompt == PromptType.None || (int)type <= (int)currentPrompt)
        {
            currentPrompt = type;
            currentPromptSource = source;
            return true;
        }
        return false;
    }

    public void ReleasePrompt(GameObject source)
    {
        if (currentPromptSource == source)
        {
            currentPrompt = PromptType.None;
            currentPromptSource = null;

            if (PossessionPromptUI.Instance != null)
            {
                PossessionPromptUI.Instance.HidePrompt();
            }
        }
    }

    public bool HasPriority(GameObject source)
    {
        return currentPromptSource == null || currentPromptSource == source;
    }
}
