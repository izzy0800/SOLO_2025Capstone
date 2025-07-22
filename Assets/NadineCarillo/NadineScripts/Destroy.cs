using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Destroy : MonoBehaviour
{
    //private PlayerControl playerControl;
    //public Vector3 startPos;
    //[SerializeField] Transform respawnPoint;

    // Start is called before the first frame update

    private void Start()
    {
       // GameObject player = GameObject.FindWithTag("Player");
        //playerControl = player.GetComponent<PlayerControl>();
        //Debug.Log("Playerfound");

        //startPos = transform.position;
     
    }

    /*void OnTriggerEnter(Collider col)
    {
      if(col.tag == "Player")
        {
            col.transform.position = respawnPoint;
        }
        //Destroy(other.gameObject);
        //Application.LoadLevel("2") this isn't necessary because we jus need to possess player, it shouldn't load a different scene 
        

    }
    */

    /*void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Respawn")
        {
            transform.position = startPos;
        }
    }
    */

    /* void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Player"))
        {
            col.collider.GetComponent<Rigidbody>().position = respawnPoint.position;
        }
    }
    */

    //void OnCollisionEnter(Collision col)
    //{
    //    if (col.gameObject.name == "Player")
    //    {
    //        this.transform.position = new Vector3(0, 0, 0);
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        Debug.Log("MEEP");
    //    }
    //}

}
