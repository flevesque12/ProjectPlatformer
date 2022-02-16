using DigitalRuby.Tween;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("Movement")]
    [SerializeField, Range(1f, 10f)] private float m_WalkSpeed = 4.0f;
    [SerializeField] private float m_DashSpeed = 1f;
    [SerializeField] private float m_SlideSpeed = 1f;
    [SerializeField, Range(0f, 10f)] private float m_JumpHeight = 10f;
    [SerializeField] private float m_WallJumpLerping = 10f;
    [SerializeField] private int m_Side = 1;
    [SerializeField] private float m_DashDistance = 100.0f;

    private float xRaw;
    private float yRaw;

    private ColorTween colorTween;
    [SerializeField] private float jumpTimeCounter = 0.2f;
    public float JumpTime { get; set; }

    private Vector2 m_Velocity = Vector2.zero;
    private Vector3 testVelocity;

    private bool m_IsMoving = false;
    private bool m_IsJumping = false;

    public bool IsMoving { get { return this.m_IsMoving; } }

    private bool m_IsDash = false;
    private bool m_HasDashed = false;

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
    private SpriteRenderer m_SprRenderer;

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
        m_SprRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        m_Velocity.x = Input.GetAxis("Horizontal");
        m_Velocity.y = Input.GetAxis("Vertical");
        xRaw = Input.GetAxisRaw("Horizontal");
        yRaw = Input.GetAxisRaw("Vertical");
        m_Velocity = Vector2.ClampMagnitude(m_Velocity, 1f);
        
        //to change
        FlipSprite();

        if (m_PlayerCollision.OnGroundCollision && !m_PlayerCollision.OnWallCollision && m_Velocity.x != 0)
        {
            m_PlayerAnimation.WalkAnimation(m_Velocity.x);
        }
        else
        {
            m_PlayerAnimation.WalkAnimation(Vector2.zero.x);
        }

        Move();

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
                m_Anim.SetBool("IsJumping", true);
                m_IsJumping = true;
                Jump(Vector2.up);
            }

            if (m_PlayerCollision.OnWallCollision && !m_PlayerCollision.OnGroundCollision)
            {
                //wall jump here
                WallJump();
            }
        }

        //this code cause a bug in the wall jump
        /*
        if (Input.GetButton("Jump") && m_IsJumping)
        {
            if(!m_PlayerCollision.OnWallCollision)
            JumpExtended();
        }*/

        if (Input.GetButtonUp("Jump") && m_PlayerCollision.OnGroundCollision)
        {
            m_Anim.SetBool("IsJumping", false);
            m_IsJumping = false;
        }

        //Add dash here
        if(Input.GetButtonDown("Fire1"))
        {
            
            if (xRaw != 0 || yRaw != 0)
            {
                Debug.Log("handle dash");
                HandleDash(xRaw, yRaw);
            }
        }


        //Grab Wall
        if (m_PlayerCollision.OnWallCollision && Input.GetButton("Fire3") && m_CanMove)
        {
            if(m_Side != m_PlayerCollision.WallSide)
            {
                FlipSprite(m_Side * -1);
            }

            m_IsGrabWall = true;
            m_IsWallSlide = false;
        }

        if (Input.GetButtonUp("Fire3") || !m_PlayerCollision.OnWallCollision && !m_CanMove)
        {
            m_IsGrabWall = false;
            m_IsWallSlide = false;
        }

        //if he on the ground he dont wall jump
        if (m_PlayerCollision.OnGroundCollision)
        {
            m_IsWallJump = false;
            //m_FallModifier.enabled = true;
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

        /*
        //that maybe not necessary(have to change)
        if (m_Velocity.x > 0)
        {
            m_Side = 1;
            FlipSprite(m_Side);
        }

        if(m_Velocity.x < 0)
        {
            m_Side = -1;
            FlipSprite(m_Side);
        }*/
    }

    private void FixedUpdate()
    {
        if (!m_PlayerCollision.OnGroundCollision)
        {
            m_IsWallJump = false;
            m_FallModifier.FallModifierGravity();
        }
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
        //m_rb.AddForce(direction * m_JumpHeight,ForceMode2D.Impulse);
    }

    private void WallJump()
    {
        StartCoroutine(DisableMovementCoroutine(0.1f));

        Vector2 _wallDirection = m_PlayerCollision.OnRightWallCollision ? Vector2.left : Vector2.right;

        //Debug.Log(_wallDirection);
        //m_rb.AddForce(new Vector2(_wallDirection.x,Vector2.up.y * m_JumpHeight), ForceMode2D.Impulse);

        Jump(Vector2.up / 1.5f + _wallDirection / 1.5f);

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

        if (m_IsGrabWall)
        {
            return;
        }

        if (!m_IsWallJump)
        {
            //only move
            m_rb.velocity = new Vector2(m_Velocity.x * m_WalkSpeed, m_rb.velocity.y);
        }
        else
        {
            //when he slide the wall
            m_rb.velocity = Vector2.Lerp(m_rb.velocity, (new Vector2(m_Velocity.x * m_WalkSpeed, m_rb.velocity.y)), m_WallJumpLerping * Time.deltaTime);
        }
    }

    public void HandleDash(float x, float y)
    {
        m_HasDashed = true;

        m_rb.velocity = Vector2.zero;

        Vector2 dir = new Vector2(x, y);

        m_rb.velocity += dir.normalized * m_DashDistance;
        //m_rb.AddForce(dir.normalized * m_DashDistance);
        //m_FallModifier.enabled = false;
    }


    private void FlipSprite(int side)
    {
        if (m_IsGrabWall || m_IsWallSlide)
        {
            if (side == -1 && m_SprRenderer.flipX)
            {
                return;
            }

            if (side == 1 && !m_SprRenderer.flipX)
            {
                return;
            }
        }

        bool flipState = (side == 1) ? false : true;
        m_SprRenderer.flipX = flipState;
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
        if (!m_CanMove)
        {
            return;
        }

        bool _pushWall = false;

        if ((m_rb.velocity.x > 0 && m_PlayerCollision.OnRightWallCollision) || (m_rb.velocity.x < 0 && m_PlayerCollision.OnLeftWallCollision))
        {
            _pushWall = true;
        }

        float _push = _pushWall ? 0 : m_rb.velocity.x;

        m_rb.velocity = new Vector2(_push, -m_SlideSpeed);
    }

    private IEnumerator DisableMovementCoroutine(float timer)
    {
        m_CanMove = false;
        yield return new WaitForSeconds(timer);
        m_CanMove = true;
    }
}