using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameController : MonoBehaviour
{
    public GameObject miniGameUI;
    private NPCscript associatedNPC;

    private void Start()
    {
        NPCscript[] npcs = FindObjectsOfType<NPCscript>();
        foreach(NPCscript npc in npcs)
        {
            if(npc.miniGameUI == miniGameUI)
            {
                associatedNPC = npc;
                break;
            }
        }
    }

    public void OnWinButtonClick()
    {
        if (associatedNPC != null)
            associatedNPC.OnPuzzleCompleted();
    }

    public void OnMiniGamerWin()
    {
        if (associatedNPC != null)
            associatedNPC.OnPuzzleCompleted();
    }

    public void CloseMiniGame()
    {
        if (associatedNPC != null)
            associatedNPC.OnPuzzleCompleted();
    }

}
