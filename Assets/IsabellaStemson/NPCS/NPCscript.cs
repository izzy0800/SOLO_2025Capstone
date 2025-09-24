using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCscript : CryptidUtils
{
    [HideInInspector] public Collider col;
    public CharacterSwitch characterSwitch;

    public int height;

    public bool hasCompletedMinigame;
    public GameObject miniGameUI; //updated this to be the slider puzzle
    public GameObject level;

    public SpriteRenderer visualSprite;

    public bool playerInRange;
    PlayerMovement pm;

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
                    //characterSwitch.npc = this.gameObject;
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

        MiniGameController m = miniGameUI.GetComponent<MiniGameController>();
        m.associatedNPC = this;

        if (level != null)
            level.SetActive(false);

        if(pm != null)
        {
            pm.enabled = false;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnPuzzleCompleted()
    {
        hasCompletedMinigame = true;

        miniGameUI.SetActive(false);

        if (level != null)
            level.SetActive(true);

        if (pm != null)
        {
            pm.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (characterSwitch != null)
        {
            characterSwitch.SwitchToNPC(this.gameObject);
        }
        else
        {
            Debug.LogError("CharacterSwitch not found! cannot switch camera to NPC.");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            pm = other.GetComponent<PlayerMovement>();
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
