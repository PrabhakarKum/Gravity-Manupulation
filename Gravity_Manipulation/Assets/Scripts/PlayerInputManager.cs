using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    private PlayerControl _playerControl;
    public PlayerManager _playerManager;
    
    [Header("CAMERA MOVEMENT INPUT")]
    [SerializeField] private Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;
    
    [Header("PLAYER MOVEMENT INPUT")]
    public Vector2 movementInput;
    public float horizontalInput;
    public float verticalInput;
    public bool jumpInput = false;
    public float moveAmount;
    
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    private void OnEnable()
    {
        if (_playerControl == null)
        {
            _playerControl = new PlayerControl();
            _playerControl.PlayerMovement.MovementControls.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            _playerControl.PlayerMovement.MovementControls.canceled += ctx => movementInput = Vector2.zero;
            _playerControl.PlayerCamera.CameraControls.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();
            _playerControl.PlayerMovement.Jumping.performed += ctx => jumpInput = true;
            
            _playerControl.PlayerRotate.SelectDirection.started += ctx => _playerManager.gravityController.SelectDirection(ctx);
            _playerControl.PlayerRotate.ConfirmRotate.started += ctx => _playerManager.gravityController.ConfirmRotate(ctx);
            _playerControl.PlayerRotate.DisableRotate.started += ctx => _playerManager.gravityController.DisableRotate(ctx);
        }
        _playerControl.Enable();
    }
    
    private void OnDisable()
    {
        _playerControl.PlayerRotate.Disable();
    }
    
    private void Update()
    {
        HandleAllInput();
    }

    private void HandleAllInput()
    {
        
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleJumpMovementInput();
        
    }
    
    
    private void HandlePlayerMovementInput()
    {
        if(!_playerManager.canMove)
            return;
        
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;
    
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;
        }
        
        if(_playerManager == null)
            return;
        
        _playerManager.playerAnimatorManager.UpdateMovementAnimation(moveAmount);
    }

    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }
    
    private void HandleJumpMovementInput()
    {
        if(!_playerManager.canMove)
            return;
        
        if (jumpInput)
        {
            jumpInput = false;
            _playerManager.playerLocomotionManager.AttemptToPerformJump();
        }
    }
}
