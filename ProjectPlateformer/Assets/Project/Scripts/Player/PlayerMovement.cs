using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [Header("Movement")]
    [SerializeField, Range(1f, 10f)] private float walkSpeed = 4.0f;

    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravityScale = 8.5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [SerializeField, Range(0.01f, 0.5f)] private float extraHeight = 0.02f;
    
    [SerializeField] private LayerMask groundLayerMask;
    
    [SerializeField] private float jumpTimeCounter = 0.35f;
    public float JumpTime { get; set; }

    private Vector2 m_Velocity = Vector2.zero;

    private bool m_IsMoving = false;
    private bool m_IsJumping = false;

    public bool IsMoving { get { return this.m_IsMoving; } }

    private Rigidbody2D m_rb;
    private Collider2D m_Collider;
    private SpriteRenderer m_Render;
    private Animator m_Anim;
    
    private Vector2 m_PlayerDirectionY = Vector2.down;
    
    private bool m_IsAntigravityIsOn = false;
    public bool IsIsAntigravityIsOn { get { return this.m_IsAntigravityIsOn; } }

    private bool m_IsJumpStart = false;
    public bool IsJumpStart { get { return this.m_IsJumpStart; } }

    private bool m_IsOnFloor = false;
    public bool IsOnFloor { get { return this.m_IsOnFloor; } set { this.IsOnFloor = value; } }
    #endregion Variables

    // Start is called before the first frame update
    private void Start()
    {
        m_Collider = GetComponent<Collider2D>();
        m_rb = GetComponent<Rigidbody2D>();
        m_Render = GetComponent<SpriteRenderer>();
        m_Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        m_Velocity.x = Input.GetAxis("Horizontal");

        FlipSprite();
        
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")) && IsGrounded())
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            JumpExtended();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            m_IsJumping = false;
        }

       

        ApplyAnimation();

        //IsGrounded();
        
    }

    private void FixedUpdate()
    {
        Move();
        
        if (m_rb.velocity.y < 0)
        {            
            m_rb.gravityScale = fallMultiplier;
        }
        else if (m_rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            m_rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            m_rb.gravityScale = 1f;
        }
    }

    public bool IsGrounded()
    {
        RaycastHit2D hit2d = Physics2D.Raycast(m_Collider.bounds.center, m_PlayerDirectionY, m_Collider.bounds.extents.y + extraHeight, groundLayerMask);

        Color rayColor;
        rayColor = Color.green;
        if (hit2d.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(m_Collider.bounds.center, m_PlayerDirectionY * (m_Collider.bounds.extents.y + extraHeight), rayColor);
        Debug.Log(hit2d.collider != null);
        return hit2d.collider != null;
    }

    private void Jump()
    {
        m_IsJumping = true;
        JumpTime = jumpTimeCounter;
        //m_rb.velocity = Vector2.up * jumpHeight;
        m_rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
    }

    private void JumpExtended()
    {
        if (JumpTime > 0)
        {
            m_rb.velocity = Vector2.up * jumpHeight;
            JumpTime -= Time.deltaTime;
        }
        else
        {
            m_IsJumping = false;
        }
    }

    public void Move()
    {       

        if (m_IsAntigravityIsOn == false)
        {
            m_rb.velocity = new Vector2(m_Velocity.x * walkSpeed, m_rb.velocity.y);
        }

        if (m_IsAntigravityIsOn)
        {
            m_rb.velocity = new Vector2(m_Velocity.x * walkSpeed, m_rb.velocity.y);
        }
    }

    private void ApplyAnimation()
    {
        if (m_Velocity.x != 0 && IsGrounded())
        {
            m_Anim.SetBool("IsWalking", true);
            m_IsMoving = true;
            m_IsOnFloor = true;
        }
        else
        {
            m_Anim.SetBool("IsWalking", false);
            m_IsMoving = false;
            m_IsOnFloor = false;
        }
    }

    private void FlipSprite()
    {
        float dotprod = Vector2.Dot(m_rb.velocity, transform.right);

        if (dotprod > 0)
        {
           m_Render.flipX = false;
        }
        else if(dotprod < 0)
        {
             m_Render.flipX = true;
        }
    }

}