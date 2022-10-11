using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BaseCharacter))]
public class Jump_Movement : MonoBehaviour
{
    [SerializeField] private float jumpSpeed = 5.0f;
    [SerializeField] private float jumpPressBufferTime = 0.5f;
    [SerializeField] private float jumpGroundGraceTime = 0.2f;


    private bool tryingToJump;
    private float lastJumpPressTime;
    private float lastGroundedTime;
    
    private BaseCharacter player;

    private void Awake()
    {
        player = GetComponent<BaseCharacter>();
    }

    void OnEnable()
    {
        player.OnBeforeMove += OnBeforeMove;
        player.OnGroundStateChange += OnGroundStateChange;
    }

    private void OnDisable()
    {
        player.OnBeforeMove -= OnBeforeMove;
        player.OnGroundStateChange -= OnGroundStateChange;
    }

    void OnJump()
    {
        tryingToJump = true;
        lastJumpPressTime = Time.time;
    }

    void OnBeforeMove()
    {
        bool wasTryingtoJump = Time.time - lastJumpPressTime < jumpPressBufferTime;
        bool wasGrounded = Time.time - lastGroundedTime < jumpGroundGraceTime;

        bool isOrWasTryingToJump = tryingToJump || (wasTryingtoJump && player.isGrounded);
        bool isOrWasGrounded = player.isGrounded || wasGrounded;
        
        
        if (isOrWasTryingToJump && isOrWasGrounded)
        {
            player.velocity.y += jumpSpeed;
        }

        tryingToJump = false;
    }

    void OnGroundStateChange(bool isGrounded)
    {
        if (!isGrounded) lastGroundedTime = Time.time;   
    }
}

