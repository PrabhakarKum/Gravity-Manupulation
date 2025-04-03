using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public GravityController gravityController;
    [HideInInspector] public Animator animator;
    
    [Header("Flags")]
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool canMove = false;
    
    [SerializeField] private Transform anchorTransform;
    
    public void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        gravityController = GetComponent<GravityController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        animator.SetBool("IsGrounded", isGrounded);
        playerLocomotionManager.HandleAllMovement();
        DetectPlayerFall();
    }

    public void LateUpdate()
    {
        PlayerCamera.instance.HandleAllCameraActions();
    }

    private void DetectPlayerFall()
    {
        if (anchorTransform == null)
            return;

        float distance = Vector3.Distance(transform.position, anchorTransform.position);
        Debug.Log("player distance of centre: "+ distance);
        float maxDistance = 50f;

        if (distance >= maxDistance || transform.position.y < -50)
        {
            Debug.Log("Distance exceeded maxDistance! Game should end.");
            
            if (GameOver.Instance == null)
            {
                Debug.LogError("GameOver.Instance is null! Game cannot end.");
                return;
            }
            
            GameOver.Instance.GameEnd(1, 0);
        }
    }
    
}
