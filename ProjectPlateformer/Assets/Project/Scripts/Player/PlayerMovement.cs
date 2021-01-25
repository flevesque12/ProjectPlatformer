using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("Movement")]
    [SerializeField, Range(1f, 10f)] private float walkSpeed = 4.0f;
    [SerializeField] private float m_SlideSpeed = 5f;
    [SerializeField] private float jumpHeight = 3f;
    
       
    [SerializeField] private float jumpTimeCounter = 0.35f;
    public float JumpTime { get; set; }

    private Vector2 m_Velocity = Vector2.zero;

    private bool m_IsMoving = false;
    private bool m_IsJumping = false;

    public bool IsMoving { get { return this.m_IsMoving; } }

    private Vector2 m_PlayerDirectionY = Vector2.down;

    private bool m_IsDash = false;
    private bool m_IsJumpStart = false;
    private bool m_IsGrabWall;
    private bool m_IsWallSlide;
    public bool IsJumpStart { get { return this.m_IsJumpStart; } }

    private bool m_IsOnFloor = false;
    public bool IsOnFloor { get { return this.m_IsOnFloor; } set { this.IsOnFloor = value; } }

    private PlayerCollision m_PlayerCollision;
    private FallModifier m_FallModifier;

    private Rigidbody2D m_rb;
    private Collider2D m_Collider;
    private SpriteRenderer m_Render;
    private Animator m_Anim;

    #endregion Variables

    // Start is called before the first frame update
    private void Start()
    {
        m_Collider = GetComponent<Collider2D>();
        m_rb = GetComponent<Rigidbody2D>();
        m_Render = GetComponent<SpriteRenderer>();
        m_Anim = GetComponent<Animator>();
        m_PlayerCollision = GetComponent<PlayerCollision>();
        m_FallModifier = GetComponent<FallModifier>();
    }

    // Update is called once per frame
    private void Update()
    {
        m_Velocity.x = Input.GetAxis("Horizontal");

        FlipSprite();

        if (Input.GetButtonDown("Jump") && m_PlayerCollision.OnGroundCollision)
        {
            //m_IsJumping = true;
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

        /*error prone*/
        if(m_PlayerCollision.OnWallCollision && Input.GetButton("Jump"))
        {
            m_IsGrabWall = true;
            m_IsWallSlide = false;
        }

        if (m_IsGrabWall && !m_IsDash)
        {
            m_rb.gravityScale = 0;
            if (m_Velocity.x > .2f || m_Velocity.x < -.2f)
                m_rb.velocity = new Vector2(m_rb.velocity.x, 0);

            float speedModifier = m_Velocity.y > 0 ? .5f : 1;

            m_rb.velocity = new Vector2(m_rb.velocity.x, m_Velocity.y * (walkSpeed * speedModifier));
        }
        else
        {
            m_rb.gravityScale = 3;
        }
        /**/

        if (m_PlayerCollision.OnWallCollision && !m_PlayerCollision.OnGroundCollision)
        {
            if(m_Velocity.x !=0 && !m_IsGrabWall)
            {
                m_IsWallSlide = true;
                WallSlide();
            }
        }

        if(!m_PlayerCollision.OnWallCollision || m_PlayerCollision.OnGroundCollision)
        {
            m_IsWallSlide = false;
        }

        ApplyAnimation();        
    }

    private void FixedUpdate()
    {
        Move();
        m_FallModifier.FallModifierGravity();
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
        m_rb.velocity = new Vector2(m_Velocity.x * walkSpeed, m_rb.velocity.y);
    }

    private void ApplyAnimation()
    {
        if (m_Velocity.x != 0 && m_PlayerCollision.OnGroundCollision)
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
        else if (dotprod < 0)
        {
            m_Render.flipX = true;
        }
    }

    private void WallSlide()
    {
        bool _pushWall = false;

        if((m_rb.velocity.x > 0 && m_PlayerCollision.OnRightWallCollision)||(m_rb.velocity.x < 0 && m_PlayerCollision.OnLeftWallCollision))
        {
            _pushWall = true;
        }

        float _push = _pushWall ? 0 : m_rb.velocity.x;

        m_rb.velocity = new Vector2(_push, -m_SlideSpeed);
    }
}