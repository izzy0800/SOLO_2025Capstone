using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Controller;
using UnityEngine;

public class NPCscript : CryptidUtils
{
    [HideInInspector] public Collider col;
    public CharacterSwitch characterSwitch;

    public int height;

    public bool hasCompletedMinigame;
    public GameObject miniGameUI; //updated this to be the slider puzzle
    public GameObject level;

    public SpriteRenderer visualSprite;

    public bool playerInRange;

    private MovePlayerInput moveInput;
    private PlayerMovement playerMovement;

    [Header("Billboard Settings")]
    [SerializeField] private bool enableBillboard = true;
    [SerializeField] private bool lockYAxis = true;
    [SerializeField] private float rotationSmoothTime = 0.5f;
    [SerializeField] private bool maintainScale = false;
    [SerializeField] private float baseDistance = 5f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2f;

    private Camera targetCamera;
    private CinemachineBrain brain;
    private Quaternion currentRotation;
    private Quaternion targetRotation;
    private Vector3 originalScale;
    private ICinemachineCamera lastActiveVcam;
    private bool isTransitioning;
    private float transitionStartTime;
    private Transform spriteTransform;

    [Header("Dialog System")]
    private NPCDialogHandler dialogHandler;

    private Rigidbody rb;
    private CharacterController characterController;

    public bool inGame;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        characterController = GetComponent<CharacterController>();

        moveInput = GetComponent<MovePlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();

        if (moveInput != null)
        {
            moveInput.enabled = false;
        }
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        var charMover = GetComponent<CharacterMover>();
        if (charMover != null)
        {
            charMover.enabled = false;
        }
        

        dialogHandler = GetComponent<NPCDialogHandler>();
        col = GetComponent<Collider>();

        if (characterSwitch == null)
            characterSwitch = FindObjectOfType<CharacterSwitch>();

        if (visualSprite == null)
            visualSprite = GetComponentInChildren<SpriteRenderer>();

        if (enableBillboard && visualSprite != null)
        {
            InitializeBillboard();
        }
    }

    private void InitializeBillboard()
    {
        targetCamera = Camera.main;
        brain = targetCamera?.GetComponent<CinemachineBrain>();
        spriteTransform = visualSprite.transform;
        originalScale = spriteTransform.localScale;
        currentRotation = spriteTransform.rotation;

        if (targetCamera == null)
        {
            Debug.LogError($"No main camera found for billboard on {gameObject.name}!");
            enableBillboard = false;
        }
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!hasCompletedMinigame)
                {
                    if (!inGame)
                    {
                        OpenSliderPuzzle();
                        inGame = true;
                        return;
                        
                    }
                    else
                    {

                    }
                   
                }
                else
                {
                    if (rb != null) rb.isKinematic = false;
                    characterSwitch.SwitchToNPC(this.gameObject);
                }
                //dialog can only happen when possessed
                if (characterSwitch.IsPossessing && characterSwitch.npc == this.gameObject)
                {
                    //dialog handling is done by NPCDialogHandler component
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (enableBillboard && targetCamera != null && visualSprite != null && visualSprite.enabled)
        {
            UpdateBillboard();
        }
    }

    private void UpdateBillboard()
    {
        CheckCameraTransition();

        Vector3 lookDirection = targetCamera.transform.position - spriteTransform.position;

        if (lockYAxis)
        {
            lookDirection.y = 0;
        }

        if (lookDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(lookDirection);

            if (isTransitioning || ShouldSmoothRotation())
            {
                currentRotation = Quaternion.Slerp(currentRotation, targetRotation,
                    Time.deltaTime / rotationSmoothTime);
            }
            else
            {
                currentRotation = targetRotation;
            }

            spriteTransform.rotation = currentRotation;
        }

        if (maintainScale)
        {
            AdjustScaleForDistance();
        }
    }

    private void CheckCameraTransition()
    {
        if (brain != null)
        {
            ICinemachineCamera activeVcam = brain.ActiveVirtualCamera;

            if (activeVcam != lastActiveVcam)
            {
                isTransitioning = true;
                transitionStartTime = Time.time;
                lastActiveVcam = activeVcam;

                Debug.Log($"NPC {gameObject.name}: Camera transitioning to {activeVcam.Name}");
            }

            if (isTransitioning && !brain.IsBlending)
            {
                if (Time.time - transitionStartTime > rotationSmoothTime)
                {
                    isTransitioning = false;
                }
            }
        }
    }

    private bool ShouldSmoothRotation()
    {
        if (brain != null)
        {
            return brain.IsBlending;
        }
        return false;
    }

    private void AdjustScaleForDistance()
    {
        float distance = Vector3.Distance(spriteTransform.position, targetCamera.transform.position);
        float scaleFactor = distance / baseDistance;

        scaleFactor = Mathf.Clamp(scaleFactor, minScale, maxScale);

        spriteTransform.localScale = originalScale * scaleFactor;
    }

   
    private void OpenSliderPuzzle()
    {
        Debug.Log("OPENDED");
        miniGameUI.SetActive(true);

        MiniGameController m = miniGameUI.GetComponent<MiniGameController>();
        m.associatedNPC = this;
        m.CreatePuzzle();

        if (level != null)
            level.SetActive(false);

        if (moveInput != null)
        {
            moveInput.enabled = false;
        }
        else if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnPuzzleCompleted()
    {
        hasCompletedMinigame = true;
        inGame = false;
        miniGameUI.SetActive(false);

        if (level != null)
            level.SetActive(true);

        if (moveInput != null)
        {
            moveInput.enabled = true;
        }
        else if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (characterSwitch != null)
        {
            if (rb != null) rb.isKinematic = false;
            characterSwitch.SwitchToNPC(this.gameObject);
        }
        else
        {
            Debug.LogError("CharacterSwitch not found! cannot switch camera to NPC.");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("fOUND");
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }


    //toggling npc sprite RAHH RAHHH RAHHH
    public void SetSpriteVisible(bool visible)
    {
        Debug.Log($"Setting {gameObject.name} sprite visible: {visible}");

        if (visualSprite != null)
        {
            visualSprite.enabled = visible;
        }

        SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in allSprites)
        {
            sr.enabled = visible;
        }

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.enabled = visible;
        }

        if (!visible)
        {
            enableBillboard = false;
        }
        else
        {
            enableBillboard = true;
            if (spriteTransform != null && targetCamera != null)
            {
                Vector3 lookDirection = targetCamera.transform.position - spriteTransform.position;
                if (lockYAxis) lookDirection.y = 0;
                if (lookDirection != Vector3.zero)
                {
                    spriteTransform.rotation = Quaternion.LookRotation(lookDirection);
                    currentRotation = spriteTransform.rotation;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (enableBillboard && targetCamera != null && visualSprite != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 lookDir = (targetCamera.transform.position - visualSprite.transform.position).normalized;
            if (lockYAxis) lookDir.y = 0;
            Gizmos.DrawRay(visualSprite.transform.position, lookDir * 2f);

            if (maintainScale)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(visualSprite.transform.position, baseDistance);
            }
        }
    }
}
