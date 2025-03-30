using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public PlayerManager playerManager;
    public Camera cameraObject;
    [SerializeField] Transform cameraPivotTransform;

    [Header("Camera Settings")]
    private Vector3 cameraVelocity;
    [SerializeField] private float leftAndRightRotationSpeed = 100;
    [SerializeField] private float upAndDownRotationSpeed = 100;
    [SerializeField] private float minimumPivot = -30f; //THE LOWEST POINT YOU ARE ABLE TO LOOK DOWN
    [SerializeField] private float maximumPivot = 60f;  //THE HIGHEST POINT YOU ARE ABLE TO LOOK UP
    [SerializeField] private float cameraCollisionRadius = 0.2f;
    [SerializeField] private LayerMask collideWithLayer;
    
    [Header("Camera values")]
    [SerializeField] private float leftAndRightLookAngle;
    [SerializeField] private float upAndDownLookAngle;
    private Vector3 cameraObjectPosition; // USED FOR CAMERA COLLISION ( MOVES THE CAMERA OBJECT TO THIS POSITION UPON COLLIDING)
    private float cameraSmoothSpeed = 1;
    private float cameraZPosition;  //VALUES USED FOR CAMERA COLLISION
    private float targetCameraZPosition;   //VALUES USED FOR CAMERA COLLISION

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if(playerManager !=  null)
        {
            HandleFollowTarget();
            HandleRotation();
            HandleCollisions();
        }
    }
    
    private void HandleFollowTarget()
{
    Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, playerManager.transform.position, ref cameraVelocity, cameraSmoothSpeed*Time.deltaTime);
    transform.position = targetCameraPosition;
}

private void HandleRotation()
{
     //ROTATE LEFT AND RIGHT BASED ON HORIZONTAL ON THE MOUSE
    leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;

    // ROTATE UP AND DOWN BASED ON VERTICAL ON THE MOUSE
    upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;

    // CLAMP THE UP AND DOWN LOOK ANGLER BETWEEN MIN AND MAX
    upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);


    Vector3 cameraRotation = Vector3.zero;
    Quaternion targetRotation;

    // ROTATE THIS GAME OBJECT LEFT AND RIGHT
    cameraRotation.y = leftAndRightLookAngle;
    targetRotation = Quaternion.Euler(cameraRotation);
    transform.rotation = targetRotation; 


    // ROTATE THE PIVOT GAME OBJECT UP AND DOWN
    cameraRotation = Vector3.zero;
    cameraRotation.x = upAndDownLookAngle;
    targetRotation = Quaternion.Euler(cameraRotation);
    cameraPivotTransform.localRotation = targetRotation;
}

private void HandleCollisions()
{
    targetCameraZPosition = cameraZPosition;
    RaycastHit hit;

    // DIRECTION FOR THE COLLISION TO CHECK 
    Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
    direction.Normalize();

    // WE CHECK IF THERE IS ANY OBJECT IN FRONT OF OUR DESIRED DIRECTION * 
    if(Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayer))
    {
        //IF THERE IS ANY OBJECT , WE GET OUR DISTANCE FROM IT
        Debug.DrawRay(cameraPivotTransform.position, direction * Mathf.Abs(targetCameraZPosition), Color.green);
        float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);

        // WE THEN EQUATE OUR DISTANCE FROM IT
        targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
    }
    
    // IF OUR TARGET POSITION IS LESS THAN OUR COLLISION RADIUS, WE SUBSTRACT OUR COLLISION RADIUS
    if(Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
    {
        targetCameraZPosition = -cameraCollisionRadius;
    }

    // WE THEN APPLY OUR FINAL POSITION USING A LERP OVER A TIME OF 0.2F SECONDS
    cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
    cameraObject.transform.localPosition = cameraObjectPosition;
    
}

void OnDrawGizmos()
{
    // Draw a wireframe sphere to visualize the SphereCast
    Gizmos.color = Color.red; // Set the color to red
    Gizmos.DrawWireSphere(cameraPivotTransform.position, cameraCollisionRadius); // Draw the sphere
}
}
