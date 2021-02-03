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

    public void WalkAnimation(float velocity, bool isTouchingGround)
    {
        if(velocity != 0 && isTouchingGround)
        {
            m_Animator.SetBool("IsWalking", true);
        }
        else
        {
            m_Animator.SetBool("IsWalking", false);
        }
    }

    public void RunAnimation(bool isTouchingGround)
    {

    }
}
