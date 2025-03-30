using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : MonoBehaviour
{
    private PlayerManager _playerManager;

    //values from input manager
    public float verticalMovement { get; private set; }
    public float horizontalMovement { get; private set; }
    public float moveAmount { get; private set; }

    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    
    [SerializeField] private float runningSpeed;
    [SerializeField] private float rotationSpeed;
    
    [Header("Ground Check & Jumping")]
    [SerializeField] private float gravityForce = -5.55f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckSphereRadius = 1f;
    [SerializeField] private Vector2 yVelocity;
    [SerializeField] private float groundedYVelocity = -20f;
    [SerializeField] private float fallStartYVelocity = -5f;
    [SerializeField] private float jumpHeight = 3f;
    private bool fallingVelocityHasBeenSet;

    [Header("Jump Flags")] 
    [SerializeField] private Vector3 jumpDirection;
    [SerializeField] private float jumpForwardSpeed;
    [SerializeField] private float freeFallSpeed;
    protected  void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        HandleGroundCheck();
        if (_playerManager.isGrounded)
        {
            if (yVelocity.y < 0)
            {
                fallingVelocityHasBeenSet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            if (!_playerManager.isJumping && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartYVelocity;
            }
            yVelocity.y += gravityForce * Time.deltaTime;
        }
        
        _playerManager.characterController.Move(yVelocity * Time.deltaTime);
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleFreeFallMovement();
    }
    
    public void GetHorizontalAndVerticalInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;
    }
    private void HandleGroundedMovement()
    {
        GetHorizontalAndVerticalInputs();
        
         moveDirection = PlayerCamera.instance.transform.forward * verticalMovement; 
         moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
         moveDirection.Normalize();
         moveDirection.y = 0;
        
        if (PlayerInputManager.instance.moveAmount > 0.5f)
        {
            _playerManager.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
        }
    
    }
    
    private void HandleJumpingMovement()
    {
        if (_playerManager.isJumping)
        {
            _playerManager.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }
    
    private void HandleFreeFallMovement()
    {
        if (!_playerManager.isGrounded)
        {
            Vector3 freeFallDirection;
            freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
            freeFallDirection += PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;
            freeFallDirection.y = 0;
            _playerManager.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        // if (_playerManager.isDead)
        //     return;
        //
        // if (!_playerManager.canRotate)
        //     return;
    
        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;
    
        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }
    
        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    private void HandleGroundCheck()
    {
        _playerManager.isGrounded = Physics.CheckSphere(_playerManager.transform.position, groundCheckSphereRadius, groundLayer);
    }

    public void AttemptToPerformJump()
    {
        if(_playerManager.isJumping)
            return;
        
        if(!_playerManager.isGrounded)
            return;
        _playerManager.playerAnimatorManager.PlayTargetActionAnimation("Falling Idle");
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityForce);
        _playerManager.isJumping = true;
        
        jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
        jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;

        jumpDirection.y = 0;
        if (jumpDirection != Vector3.zero)
        {
            if (PlayerInputManager.instance.moveAmount > 0.5f)
            {
                jumpDirection *= 0.5f;
            }
        }
    }
    
    public void OnDrawGizmosSelected()
    {
        if (_playerManager == null)
        {
            _playerManager = GetComponent<PlayerManager>();
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_playerManager.transform.position, groundCheckSphereRadius);
    }
}
