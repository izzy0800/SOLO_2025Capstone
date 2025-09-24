using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameController : MonoBehaviour
{
    public GameObject miniGameUI;
    public NPCscript associatedNPC;

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
