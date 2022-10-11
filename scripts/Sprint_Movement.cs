using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BaseCharacter))]
public class Sprint_Movement : MonoBehaviour
{
    //Fields
    [SerializeField] private float speedMultiplier = 2.0f;
    //Variables
    BaseCharacter player;
    PlayerInput playerInput;
    InputAction sprintAction;
    //Bools
    
    
    

    private void Awake()
    {
        player = GetComponent<BaseCharacter>();
        playerInput = GetComponent<PlayerInput>();
        sprintAction = playerInput.actions["sprint"];
    }

    void OnEnable() => player.OnBeforeMove += OnBeforeMove;
    void OnDisable() => player.OnBeforeMove -= OnBeforeMove;

    void OnBeforeMove()
    {
        var sprintInput = sprintAction.ReadValue<float>();
        if (sprintInput == 0) return;
        var forwardMovementFactor = Mathf.Clamp01(
            Vector3.Dot(player.transform.forward, player.velocity.normalized)
        );
        var multiplier = Mathf.Lerp(1.0f, speedMultiplier, forwardMovementFactor);
        player.movementSpeedMultiplier *= multiplier;
    }

  
}
