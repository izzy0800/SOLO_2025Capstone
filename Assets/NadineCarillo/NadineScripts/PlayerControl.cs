using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public CharacterSwitch characterSwitch;

    public Transform respawnPoint;
    public Transform winPoint;

    public GameObject miniGameUI;
    public GameObject jailScene;

    float distance = 10;
    public bool touchingCollider;
    bool dragging;

    public GameObject associatedNPC;

    private void Start()
    {
        if (characterSwitch == null)
            characterSwitch = FindObjectOfType<CharacterSwitch>();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    private void OnMouseDown()
    {
        Debug.Log("Drag");
        dragging = true;
    }

    private void OnMouseUp()
    {
        Debug.Log("Let Go");
        dragging = false;
    }

    [ContextMenu("Secret Teleport")]
    public void SecretTeleport()
    {
        transform.position = winPoint.position;
    }

    private void Update()
    {
        if (dragging)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = objPosition;
        }
    }

    /*
    private void OnMouseDrag()
    {
        if (!touchingCollider)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = objPosition;
        }
        
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Respawn")
        {
            //touchingCollider = true;
            dragging = false;
            StartCoroutine(ResetDelay());
        }

        if (other.tag == "Win")
        {
            jailScene.SetActive(true);
            characterSwitch.SwitchToNPC(associatedNPC);
            miniGameUI.SetActive(false);
        }
    }

    IEnumerator ResetDelay()
    {
        transform.position = respawnPoint.position;

        yield return new WaitForSeconds(1);

        touchingCollider = false;
    }

}
