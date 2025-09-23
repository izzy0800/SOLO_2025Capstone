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
        //Debug.Log(exitBlock != null ? "Exit block found" : "Exit block NOT found");
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

            //Debug.Log($"Ghost position: {ghostPos}, Exit position: {exitPos}");
            //Debug.Log($"Distance: {distance} (threshold: {checkDistance})");
        }

        if (distance < checkDistance)
        {
            //Debug.Log($"WIN TRIGGERED! Distance: {distance}");
            hasWon = true;

            MiniGameController controller = FindObjectOfType<MiniGameController>();
            if (controller != null)
            {
                //Debug.Log("Calling controller.OnMiniGamerWin()");
                controller.OnMiniGamerWin();
            }
            else
            {
                //Debug.LogError("MiniGameController not found");
            }
        }
    }
}
