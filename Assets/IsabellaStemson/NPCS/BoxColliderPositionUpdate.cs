using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderPositionUpdate : MonoBehaviour
{
    public GameObject parent;
    [HideInInspector] public Rigidbody rb;
    public BoxCollider bc;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        bc.center = rb.position;
    }
}
