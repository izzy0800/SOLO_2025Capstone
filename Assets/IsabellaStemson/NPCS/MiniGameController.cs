using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameController : MonoBehaviour
{
    public GameObject miniGameUI;
    public NPCscript associatedNPC;

    public GameObject puzzlePrefab;
    GameObject createdPuzzle;

    public void CreatePuzzle()
    {
        createdPuzzle = Instantiate(puzzlePrefab, this.transform);
    }

    public void DestroyPuzzle()
    {
        Destroy(createdPuzzle);
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

        DestroyPuzzle();
    }

    public void CloseMiniGame()
    {
        if (associatedNPC != null)
            associatedNPC.OnPuzzleCompleted();
    }

}
