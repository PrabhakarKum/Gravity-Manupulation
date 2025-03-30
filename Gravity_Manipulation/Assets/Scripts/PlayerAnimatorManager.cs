using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviour
{
    private PlayerManager playerManager;
    
    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        
    }
    
    public void UpdateMovementAnimation(float moveAmount)
    {
        playerManager.animator.SetFloat("Running", moveAmount, 0.1f, Time.deltaTime);
    }

    public void PlayTargetActionAnimation(string targetAnimation)
    {
        playerManager.animator.CrossFade(targetAnimation, 0.2f);
    }
}
