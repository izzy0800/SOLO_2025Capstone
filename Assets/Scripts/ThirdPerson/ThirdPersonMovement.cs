using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    BasePlayerInput playerInput;

    [HideInInspector] public Animator anim;

    public Transform orientation;
    public Transform playerModel;

    Vector3 moveDirection;
    [HideInInspector] public Rigidbody rb;

    [Header("Values")]
    public float playerHeight;
    public float movementSpeed = 7;
    public float turnSpeed = 6;

    float groundDrag = 3.5f;
    float airMultiplier = 2;

    [Header("Actions")]

    public bool canMove = true;
    public bool canLook = true;

    public bool isGrounded;

    float moveAmount;

    private void Start()
    {
        playerInput = GetComponent<BasePlayerInput>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;
    }

    private void Update()
    {
        //Ground Check!
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f); //Can add a layermask.
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        if (!canMove)
            StopMovement();
        else
            HandleMovement();

        if(canLook)
            HandleLookDirection();
    }

    void HandleMovement()
    {
        moveDirection = orientation.forward * playerInput.movementVector.y + orientation.right * playerInput.movementVector.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(playerInput.movementVector.x) + Mathf.Abs(playerInput.movementVector.y));

        if (isGrounded)
        {
            anim.SetFloat("Vertical", moveAmount, 0.2f, Time.deltaTime);
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f, ForceMode.Force);

            //Can also add if we're sprinting.
        }
        else
        {
            //We're falling.
            rb.AddForce(moveDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    void HandleLookDirection()
    {
        moveDirection = Quaternion.AngleAxis(Camera.main.transform.rotation.eulerAngles.y, Vector3.up) * moveDirection;
        moveDirection.Normalize();
    }

    void StopMovement()
    {
        moveAmount = 0;
        rb.velocity = Vector3.zero;

        anim.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
    }
}
