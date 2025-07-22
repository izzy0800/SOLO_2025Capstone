using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : CryptidUtils
{
    public float movespeed = 1f;
    public float hoverSpeed = 3f; //ADJUST THIS FOR HOW FAST PLAYER RISES/FALLS
    public float maxHoverHeight = 10f;
    public float minFloatHeight = 0.5f;

    [HideInInspector] public Rigidbody rb;
    public Vector3 moveInput;
    public Vector3 moveVelocity;
    public GameObject followCamera;

    private float verticalVelocity = 0f; //this is for hover function

    public bool allowVerticalMovement = true; //SET TO FALSE DURING POSSESSION

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        followCamera = Camera.main.gameObject;
        rb.useGravity = false;
        rb.drag = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        //followCamera = ThirdPersonCamera.Instance.transform;
        Vector3 camForward = followCamera.transform.forward;
        Vector3 camRight = followCamera.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Camera-relative movement direction
        Vector3 inputDirection = new Vector3(moveX, 0, moveZ);
        moveInput = camForward * inputDirection.z + camRight * inputDirection.x;
        moveVelocity = moveInput.normalized * movespeed;

        //logic for da rotation
        if (moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.deltaTime);
        }

        //hover logic
        verticalVelocity = 0f;

        if (allowVerticalMovement)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                verticalVelocity = hoverSpeed;
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                verticalVelocity = -hoverSpeed;
            }

            // clamping hieght RAH
            float currentY = transform.position.y;
            if ((currentY >= maxHoverHeight && verticalVelocity > 0) ||
                (currentY <= minFloatHeight && verticalVelocity < 0))
            {
                verticalVelocity = 0f;
            }
        }

        //comb horizontal and vertical
        Vector3 fullmove = moveVelocity + new Vector3(0f, verticalVelocity, 0f);
        rb.MovePosition(rb.position + fullmove * Time.deltaTime);

        //rotation BODY TEA
        if (moveInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        //Horizontal movement
        Vector3 horizontalMove = moveVelocity * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + horizontalMove);

        if (allowVerticalMovement)
        {
            float currentY = transform.position.y;
            bool canAscend = currentY < maxHoverHeight;
            bool canDescend = currentY > minFloatHeight;

            Vector3 verticalForce = Vector3.zero;

            if (Input.GetKey(KeyCode.Space) && canAscend)
            {
                verticalForce += Vector3.up * hoverSpeed;
            }
            else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && canDescend)
            {
                verticalForce += Vector3.down * hoverSpeed;
            }
            else
            {

                //this will add a gentle floaty gravity effect if not actively hovering (hope hope hope)
                verticalForce += Vector3.down * (hoverSpeed * 0.2f); //ADJUST THIS FOR FLOATY FALL
            }

            rb.AddForce(verticalForce, ForceMode.Acceleration);
        }

        Vector3 clampedPos = transform.position;
        clampedPos.y = Mathf.Clamp(clampedPos.y, minFloatHeight, maxHoverHeight);
        transform.position = clampedPos;

    }

}
