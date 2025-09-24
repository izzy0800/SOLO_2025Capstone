using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            MiniGameController controller = FindObjectOfType<MiniGameController>();
            if (controller != null)
            {
                //Debug.Log("Calling controller.OnMiniGamerWin()");
                controller.OnMiniGamerWin();
            }
        }
    }
}
