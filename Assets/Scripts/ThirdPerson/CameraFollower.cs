using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform orientation;

    [Space]

    public bool canLook;

    private float yRotation;
    private float xRotation;

    [Space]

    public float xRotationMin = -90f;
    public float xRotationMax = 90f;

    [Header("Settings")]
    public float sensitivity = 0.2f;

    public bool invertX;
    private int xInvertedValue;

    private Vector3 desiredPos;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (canLook)
            Look();
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, xRotationMin, xRotationMax);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        transform.position = orientation.position;
    }

    private void LateUpdate()
    {

    }
}

