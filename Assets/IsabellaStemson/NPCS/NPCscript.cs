using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCscript : CryptidUtils
{
    [HideInInspector] public Collider col;
    public CharacterSwitch characterSwitch;
    //Nadine will do the actual mini game coding for this
    public bool hasCompletedMinigame;
    public GameObject miniGameUI;
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
                    miniGameUI.SetActive(true);
                    miniGameUI.GetComponentInChildren<PlayerControl>().associatedNPC = this.gameObject;
                    level.SetActive(false);

                    //If you would rather you have to WIN, move this to the Player Control script
                    hasCompletedMinigame = true;
                    return;
                }
                else
                {
                    characterSwitch.SwitchToNPC(this.gameObject);
                }      
            }
        }
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
