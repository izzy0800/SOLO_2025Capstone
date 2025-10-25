using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJitterFix : MonoBehaviour
{
    [Header("Rigidbody Settings (3D)")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private bool freezePositionDuringCutscene = true;
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool pauseAnimatorDuringCutscene = false;
    [Header("Character Controller (if using)")]
    [SerializeField] private CharacterController characterController;
    private Vector3 frozenPosition;
    private Quaternion frozenRotation;
    private bool isFrozen = false;
    private bool wasKinematic = false;
    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }
    private void Start()
    {
        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            Debug.Log("PlayerJitterFix: Set Rigidbody to Interpolate mode");
        }
    }
    public void FreezeForCutscene()
    {
        Debug.Log("PlayerJitterFix: Freezing player for cutscene");
        if (freezePositionDuringCutscene)
        {
            frozenPosition = transform.position;
            frozenRotation = transform.rotation;
            isFrozen = true;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                wasKinematic = rb.isKinematic;
                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            if (characterController != null)
            {
                characterController.enabled = false;
            }
        }
        if (animator != null && pauseAnimatorDuringCutscene)
        {
            animator.speed = 0f;
        }
    }
    public void UnfreezeAfterCutscene()
    {
        Debug.Log("PlayerJitterFix: Unfreezing player after cutscene");
        if (freezePositionDuringCutscene)
        {
            isFrozen = false;
            if (rb != null)
            {
                rb.isKinematic = wasKinematic;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezeRotationX |
                                 RigidbodyConstraints.FreezeRotationY |
                                 RigidbodyConstraints.FreezePositionZ;
            }
            if (characterController != null)
            {
                characterController.enabled = true;
            }
        }
        if (animator != null && pauseAnimatorDuringCutscene)
        {
            animator.speed = 1f;
        }
    }
    private void LateUpdate()
    {
        if (isFrozen && freezePositionDuringCutscene)
        {
            transform.position = frozenPosition;
            transform.rotation = frozenRotation;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
