using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class CharacterSwitch : CryptidUtils
{
    public GameObject player;
    public GameObject npc;

    ParticleSystem playerParticles;

    public ThirdPersonCamera thirdPerson;
    public FirstPersonCamera firstPerson;

    private bool isControllingNPC = false;
    public bool IsPossessing => isControllingNPC;

    PlayerMovement playerMovement;

    private enum TargetType
    {
        player,
        npc
    }
   
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerParticles = player.GetComponentInChildren<ParticleSystem>();
        SwitchToPlayer();
    }

    public void SwitchToNPC(GameObject newNPC)
    {
        npc = newNPC;
        Switch(TargetType.npc);
    }

    public void SwitchToPlayer()
    {
        Switch(TargetType.player);
    }

    private void Switch(TargetType type)
    {
        bool isNPC = type == TargetType.npc;
        isControllingNPC = isNPC;
        
        EnableControl(player, !isNPC);
        if (npc != null)
            EnableControl(npc, isNPC);

        thirdPerson.enabled = !isNPC;
        firstPerson.enabled = isNPC;

        thirdPerson.target = player;
        firstPerson.target = npc;

        playerMovement.allowVerticalMovement = !isNPC;
        playerParticles.gameObject.SetActive(!isNPC);

        if (npc != null)
        {
            var npcMovement = npc.GetComponent<PlayerMovement>();
            if (npcMovement != null)
            {
                npcMovement.allowVerticalMovement = false; 
            }

            //to toggle sprite visblility
            var npcScript = npc.GetComponent<NPCscript>();
            if (npcScript != null)
            {
                npcScript.SetSpriteVisible(!isNPC);
            }

        }

    }

    // Update is called once per frame
    private void Update()
    {
        //Pressing 'Tab' will toggle back to the player
        if (isControllingNPC && Input.GetKeyDown(KeyCode.Tab))
        {
            Switch(TargetType.player);
        }
    }

    private void EnableControl(GameObject character, bool isEnabled)
    {
        if (character.GetComponent<PlayerMovement>())
            character.GetComponent<PlayerMovement>().enabled = isEnabled;

        if (character.GetComponent<NPCscript>())
        {
            //character.GetComponent<NPCscript>().enabled = isEnabled;
            character.GetComponent<NPCscript>().col.isTrigger = !isEnabled;
        }

        if (character.GetComponent<PickUpSystem>())
            character.GetComponent<PickUpSystem>().enabled = isEnabled;

    }
}
