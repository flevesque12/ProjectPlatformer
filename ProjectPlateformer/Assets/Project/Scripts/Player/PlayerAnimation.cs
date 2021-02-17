using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_Animator;


    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

   
    public void FlipSprite(int side)
    {

    }

    public void WalkAnimation(float x)
    {
        m_Animator.SetFloat("HorizontalAxis", x);
    }

    public void RunAnimation(bool isTouchingGround)
    {

    }
}
