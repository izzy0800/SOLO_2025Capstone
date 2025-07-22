    using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThirdPersonCamera : CryptidUtils
{
    public GameObject target;
    private Collider col;
    [SerializeField] private GameObject cam;
    public Vector3 offset = Vector3.zero;
    public float smoothSpeed = 5f;

    [SerializeField]
    private float _mouseSensitivity = 3;

    private float _rotationY;
    private float _rotationX;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _distanceFromTarget = 3;

    // Raycast settings for moving camera away from walls
  //[SerializeField] private float _distanceFromWalls = 1;
    [SerializeField] private LayerMask _layerMask;

    private Vector3 _currentRotation;
    private Vector3 _smoothVelocity = Vector3.zero;

    [SerializeField]
    private float _smoothTime = 0.2f;

    [SerializeField]
    private Vector2 _rotationXMinMax = new Vector2(-40, 40);


    //ZOOM TRIALING
    public float zoomSpeed = 5f;
    public float miniDistance = 2f;
    public float maxDistance = 10f;


    // Start is called before the first frame update
    void Start()
    {
        offset += cam.transform.localPosition;
        offset.y += transform.position.y;
        cam.transform.localPosition = Vector3.zero;

        //collider got gotten
        col = target.GetComponent<Collider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (target != null)
        {
            col = target.GetComponent<Collider>();
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            _rotationY += mouseX;
            _rotationX += mouseY;
            _rotationX = Mathf.Clamp(_rotationX, _rotationXMinMax.x, _rotationXMinMax.y);
            //Vector3 desiredPosition = target.transform.position + offset;

            //transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            Vector3 nextRotation = new Vector3(_rotationX, _rotationY);
            // Apply damping between rotation changes
            _currentRotation = Vector3.SmoothDamp(_currentRotation, nextRotation, ref _smoothVelocity, _smoothTime);
            transform.localEulerAngles = _currentRotation;

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            _distanceFromTarget -= scroll * zoomSpeed;
            _distanceFromTarget = Mathf.Clamp(_distanceFromTarget, miniDistance, maxDistance);

            transform.position = _target.position - transform.forward * _distanceFromTarget;

            Quaternion desiredRot = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, smoothSpeed * Time.deltaTime);

            cam.transform.position = transform.position;
            cam.transform.rotation = transform.rotation;

            Debug.DrawLine(transform.position, target.transform.position, Color.green);
        }
        else
        {
            if (target == null)
                LogWarning("Target is not assigned or is null!");

            if (col == null)
                LogWarning("Collider not found on target!");
        }

        

        // Apply clamping for x rotation 
        _rotationX = Mathf.Clamp(_rotationX, _rotationXMinMax.x, _rotationXMinMax.y);

        
        // Substract forward vector of the GameObject to point its forward vector to the target
        transform.position = _target.position - transform.forward * _distanceFromTarget;

        cam.transform.position = transform.position;
        cam.transform.rotation = transform.rotation;

        //handle player rotation
    }

}
