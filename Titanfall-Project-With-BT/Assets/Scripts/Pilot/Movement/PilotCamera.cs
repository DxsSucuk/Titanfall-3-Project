using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

public class PilotCamera : NetworkBehaviour
{
    public float minX = -60f;
    public float maxX = 60f;

    public float sensitivity;
    public Camera cam;

    Vector2 look;
    float rotY = 0f;
    float rotX = 0f;

    PilotMovement move;

    public float sprintBobSpeed;
    public float runBobSpeed;
    public float sprintBobAmount;
    public float runBobAmount;
    float defaultY;
    private float timer;

    void Start()
    {
        if (!HasInputAuthority)
        {
            cam.enabled = false;
            if (cam.gameObject.TryGetComponent(out AudioListener audioListener))
            {
                audioListener.enabled = false;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        move = GetComponentInParent<PilotMovement>();

        defaultY = cam.transform.localPosition.y;
    }

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>()*sensitivity;
    }

    void Update()
    {
        rotY += look.x;
        rotX += look.y;

        rotX = Mathf.Clamp(rotX, minX, maxX);

        transform.localEulerAngles = new Vector3(0, rotY, 0);
        cam.transform.localEulerAngles = new Vector3(-rotX, 0, move.tilt);

        HandleHeadBob();
    }

    void HandleHeadBob()
    {
        if (move.isMoving && move.isGrounded && !move.isSliding)
        {
            timer += Time.deltaTime * (move.isSprinting ? sprintBobSpeed : runBobSpeed);
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, defaultY + Mathf.Sin(timer) * (move.isSprinting ? sprintBobAmount : runBobAmount), cam.transform.localPosition.z);
        }
    }

}
