﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_Animator;
    private PlayerCollision m_PlayerCollision;
    

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_PlayerCollision = GetComponent<PlayerCollision>();
        
    }

    private void Update()
    {
        m_Animator.SetBool("onGround", m_PlayerCollision.OnGroundCollision);
    }

    public void FlipSprite(int side)
    {

    }

    public void WalkAnimation(float x)
    {
        
        m_Animator.SetFloat("speed", Mathf.Abs(x));
    }

    public void RunAnimation(bool isTouchingGround)
    {

    }
}
