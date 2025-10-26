using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private GameObject exitBlock;
    private float checkDistance = 70f;
    private bool hasWon = false;
    private int updateCount = 0;

    private void Start()
    {
        exitBlock = GameObject.Find("Exit");
    }

    private void Update()
    {
        updateCount++;
        if (exitBlock == null || hasWon) return;

        float distance = Vector2.Distance(transform.position, exitBlock.transform.position);

        if (updateCount % 30 == 0)
        {
            Vector2 ghostPos = transform.position;
            Vector2 exitPos = exitBlock.transform.position;
        }

        if (distance < checkDistance)
        {
            hasWon = true;

            if (SoundEffectsManager.Instance != null)
            {
                SoundEffectsManager.Instance.PlayPuzzleComplete();
            }

            MiniGameController controller = FindObjectOfType<MiniGameController>();
            if (controller != null)
            {
                controller.OnMiniGamerWin();
            }
            else
            {
               
            }
        }
    }
}
