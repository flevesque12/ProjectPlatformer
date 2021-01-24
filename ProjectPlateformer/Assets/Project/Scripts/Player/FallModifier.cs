using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallModifier : MonoBehaviour
{

    private Rigidbody2D m_rb;
    [SerializeField]
    private float m_fallModifier = 2.5f;

    [SerializeField]
    private float m_LowJumpModifier = 2f;


    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();    
    }

    public void FallModifierGravity()
    {
        if(m_rb.velocity.y < 0)
        {
            m_rb.velocity += Vector2.up * Physics2D.gravity.y * (m_fallModifier - 1) * Time.deltaTime;
        }
        else if(m_rb.velocity.y>0 && !Input.GetButton("Jump"))
        {
            m_rb.velocity += Vector2.up * Physics2D.gravity.y * (m_LowJumpModifier - 1) * Time.deltaTime;
        }
    }

}
