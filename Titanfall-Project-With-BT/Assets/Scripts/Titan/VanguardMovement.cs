using Fusion;
using System.Collections;
using System.Collections.Generic;
using Networking.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

public class VanguardMovement : NetworkBehaviour
{
    CharacterController controller;

    public PlayerInput controls;
    public Transform groundCheck;

    public LayerMask groundMask;

    public Animator titanAnimator;

    bool isRunning;
    public bool isWalking;
    public bool isSprinting;
    public bool isDashing;
    bool isGrounded;
    public bool isMoving;

    float speed;
    public float walkSpeed;
    public float runSpeed;
    public float sprintSpeed;
    public float normalGravity;

    Vector3 input;
    Vector3 move;
    Vector3 forwardDirection;
    Vector3 Yvelocity;

    NetworkTitanInput _networkTitanInput;
    
    EnterVanguardTitan enterScript;

    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        enterScript = GetComponent<EnterVanguardTitan>();

        controls = GetComponent<PlayerInput>();
    }


    void HandleInput()
    {
        input = new Vector3(_networkTitanInput.moveData.x, 0f, _networkTitanInput.moveData.y);

        titanAnimator.SetFloat("moveX", input.x, 0.1f, Time.deltaTime);
        titanAnimator.SetFloat("moveZ", input.z, 0.1f, Time.deltaTime);

        input = transform.TransformDirection(input);
        input = Vector3.ClampMagnitude(input, 1f);

        if (_networkTitanInput.Buttons.IsSet(TitanButtons.Sprint) && !isWalking)
        {
            isSprinting = true;
        }

        if (!_networkTitanInput.Buttons.IsSet(TitanButtons.Sprint))
        {
            isSprinting = false;
        }

        if (_networkTitanInput.Buttons.IsSet(TitanButtons.Walk) && !isSprinting)
        {
            isWalking = true;
        }

        if (!_networkTitanInput.Buttons.IsSet(TitanButtons.Walk))
        {
            isWalking = false;
        }

        if (_networkTitanInput.Buttons.IsSet(TitanButtons.Dash) && !isDashing)
        {
            StartCoroutine(HandleDash());
        }
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority) return;

        GetInput<NetworkTitanInput>(out _networkTitanInput);

        if (enterScript != null && enterScript.inTitan)
        {
            HandleInput();
            CheckSprinting();
            CheckRunning();
            if (isDashing)
            {
                Dash();
            }
            else if (isGrounded)
            {
                Movement();
            }
            else if (!isGrounded)
            {
                AirMovement();
            }

            controller.Move(move * Time.deltaTime);
            CheckGround();
            HandleAnimation();
        }
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);
        if (!isGrounded)
        {
            ApplyGravity();
        }
    }

    void CheckSprinting()
    {
        if (isSprinting)
        {
            if (Vector3.Dot(input, transform.forward) < 0.1)
            {
                isSprinting = false;
            }
        }
    }

    void CheckRunning()
    {
        if (!isSprinting && !isWalking && isMoving)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    void Movement()
    {
        speed = isSprinting ? sprintSpeed : isWalking ? walkSpeed : runSpeed;
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

        if (input == new Vector3(0f, 0f, 0f))
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

        move = Vector3.ClampMagnitude(move, speed);
    }

    void AirMovement()
    {
        move.x += input.x * 1f;
        move.z += input.z * 1f;

        move = Vector3.ClampMagnitude(move, speed);
    }

    void Dash()
    {
        move += forwardDirection;
        move = Vector3.ClampMagnitude(move, speed);
    }

    IEnumerator HandleDash()
    {
        titanAnimator.SetTrigger("dodge");
        speed += 27f;
        forwardDirection = input;
        isDashing = true;

        yield return new WaitForSeconds(0.2f);

        isDashing = false;
    }

    void HandleAnimation()
    {
        titanAnimator.SetBool("run", isRunning);
        titanAnimator.SetBool("walk", isWalking);
        titanAnimator.SetBool("sprint", isSprinting);
    }

    void ApplyGravity()
    {
        Yvelocity.y += normalGravity * Time.deltaTime;
        controller.Move(Yvelocity * Time.deltaTime);
    }
}