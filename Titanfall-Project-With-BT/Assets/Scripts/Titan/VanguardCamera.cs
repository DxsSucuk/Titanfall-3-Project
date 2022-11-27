using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class VanguardCamera : NetworkBehaviour
{
    public Animator titanAnimator;
    public float minX = -60f;
    public float maxX = 60f;
 
    public float sensitivity;
    public Camera cam;
 
    [Networked] private float RotY { get; set; }
    
    [Networked] private float RotX { get; set; }
    
    [Networked] private float Yaw { get; set; }
 
    EnterVanguardTitan enterScript;
    VanguardMovement moveScript;
 
    bool shouldHeadBob;
 
    public float sprintBobSpeed;
    public float walkBobSpeed;
    public float runBobSpeed;
    public float walkBobAmount;
    public float sprintBobAmount;
    public float runBobAmount;
    float defaultY;
    private float timer;
 
    Vector3 lastMousePosition;
    Vector2 look;
 
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
 
        defaultY = cam.transform.localPosition.y;
 
        enterScript = GetComponent<EnterVanguardTitan>();
        moveScript = GetComponent<VanguardMovement>();
    }

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
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority)
        {
            transform.localEulerAngles = new Vector3(0, RotY, 0);
            cam.transform.localEulerAngles = new Vector3(-RotX, 0, 0);
        }
        else
        {

            if (enterScript.inTitan)
            {

                Vector2 lookWithSens = moveScript.networkTitanInput.look * sensitivity;;
                RotY += lookWithSens.x;
                RotX += lookWithSens.y;

                RotX = Mathf.Clamp(RotX, minX, maxX);

                transform.localEulerAngles = new Vector3(0, RotY, 0);
                cam.transform.localEulerAngles = new Vector3(-RotX, 0, 0);

                Yaw -= Input.GetAxisRaw("Mouse Y") * 0.1f;
                Yaw = Mathf.Clamp(Yaw, -1f, 1f);

                titanAnimator.SetFloat("aim", Yaw, 0.1f, Time.deltaTime);

                HandleHeadBob();
            }
        }
    }
 
    void HandleHeadBob()
    {
        if (moveScript.isMoving && !moveScript.isDashing)
        {
            timer += Time.deltaTime * (moveScript.isSprinting ? sprintBobSpeed : moveScript.isWalking ?  walkBobSpeed : runBobSpeed);
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, defaultY + Mathf.Sin(timer) * (moveScript.isSprinting ? sprintBobAmount : moveScript.isWalking ? walkBobAmount : runBobAmount), cam.transform.localPosition.z);
        }
    }

}
