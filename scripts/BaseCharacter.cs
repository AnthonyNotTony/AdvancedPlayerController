using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseCharacter : MonoBehaviour
{
    public Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 3.0f;
    [SerializeField] private float jumpSpeed = 5.0f;
    [SerializeField] private float mass = 1.0f;
    [SerializeField] private float acceleration = 10.0f;
    [SerializeField] private float movementSpeed = 3.0f;

    public bool isGrounded => controller.isGrounded;

    public float Height
    {
        get => controller.height;
        set => controller.height = value;
    }

    public event Action OnBeforeMove;
    public event Action<bool> OnGroundStateChange;

    private CharacterController controller;
    internal Vector3 velocity;
    private Vector2 look;

    private bool wasGrounded;
    
    internal float movementSpeedMultiplier;

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
   // InputAction sprintAction;
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent <PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
        //jumpAction = playerInput.actions["jump"];
        //sprintAction = playerInput.actions["sprint"];
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateGround();
        UpdateGravity(); 
        UpdateMovement(); 
        UpdateMouse();
    }

    void UpdateGround()
    {
        if (wasGrounded != isGrounded)
        {
            OnGroundStateChange?.Invoke(isGrounded);
            wasGrounded = isGrounded;
        }
    }
    void UpdateGravity()
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }

    Vector3 GetMovementInput()
    {
        var moveInput = moveAction.ReadValue<Vector2>();
        var input = new Vector3();
        input += transform.forward * moveInput.y; 
        input += transform.right * moveInput.x; 
        input = Vector3.ClampMagnitude(input, 1f);
        //var sprintInput = sprintAction.ReadValue<float>();
        //var multiplier = sprintInput > 0 ? 1.5f : 1.0f;
        input *= movementSpeed * movementSpeedMultiplier;
        return input;
    }

    void UpdateMovement()
    {

        movementSpeedMultiplier = 1.0f;
        OnBeforeMove?.Invoke();
        //var x = Input.GetAxis("Horizontal");
        //var y = Input.GetAxis("Vertical");
        var input = GetMovementInput();
        
        var factor = acceleration * Time.deltaTime; //Allows for interpolation
        velocity.x = Mathf.Lerp(velocity.x, input.x, factor);
        velocity.z = Mathf.Lerp(velocity.z, input.z, factor);

            /*  var jumpInput = jumpAction.ReadValue<float>();
        if (jumpInput > 0 && controller.isGrounded)
        {
            velocity.y += jumpSpeed;
        } */
        
        //transform.Translate(input * movementSpeed * Time.deltaTime, Space.World);
        controller.Move( velocity * Time.deltaTime);
    }
    void UpdateMouse()
    {
        var lookInput = lookAction.ReadValue<Vector2>();
        //look.x += Input.GetAxis("Mouse X") * mouseSensitivity;
        //look.y += Input.GetAxis("Mouse Y") * mouseSensitivity;
        look.x += lookInput.x * mouseSensitivity;
        look.y += lookInput.y * mouseSensitivity;

        look.y = Mathf.Clamp(look.y, -89.0f, 89.0f);
        
        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }

    
}
