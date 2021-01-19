using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float m_MoveSpeed = 10f;    
    private Vector2 m_Direction;
    private bool m_facingRight = true;

    Rigidbody2D m_rb;
    Animator m_Anim;

    [SerializeField] private float m_MaxSpeed = 7f;
    [SerializeField] private float m_LinearDrag = 4f;
    [SerializeField] private float gravity = 1f;
    [SerializeField] private float fallMultiplier = 5f;

    public bool onGround = true;
    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float xMove = Input.GetAxis("Horizontal");
        //float yMove = Input.GetAxis("Vertical");

        m_Direction = new Vector2(xMove, 0f);
    }


    private void FixedUpdate()
    {
        MoveCharacterHorizontal(m_Direction.x);
        modifyPhysics();
    }

    private void MoveCharacterHorizontal(float horizontal)
    {
        m_rb.AddForce(Vector2.right * horizontal * m_MoveSpeed);
        
        //limit our addforce to max speed
        if (Mathf.Abs(m_rb.velocity.x) > m_MaxSpeed)
        {
            m_rb.velocity = new Vector2(Mathf.Sign(m_rb.velocity.x) * m_MaxSpeed, m_rb.velocity.y);
        }       
    }

    void modifyPhysics()
    {
        bool changingDirections = (m_Direction.x > 0 && m_rb.velocity.x < 0) || (m_Direction.x < 0 && m_rb.velocity.x > 0);

        if (onGround)
        {
            if (Mathf.Abs(m_Direction.x) < 0.4f || changingDirections)
            {
                m_rb.drag = m_LinearDrag;
            }
            else
            {
                m_rb.drag = 0f;
            }
            m_rb.gravityScale = 0;
        }
        else
        {
            m_rb.gravityScale = gravity;
            m_rb.drag = m_LinearDrag * 0.15f;
            if (m_rb.velocity.y < 0)
            {
                m_rb.gravityScale = gravity * fallMultiplier;
            }
            else if (m_rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                m_rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }
}
