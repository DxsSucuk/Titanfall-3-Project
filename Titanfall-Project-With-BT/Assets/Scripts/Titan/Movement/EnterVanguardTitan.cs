using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class EnterVanguardTitan : NetworkBehaviour
{
    public Animator titanAnimator;

    CharacterController controller;
    BoxCollider rangeCheck;

    public GameObject embarkRifle;
    public GameObject Rifle;

    public GameObject playerCamera;
    public GameObject player;
    public GameObject titanCamera;
    public GameObject embarkTitanCamera;
    public Transform groundCheck;

    public LayerMask groundMask;

    Vector3 Yvelocity;

    public bool isFalling;
    public bool isGrounded;
    public bool isEmbarking;
    public bool inTitan;
    public bool inRangeForEmbark;

    public float fallingSpeed;

    public Transform embarkPos;
    public Transform embarkLookTarget;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        rangeCheck = GetComponent<BoxCollider>();

        Rifle.SetActive(false);
    }

    private void Start()
    {
        isFalling = true;
        titanAnimator.SetTrigger("StartFall");
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);
        if (isGrounded)
        {
            isFalling = false;
        }
    }

    [Rpc]
    private void Player_HideRPC()
    {
        if (Runner.TryGetPlayerObject(Object.InputAuthority, out NetworkObject networkObject))
        {
            networkObject.gameObject.SetActive(false);
        }
    }

    [Rpc]
    private void Player_ShowRPC()
    {
        if (Runner.TryGetPlayerObject(Object.InputAuthority, out NetworkObject networkObject))
        {
            networkObject.gameObject.SetActive(true);
            networkObject.gameObject.GetComponent<AccesTitan>().ExitTitan();
        }
    }
    
    public IEnumerator Embark()
    {
        playerCamera.SetActive(false);
        embarkTitanCamera.SetActive(true);
        rangeCheck.enabled = false;
        PlayAnimationRPC();
        isEmbarking = true;

        yield return new WaitForSeconds(1.5f);

        embarkRifle.SetActive(false);
        Rifle.SetActive(true);

        yield return new WaitForSeconds(2.4f);

        player.transform.parent = transform;
        Player_HideRPC();
        embarkTitanCamera.SetActive(false);
        titanCamera.SetActive(true);
        
        SetTitanRPC(true, false);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void PlayAnimationRPC()
    {
        titanAnimator.SetTrigger("Embark");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void SetTitanRPC(bool titan, bool embark)
    {
        inTitan = titan;
        isEmbarking = embark;
    }

    void OnTriggerEnter()
    {
        inRangeForEmbark = true;
    }

    void OnTriggerExit()
    {
        inRangeForEmbark = false;
    }

    void ExitTitan()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Player_ShowRPC();
            playerCamera.SetActive(true);
            titanCamera.SetActive(false);
            SetTitanRPC(false, false);
            rangeCheck.enabled = true;
            player.transform.parent = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inTitan && HasInputAuthority)
        {
            ExitTitan();
        }
        else if (isFalling && !inTitan && HasStateAuthority)
        {
            Fall();
        }
    }

    void Fall()
    {
        Yvelocity.y += fallingSpeed * Time.deltaTime;
        controller.Move(Yvelocity * Time.deltaTime);
    }
}