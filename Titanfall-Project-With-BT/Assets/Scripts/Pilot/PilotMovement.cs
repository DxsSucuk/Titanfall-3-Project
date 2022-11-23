using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

public class PilotMovement : NetworkBehaviour
{
    [Header("Player Components")] CharacterController controller;

    private PlayerInput controls;
    private InputAction jumpAction;

    public Transform groundCheck;

    public LayerMask groundMask;

    public GameObject body;
    public GameObject climbCheck;

    public InputValues wallInputDifference;
    public InputValues wallNormalDirection;
    public InputValues boostInputDifference;

    [Header("Basic Movement")] Vector3 move;
    Vector3 input;
    Vector2 moveData;
    Vector3 Yvelocity;
    Vector3 forwardDirection;
    Vector3 jumpForward;

    float speed;

    public float runSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float airSpeedMultiplier;
    public float climbSpeed;
    float desiredSpeed;

    float gravity;
    public float normalGravity;
    public float wallRunGravity;
    public float jumpHeight;

    bool shouldSprint;
    public bool isSprinting;
    bool shouldCrouch;
    bool isCrouching;
    public bool isSliding;
    bool isWallRunning;
    public bool isGrounded;
    public bool isMoving;

    float jumpCooldown = 0f;
    int jumpCharges;
    float inAir;

    [Header("Crouch")] float startHeight;
    float crouchHeight = 0.5f;
    float slideTimer;
    bool canBoost = true;
    float slideBoostWait;
    public float maxSlideTimer;
    Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    Vector3 standingCenter = new Vector3(0, 0, 0);

    float friction;
    RaycastHit groudHit;

    public float slideSpeedIncrease;

    [Header("Wallrun")] bool onLeftWall;
    bool onRightWall;
    bool hasWallRun = false;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    Vector3 wallNormal;
    Vector3 lastWall;
    float wallDistance = 1f;

    [Header("Climbing")] bool isClimbing;
    bool hasClimbed;
    bool canClimb;
    private RaycastHit wallHit;
    float climbTimer;
    public float maxClimbTimer;

    [Header("WallJumping")] bool isWallJumping;
    float wallJumpTimer;
    public float maxWallJumpTimer;

    [Header("Camera Effects")] public Camera playerCamera;
    float normalFov;
    public float specialFov;
    float slideFov;
    public float cameraChangeTime;
    public float wallRunTilt;
    public float tilt;

    [Header("Lurch")] float lurchTimer;
    bool lurch;
    bool canLurch = true;
    bool groundBoost;

    [Header("TurnInAir")] Vector3 oldForward;
    Vector3 newForward;
    bool turn = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        startHeight = transform.localScale.y;
        normalFov = playerCamera.fieldOfView;

        controls = GetComponent<PlayerInput>();
        jumpAction = controls.actions["Jump"];
    }

    void IncreaseSpeed(float speedIncrease)
    {
        speed += speedIncrease;
    }

    void DecreaseSpeed(float speedDecrease)
    {
        speed -= speedDecrease * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!HasInputAuthority) return;
        
        SecondChanceJump();
        HandleInput();
        CheckClimbing();
        CheckWallRun();
        SlideBoost();
        CheckMoving();

        if (turn && (!isGrounded || isSliding))
        {
            StartCoroutine(TurningDecreasesSpeed());
            turn = false;
        }

        if (isGrounded && !isSliding)
        {
            GroundedMovement();
        }
        else if (!isGrounded && !isWallRunning && !isClimbing)
        {
            AirMovement();
        }
        else if (isSliding)
        {
            SlideMovement();
            slideTimer -= 1f * Time.deltaTime;
            if (slideTimer <= 0)
            {
                isSliding = false;
            }
        }
        else if (isWallRunning)
        {
            WallRunMovement();
        }
        else if (isClimbing)
        {
            ClimbMovement();
            climbTimer -= 1f * Time.deltaTime;
            if (climbTimer < 0)
            {
                if (Physics.Raycast(transform.position, transform.forward, out wallHit, 1f, groundMask) &&
                    Physics.Raycast(climbCheck.transform.position, transform.forward, 4f, groundMask) == false)
                {
                    climbTimer += 0.3f;
                    speed += 3f;
                }
                else
                {
                    hasClimbed = true;
                    isClimbing = false;
                }
            }
        }

        controller.Move(move * Time.deltaTime);
        ApplyGravity();
        CameraEffects();
    }

    void CheckMoving()
    {
        if (input == new Vector3(0f, 0f, 0f) && isGrounded && !isSliding)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
    }

    void FixedUpdate()
    {
        CheckGround();

        if (!isMoving)
        {
            speed = 0f;
        }
        else if (isGrounded && isSliding)
        {
            DecreaseSpeed(friction);
        }
        // Why have two different if-statement if they do the same? why not merge?
        else if (isGrounded)
        {
            speed = Mathf.SmoothStep(speed, desiredSpeed, 9f * Time.deltaTime);
        }
        else if (isWallRunning)
        {
            speed = Mathf.SmoothStep(speed, desiredSpeed, 9f * Time.deltaTime);
        }
    }

    void CameraEffects()
    {
        float fov = isWallRunning && !isGrounded ? specialFov : isSliding ? slideFov : normalFov;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, cameraChangeTime * Time.deltaTime);

        if (isWallRunning)
        {
            if (onRightWall && !isGrounded)
            {
                tilt = Mathf.Lerp(tilt, wallRunTilt, cameraChangeTime * Time.deltaTime);
            }
            else if (onLeftWall && !isGrounded)
            {
                tilt = Mathf.Lerp(tilt, -wallRunTilt, cameraChangeTime * Time.deltaTime);
            }
        }
        else
        {
            tilt = Mathf.Lerp(tilt, 0f, cameraChangeTime * Time.deltaTime);
        }
    }

    public void OnMove(InputValue value)
    {
        moveData = value.Get<Vector2>();
    }

    public void OnSprint(InputValue value)
    {
        shouldSprint = value.isPressed;
    }

    public void OnCrouch(InputValue value)
    {
        shouldCrouch = value.isPressed;
    }

    void HandleInput()
    {
        input = new Vector3(moveData.x, 0f, moveData.y);
        input = transform.TransformDirection(input);
        input = Vector3.ClampMagnitude(input, 1f);

        if (shouldCrouch && !isCrouching)
        {
            Crouch();
        }
        else if (!shouldCrouch && isCrouching)
        {
            ExitCrouch();
        }

        if (Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            Application.Quit();
        }

        if (shouldSprint && isGrounded && !isCrouching && !isSliding)
        {
            isSprinting = true;
        }
        else if (!shouldSprint)
        {
            isSprinting = false;
        }


        if (jumpAction.triggered && jumpCharges > 0)
        {
            Invoke("Jump", jumpCooldown);
        }

        if (!isGrounded && !isWallJumping &&
            (Vector3.Dot(-transform.right, input.normalized) > 0.4 ||
             Vector3.Dot(transform.right, input.normalized) > 0.4) && canLurch)
        {
            speed -= 1f;
            forwardDirection = input;
            lurch = true;
            lurchTimer = 0.3f;
            canLurch = false;
        }
        else if (isGrounded &&
                 (Vector3.Dot(-transform.right, input.normalized) > boostInputDifference.value ||
                  Vector3.Dot(transform.right, input.normalized) > boostInputDifference.value) && groundBoost)
        {
            speed += 0.65f;
            groundBoost = false;
            StartCoroutine(BoostCooldown());
        }
    }

    IEnumerator LurchTimer()
    {
        canLurch = true;
        yield return new WaitForSeconds(0.3f);
        canLurch = false;
    }

    IEnumerator BoostCooldown()
    {
        yield return new WaitForSeconds(3f);
        groundBoost = true;
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);
        if (isGrounded)
        {
            inAir = 0.2f;
            hasWallRun = false;
            hasClimbed = false;
            climbTimer = maxClimbTimer;
            jumpCooldown = 0f;
        }
        else if (!isGrounded)
        {
            jumpCooldown = Mathf.Lerp(0, jumpCooldown, 0.3f);
        }
        else if (isGrounded && Yvelocity.y == 0)
        {
            jumpCharges = 1;
        }
    }

    void CheckWallRun()
    {
        onRightWall = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallDistance, groundMask);
        onLeftWall = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallDistance, groundMask);

        if ((onRightWall && Vector3.Dot(transform.right, input.normalized) > wallInputDifference.value ||
             onLeftWall && Vector3.Dot(-transform.right, input.normalized) > wallInputDifference.value) &&
            !isWallRunning && !isGrounded && !isSliding)
        {
            TestWallRun();
        }

        if (!onRightWall && !onLeftWall && isWallRunning)
        {
            ExitWallRun();
        }
    }

    void CheckClimbing()
    {
        canClimb = Physics.Raycast(transform.position, transform.forward, out wallHit, 1f, groundMask);
        if (Vector3.Dot(wallHit.normal.normalized, Vector3.up) < -wallNormalDirection.value ||
            Vector3.Dot(wallHit.normal.normalized, Vector3.up) > wallNormalDirection.value)
        {
            return;
        }

        float wallAngle = Vector3.Angle(-wallHit.normal, transform.forward);
        if (wallAngle < 15 && canClimb && !hasClimbed &&
            Vector3.Dot(transform.forward, input.normalized) > wallInputDifference.value)
        {
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }
    }

    void GroundedMovement()
    {
        desiredSpeed = isSprinting ? sprintSpeed : isCrouching ? crouchSpeed : runSpeed;

        if (input.x != 0)
        {
            move.x += input.x * speed;
        }
        else
        {
            move.x = 0;
        }

        if (input.z != 0)
        {
            move.z += input.z * speed;
        }
        else
        {
            move.z = 0;
        }

        move = Vector3.ClampMagnitude(move, speed);
    }

    void AirMovement()
    {
        if (isGrounded)
        {
            move.x += input.x * 0.005f;
            move.z += input.z * 0.005f;
        }
        else
        {
            move.x += input.x * airSpeedMultiplier;
            move.z += input.z * airSpeedMultiplier;
        }

        if (isWallJumping)
        {
            move += forwardDirection * airSpeedMultiplier;
            wallJumpTimer -= 1f * Time.deltaTime;
            if (wallJumpTimer <= 0)
            {
                isWallJumping = false;
            }
        }

        if (lurch)
        {
            move += forwardDirection;
            lurchTimer -= 1f * Time.deltaTime;
            if (lurchTimer <= 0)
            {
                lurch = false;
            }
        }

        move = Vector3.ClampMagnitude(move, speed);
    }

    void SlideMovement()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out groudHit, 2.2f, groundMask))
        {
            Vector3 normal = groudHit.normal.normalized;

            float difference = ((Vector3.Dot(normal, transform.forward) * -1) + 1);

            friction = 7f * difference;
        }

        move.x += input.x * airSpeedMultiplier;
        move.z += input.z * airSpeedMultiplier;
        move = Vector3.ClampMagnitude(move, speed);
    }

    void WallRunMovement()
    {
        wallNormal = onRightWall ? rightWallHit.normal : leftWallHit.normal;

        forwardDirection = Vector3.Cross(wallNormal, Vector3.up);

        if (Vector3.Dot(forwardDirection, transform.forward) < 0)
        {
            forwardDirection = -forwardDirection;
        }

        if (Vector3.Dot(forwardDirection.normalized, input.normalized) > 0)
        {
            move += forwardDirection;
        }
        else if (Vector3.Dot(forwardDirection.normalized, input.normalized) < 0)
        {
            move.x = 0;
            move.z = 0;
            ExitWallRun();
        }

        if (Physics.Raycast(transform.position, transform.forward, 1f, groundMask))
        {
            ExitWallRun();
        }

        move.x += input.x * airSpeedMultiplier;
        move = Vector3.ClampMagnitude(move, speed);
    }

    void ClimbMovement()
    {
        if (Vector3.Dot(transform.forward, input.normalized) < wallInputDifference.value)
        {
            hasClimbed = true;
            Yvelocity.y = 1f;
            isClimbing = false;
        }

        forwardDirection = Vector3.up;
        move.x += input.x * airSpeedMultiplier;
        move.z += input.z * airSpeedMultiplier;

        Yvelocity += forwardDirection;
        speed = climbSpeed;

        move = Vector3.ClampMagnitude(move, speed);
        Yvelocity = Vector3.ClampMagnitude(Yvelocity, speed);
    }

    void Crouch()
    {
        controller.height = crouchHeight;
        controller.center = crouchingCenter;
        groundCheck.position += new Vector3(0f, 1f, 0f);
        body.transform.localScale = new Vector3(body.transform.localScale.x, crouchHeight, transform.localScale.z);
        if (speed > 8 || !isGrounded)
        {
            isSliding = true;
            isSprinting = false;
            forwardDirection = input;
            slideBoostWait = 2f;
            if (isGrounded && canBoost)
            {
                IncreaseSpeed(slideSpeedIncrease);
                slideFov = 95f;
                canBoost = false;
            }
            else
            {
                slideFov = 92f;
            }

            slideTimer = maxSlideTimer;
        }

        isCrouching = true;
    }

    void SlideBoost()
    {
        if (!canBoost)
            slideBoostWait -= 1f * Time.deltaTime;
        if (slideBoostWait < 0)
            canBoost = true;
    }

    void ExitCrouch()
    {
        controller.height = (startHeight * 2);
        controller.center = standingCenter;
        groundCheck.position += new Vector3(0f, -1f, 0f);
        body.transform.localScale = new Vector3(body.transform.localScale.x, startHeight, transform.localScale.z);
        isCrouching = false;
        isSliding = false;
    }

    void TestWallRun()
    {
        wallNormal = onRightWall ? rightWallHit.normal : leftWallHit.normal;

        if (Vector3.Dot(wallNormal.normalized, Vector3.up) < -wallNormalDirection.value)
        {
            return;
        }

        if (hasWallRun)
        {
            float wallAngle = Vector3.Angle(wallNormal, lastWall);
            if (wallAngle > 15)
            {
                WallRun();
            }
        }
        else
        {
            hasWallRun = true;
            WallRun();
        }
    }

    void WallRun()
    {
        if (speed < 8)
        {
            desiredSpeed = 10;
        }
        else if (speed < 13)
        {
            desiredSpeed = speed + 2;
        }
        else
        {
            desiredSpeed = speed;
        }

        wallDistance = 2f;
        isWallRunning = true;
        jumpCharges = 1;
        Yvelocity = new Vector3(0f, 0f, 0f);

        forwardDirection = Vector3.Cross(wallNormal, Vector3.up);

        if (Vector3.Dot(forwardDirection, transform.forward) < 0)
        {
            forwardDirection = -forwardDirection;
        }
    }

    void ExitWallRun()
    {
        isWallRunning = false;
        lastWall = wallNormal;
        forwardDirection = wallNormal;
        inAir = 0.2f;
        isWallJumping = true;
        wallJumpTimer = maxWallJumpTimer;
        wallDistance = 1f;
    }


    void Jump()
    {
        if (!isGrounded && !isWallRunning)
        {
            jumpCharges -= 1;
            if (Vector3.Dot(transform.forward, jumpForward) > boostInputDifference.value)
            {
                IncreaseSpeed(1.2f);
            }
        }
        else if (isWallRunning)
        {
            ExitWallRun();
            jumpForward = transform.forward;
        }
        else if (isGrounded)
        {
            StartCoroutine(LurchTimer());
            jumpForward = transform.forward;
        }

        hasClimbed = false;
        climbTimer = maxClimbTimer;
        jumpCooldown = 0.3f;
        float currentJumpHeight = isCrouching ? jumpHeight * 0.8f : jumpHeight;
        Yvelocity.y = Mathf.Sqrt(currentJumpHeight * -2f * normalGravity);
    }

    void ApplyGravity()
    {
        gravity = isWallRunning ? wallRunGravity : isClimbing ? 0f : normalGravity;
        if (!isGrounded)
            Yvelocity.y += gravity * Time.deltaTime;
        controller.Move(Yvelocity * Time.deltaTime);
    }

    void SecondChanceJump()
    {
        if (!isGrounded && !isWallRunning)
            inAir -= 1f * Time.deltaTime;
        if (jumpAction.triggered && inAir > 0)
            jumpCharges += 1;
    }

    IEnumerator TurningDecreasesSpeed()
    {
        oldForward = transform.forward;
        oldForward = transform.TransformDirection(oldForward);

        yield return new WaitForSeconds(0.1f);

        newForward = transform.forward;
        newForward = transform.TransformDirection(newForward);

        float difference = Vector3.Dot(newForward, oldForward);
        float inputDifference = Vector3.Dot(input, oldForward);

        if (difference < 0.98 && inputDifference < 0.7)
        {
            speed -= ((speed * 0.1f) * (1f - difference));
        }

        if (difference < 0.45 && inputDifference < 0.9)
        {
            speed -= ((speed * 0.3f) * (1f - difference));
        }

        if (speed < 0)
            speed = 0;
        turn = true;
    }
}