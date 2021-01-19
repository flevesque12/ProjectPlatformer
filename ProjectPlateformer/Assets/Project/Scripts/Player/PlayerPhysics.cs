using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    // Start is called before the first frame update
    public float gravityModifier = 1f;
    public float walkSpeed = 3f;

    protected Vector2 m_Velocity;
    Vector2 m_TargetVelocity;
    private Rigidbody2D m_Rb;
    

    void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_TargetVelocity = Vector2.zero;
       
    }

    // Update is called once per frame
    void Update()
    {

       
            m_TargetVelocity.x  = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        m_Velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        m_Velocity.x += m_TargetVelocity.x * walkSpeed * Time.deltaTime;

        Vector2 deltaPosition = m_Velocity * Time.deltaTime;

        Vector2 move = Vector2.up * deltaPosition.y;

        
        Movement(move);

    }

    void Movement(Vector2 move)
    {
        m_Rb.velocity += move;
    }
}
