using UnityEngine;
using UnityEngine.InputSystem;   // new Input System
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(CharacterController))]
public class FPSPlayerControllerIS : MonoBehaviour
{
    [Header("Links")]
    public CharacterController characterController;
    public Animator animator;
    public Transform headBone;
    public Transform camPivot;

    [Header("Move")]
    public float walkSpeed = 1f;
    public float crawlSpeed = 0.36f;
    public float crouchSpeed = 0.7f;

    [Tooltip("Base gravity (negative)")]
    public float gravity = -20f;

    [Header("Sprint")]
    [Tooltip("Run speed while sprinting (uses run animation)")]
    public float sprintSpeed = 4.2f;

    [Tooltip("Can the player sprint at all? (gated by unlock + stamina internally)")]
    public bool sprintFeatureEnabled = true; // �ܿ��أ����ڽ�������
    public bool sprintUnlocked = true; 


    [Header("Stamina")]
    public float staminaMax = 100f;
    [HideInInspector] public float staminaCurrent;      // for UI read
    public float staminaDrainPerSecond = 25f;           // while sprinting
    public float staminaRegenPerSecond = 20f;           // while not sprinting
    public float staminaRegenDelay = 1.0f;              // wait after sprint stops

    private float staminaRegenTimer = 0f;


    [Header("View")]
    public float mouseSensitivity = 10f;   // degrees per second
    public float minPitch = -80f;
    public float maxPitch = 70f;
    public Vector3 cameraLocalOffset = new Vector3(0f, -0.03f, 0.04f);
    public bool lockCursor = true;

    [Header("Crouch (Controller height)")]
    public float standingHeight = 0.55f;
    public float crouchingHeight = 0.2f;
    public float heightLerpSpeed = 12f;
    [HideInInspector] public bool crawlLock = false;
    public void SetCrawlLock(bool v) => crawlLock = v;

    [Header("Audio")]
    public Transform monsterTransform;

    // input cache
    private Vector2 moveInput;   // from Move action
    private Vector2 lookInput;   // from Look action
    private bool crouchHeld;     // from Crouch action
    private bool sprintHeld;     // from Sprint action (Shift)

    // state
    private float pitch;
    private Vector3 velocity;
    private bool wasCrouching;

    // audio state
    private bool isMoving;
    private Vector3 lastPosition;

    void OnEnable()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Start()
    {
        // initial capsule height
        characterController.height = standingHeight;
        var c = characterController.center;
        c.y = characterController.height * 0.5f;
        characterController.center = c;

        lastPosition = transform.position;

        // stamina
        staminaCurrent = staminaMax;     // start full
        sprintUnlocked = false;          // <<< locked until you pick up unlock

    }

    // === PlayerInput (Send Messages) ===
    public void OnMove(InputValue value) { moveInput = value.Get<Vector2>(); }
    public void OnLook(InputValue value) { lookInput = value.Get<Vector2>(); }
    public void OnCrouch(InputValue val) { crouchHeld = val.isPressed; }
    public void OnSprint(InputValue val) { sprintHeld = val.isPressed; }  // <- ����

    void Update()
    {
        // 1) View
        float mx = lookInput.x * mouseSensitivity * Time.deltaTime;
        float my = lookInput.y * mouseSensitivity * Time.deltaTime;

        transform.Rotate(0f, mx, 0f);
        pitch -= my;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        if (camPivot) camPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        if (headBone && camPivot)
            camPivot.position = headBone.TransformPoint(cameraLocalOffset);

        // 2) Intent
        Vector2 dir = Vector2.ClampMagnitude(moveInput, 1f);
        bool wantsToMove   = dir.sqrMagnitude > 0.0001f;

        // Force crouch while in a crawl zone (HoleTrigger)
        bool isCrouching   = crouchHeld || crawlLock;

        // Only allow sprint if not crouching/locked
        bool wantsToSprint = sprintHeld && wantsToMove && !isCrouching;


        // 3) Sprint gate: ���� + ���� + ȫ�ֿ���
        bool canSprint = sprintFeatureEnabled && sprintUnlocked && HasStamina();
        bool isSprinting = wantsToSprint && canSprint;

        // ������ת��ռλ��
        if (isSprinting) SpendStamina(Time.deltaTime);
        else RecoverStamina(Time.deltaTime);

        // 4) Speed choose
        float speed =
            isSprinting ? sprintSpeed :
            (wantsToMove && !isCrouching) ? walkSpeed :
            (isCrouching ? crouchSpeed : 0f);

        Vector3 planar = (transform.forward * dir.y + transform.right * dir.x) * speed;

        // 5) Gravity & Move
        if (characterController.isGrounded && velocity.y < 0f) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;

        characterController.Move((planar + velocity) * Time.deltaTime);

        // Clamp player to ground Y = 0 (���������ƽ�����ʱ���������ò�ƽ������ɾ��)
        var p = transform.position;
        p.y = 0f;
        transform.position = p;

        // 6) Crouch: lerp controller height
        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * heightLerpSpeed);
        SetControllerHeight(characterController, targetHeight);

        // 7) Animator parameters
        bool isWalking = wantsToMove && !isCrouching && !isSprinting;
        bool isCrawling = wantsToMove && isCrouching;

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isCrawling", isCrawling);
        animator.SetBool("isSprinting", isSprinting); // <- ����

        if (isCrouching && !wasCrouching)
        {
            animator.SetBool("isCrouching", true);
            animator.ResetTrigger("standUpTrigger");
            animator.SetTrigger("crouchTrigger");
        }
        else if (!isCrouching && wasCrouching)
        {
            animator.SetBool("isCrouching", false);
            animator.ResetTrigger("crouchTrigger");
            animator.SetTrigger("standUpTrigger");
        }
        wasCrouching = isCrouching;

        // 8) Audio: footsteps & heartbeat����������߼���
        Vector3 curPos = transform.position;
        Vector3 horizDelta = curPos - lastPosition;
        horizDelta.y = 0f;
        float horizSpeedSqr = horizDelta.sqrMagnitude / (Time.deltaTime * Time.deltaTime);
        bool shouldPlayFootsteps = characterController.isGrounded && horizSpeedSqr > 0.01f;

        if (shouldPlayFootsteps)
        {
            if (!isMoving)
            {
                isMoving = true;
                if (AudioManager.instance != null)
                    AudioManager.instance.StartPlayerFootsteps();
            }
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;
                if (AudioManager.instance != null)
                    AudioManager.instance.StopPlayerFootsteps();
            }
        }
        lastPosition = curPos;

        if (AudioManager.instance != null && monsterTransform != null)
        {
            float distanceToMonster = Vector3.Distance(transform.position, monsterTransform.position);
            AudioManager.instance.UpdateHeartbeatBySAN(distanceToMonster);
        }
    }

    // helper
    void SetControllerHeight(CharacterController cc, float h)
    {
        cc.height = h;
        var c = cc.center;
        c.y = h * 0.5f;
        cc.center = c;
    }


    private bool HasStamina() => staminaCurrent > 0.5f;   // small threshold to avoid flicker

    private void SpendStamina(float dt)
    {
        staminaCurrent = Mathf.Max(0f, staminaCurrent - staminaDrainPerSecond * dt);
        staminaRegenTimer = staminaRegenDelay;  // reset regen delay while sprinting
    }

    private void RecoverStamina(float dt)
    {
        if (staminaRegenTimer > 0f)
        {
            staminaRegenTimer -= dt;
            return;
        }
        staminaCurrent = Mathf.Min(staminaMax, staminaCurrent + staminaRegenPerSecond * dt);
    }
}
