using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCscript : CryptidUtils
{
    [HideInInspector] public Collider col;
    public CharacterSwitch characterSwitch;

    public bool hasCompletedMinigame;
    public GameObject miniGameUI; //updated this to be the slider puzzle
    public GameObject level;

    public SpriteRenderer visualSprite;

    public bool playerInRange;

    private void Start()
    {
        col = GetComponent<Collider>();

        if (characterSwitch == null)
            characterSwitch = FindObjectOfType<CharacterSwitch>();

        if (visualSprite == null)
            visualSprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!hasCompletedMinigame)
                {
                    OpenSliderPuzzle();
                    return;
                }
                else
                {
                    characterSwitch.SwitchToNPC(this.gameObject);
                }      
            }
        }
    }

    private void OpenSliderPuzzle()
    {
        miniGameUI.SetActive(true);

        if (level != null)
            level.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            var playerMovement = player.GetComponent<MonoBehaviour>();
            if (playerMovement != null)
                playerMovement.enabled = false;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        hasCompletedMinigame = true;
    }

    public void OnPuzzleCompleted()
    {
        miniGameUI.SetActive(false);

        if (level != null)
            level.SetActive(true);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var playerMovement = player.GetComponent<MonoBehaviour>();
            if (playerMovement != null)
                playerMovement.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            //Show a prompt
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            //Show a prompt
        }
    }


  //toggling npc sprite RAHH RAHHH RAHHH
    public void SetSpriteVisible(bool visible)
    {
        if (visualSprite != null)
            visualSprite.enabled = visible;
    }

}
