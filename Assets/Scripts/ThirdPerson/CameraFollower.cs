using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public BasePlayerInput playerInput;
    ThirdPersonMovement playerLocomotion;

    public Transform target;

    [Space]

    public Vector3 cameraOffset = new Vector3(0, 2.2f, 10f);
    public float cameraHeight = 2f;

    private Quaternion targetRotation;

    private float yRotation;
    private float xRotation;

    [Space]

    public float xRotationMin = -5f;
    public float xRotationMax = 35f;

    [Header("Settings")]

    [Range(0f, 1f)]
    public float sensitivity = 0.2f;

    public bool invertX;
    private int xInvertedValue;

    private Vector3 desiredPos;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        xInvertedValue = invertX ? -1 : 1;

        playerLocomotion = playerInput.GetComponentInChildren<ThirdPersonMovement>();
    }

    private void Update()
    {
        if (playerLocomotion.canLook)
        {
            yRotation += playerInput.mouseInput.x * sensitivity;
            xRotation += playerInput.mouseInput.y * sensitivity * xInvertedValue;

            xRotation = Mathf.Clamp(xRotation, xRotationMin, xRotationMax);

            //Orientation of player
            Vector3 viewDir = playerLocomotion.transform.position - new Vector3(transform.position.x, playerLocomotion.transform.position.y, transform.position.z);
            playerLocomotion.orientation.forward = viewDir.normalized;

            Vector3 inputDir = playerLocomotion.orientation.forward * playerInput.movementVector.y + playerLocomotion.orientation.right * playerInput.movementVector.x;

            if (inputDir != Vector3.zero)
                playerLocomotion.playerModel.forward = Vector3.Slerp(playerLocomotion.playerModel.forward, inputDir.normalized, Time.deltaTime * playerLocomotion.turnSpeed);

        }
    }

    private void LateUpdate()
    {
        //Move camera
        targetRotation = Quaternion.Euler(xRotation, yRotation, 0.0f);
        desiredPos = target.position - targetRotation * cameraOffset + Vector3.up * cameraHeight;
        transform.SetPositionAndRotation(desiredPos, targetRotation);

    }
}

