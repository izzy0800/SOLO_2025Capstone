using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameController : MonoBehaviour
{
    public GameObject miniGameUI;

    public void OnWinButtonClick()
    {
        //will close mini game
        miniGameUI.SetActive(false);
        Time.timeScale = 1f;

        //switch control to NPC
        //FindObjectOfType<CharacterSwitch>().SwitchToNPC();
    }

    public void OnMiniGamerWin()
    {
        miniGameUI.SetActive(false);
        Time.timeScale = 1f;

        //FindObjectOfType<CharacterSwitch>().SwitchToNPC();
    }

    public void CloseMiniGame()
    {
        miniGameUI.SetActive(false);
        Time.timeScale = 1f;
    }

}
