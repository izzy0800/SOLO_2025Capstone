using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleUIController : MonoBehaviour
{

    public GameObject puzzlePanel;
    private bool isOpen = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TogglePuzzle();
        }
    }

    public void TogglePuzzle()
    {
        isOpen = !isOpen;
        puzzlePanel.SetActive(isOpen);

        //pauses player movement
        Time.timeScale = isOpen ? 0f : 1f;

        // allows cursor to appear
        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

}
