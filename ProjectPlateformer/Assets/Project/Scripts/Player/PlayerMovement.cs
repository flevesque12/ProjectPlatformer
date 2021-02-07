using UnityEngine;
using DigitalRuby.Tween;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("Movement")]
    [SerializeField, Range(1f, 10f)] private float m_WalkSpeed = 4.0f;
    [SerializeField] private float m_SlideSpeed = 2f;
    [SerializeField] private float m_JumpHeight = 50f;
    [SerializeField] private float m_WallJumpLerping = 10f;

    private ColorTween colorTween;
    [SerializeField] private float jumpTimeCounter = 0.2f;
    public float JumpTime { get; set; }

    private Vector2 m_Velocity = Vector2.zero;

    private bool m_IsMoving = false;
    private bool m_IsJumping = false;

    public bool IsMoving { get { return this.m_IsMoving; } }

    
    private bool m_IsDash = false;
    private bool m_IsJumpStart = false;
    private bool m_IsGrabWall;
    private bool m_IsWallSlide;
    private bool m_IsWallJump;
    private bool m_IsRunning = false;
    private bool m_CanMove = true;

    public bool IsJumping { get { return this.m_IsJumping; } }

    private bool m_IsOnFloor = false;
    public bool IsOnFloor { get { return this.m_IsOnFloor; } set { this.IsOnFloor = value; } }

    private PlayerCollision m_PlayerCollision;
    private FallModifier m_FallModifier;
    private PlayerAnimation m_PlayerAnimation;

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
        m_PlayerAnimation = GetComponent<PlayerAnimation>();
    }

    // Update is called once per frame
    private void Update()
    {
        m_Velocity.x = Input.GetAxis("Horizontal");
        m_Velocity.y = Input.GetAxis("Vertical");
        FlipSprite();

        //Wall Slide
        if (m_PlayerCollision.OnWallCollision && !m_PlayerCollision.OnGroundCollision)
        {
            if (m_Velocity.x != 0 && !m_IsGrabWall)
            {
                m_IsWallSlide = true;
                WallSlide();
            }
        }

        if (!m_PlayerCollision.OnWallCollision || m_PlayerCollision.OnGroundCollision)
        {
            m_IsWallSlide = false;
        }

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (m_PlayerCollision.OnGroundCollision)
            {
                m_IsJumping = true;
                Jump(Vector2.up);
            }

            if (m_PlayerCollision.OnWallCollision && !m_PlayerCollision.OnGroundCollision)
            {
                //wall jump here
                WallJump();
            }
        }
        
        if (Input.GetButton("Jump"))
        {            
            JumpExtended();            
        }
    

        if (Input.GetButtonUp("Jump"))
        {
            m_IsJumping = false;
        }

        //Grab Wall        
        if(m_PlayerCollision.OnWallCollision && Input.GetButton("Fire3"))
        {
            m_IsGrabWall = true;
            m_IsWallSlide = false;
        }

        //if he on the ground he dont wall jump
        if (m_PlayerCollision.OnGroundCollision)
        {
            m_IsWallJump = false;
        }

        if (m_IsGrabWall && !m_IsDash)
        {
            m_rb.gravityScale = 0;
            if (m_Velocity.x > .2f || m_Velocity.x < -.2f)
                m_rb.velocity = new Vector2(m_rb.velocity.x, 0);

            float speedModifier = m_Velocity.y > 0 ? .5f : 1;

            m_rb.velocity = new Vector2(m_rb.velocity.x, m_Velocity.y * (m_WalkSpeed * speedModifier));
        }
        else
        {
            m_rb.gravityScale = 3;
        }
                
              
    }

    private void FixedUpdate()
    {
        Move();
        m_FallModifier.FallModifierGravity();
    }
       

    private void Jump()
    {        
        JumpTime = jumpTimeCounter;
        //m_rb.velocity = Vector2.up * jumpHeight;
        m_rb.velocity = new Vector2(m_rb.velocity.x, 0f);
        m_rb.AddForce(Vector2.up * m_JumpHeight, ForceMode2D.Impulse);
        
    }

    private void Jump(Vector2 direction)
    {
        JumpTime = jumpTimeCounter;
        m_rb.velocity = new Vector2(m_rb.velocity.x, 0f);
        m_rb.velocity += direction * m_JumpHeight;
    }

    private void WallJump()
    {
        StartCoroutine(DisableMovementCoroutine(0.1f));

        Vector2 _wallDirection = m_PlayerCollision.OnRightWallCollision ? Vector2.left : Vector2.right;

        //Debug.Log(_wallDirection);
        //m_rb.AddForce(new Vector2(_wallDirection.x,Vector2.up.y * m_JumpHeight), ForceMode2D.Impulse);
        
        Jump(Vector2.up + _wallDirection);
        
        m_IsWallJump = true;
    }

    private void JumpExtended()
    {
        if (JumpTime > 0)
        {
            m_rb.velocity = Vector2.up * m_JumpHeight;
            JumpTime -= Time.deltaTime;
        }
        else
        {
            m_IsJumping = false;
        }
    }

    public void Move()
    {
        if (!m_CanMove)
        {
            return;
        }

        if(m_IsGrabWall) {
            return;
        }

        if (!m_IsWallJump)
        {
            m_rb.velocity = new Vector2(m_Velocity.x * m_WalkSpeed, m_rb.velocity.y);
            m_PlayerAnimation.WalkAnimation(m_Velocity.x, m_PlayerCollision.OnGroundCollision);
        }
        else
        {
            m_rb.velocity = Vector2.Lerp(m_rb.velocity, (new Vector2(m_Velocity.x * m_WalkSpeed, m_rb.velocity.y)), m_WallJumpLerping * Time.deltaTime);
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

    IEnumerator DisableMovementCoroutine(float timer)
    {
        m_CanMove = false;
        yield return new WaitForSeconds(timer);
        m_CanMove = true;
    }
    
}