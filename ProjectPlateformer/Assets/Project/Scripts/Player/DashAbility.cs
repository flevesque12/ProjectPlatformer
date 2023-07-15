using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    [SerializeField] private float m_DashDistance = 5f;
    [SerializeField] private float m_DashDurationTimer = 0.2f;
    private bool m_IsDashing = false;
    private Vector3 m_DashDirection;
    private Vector2 dashDirection;
    private float m_DashTimer = 0f;
    private Rigidbody2D m_Rb;

    // Start is called before the first frame update
    void Start()
    {
        m_Rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) && !m_IsDashing)
        {
            //StartDashAbility();

            // Get the dash direction based on the input axis (you can modify this according to your controls)
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            // Normalize the direction vector to ensure consistent dash distance
            dashDirection = new Vector2(moveX, moveY).normalized;

            // Start the dash coroutine
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate() {
    /*
        if(m_IsDashing){
            m_Rb.MovePosition(transform.position + m_DashDirection * m_DashDistance * (m_DashTimer / m_DashDurationTimer)*Time.deltaTime);
            m_DashTimer -= Time.fixedDeltaTime;

            if(m_DashTimer <= 0f){
                m_IsDashing = false;
            }
        }  */ 
    }

    private void StartDashAbility()
    {
        m_IsDashing = true;
        m_DashDirection = new Vector3(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"),0f).normalized;
        m_DashTimer = m_DashDurationTimer;
    }

    private IEnumerator Dash(){
        m_IsDashing = true;

        //disable other movement ability here 

        // Apply the dash force
        m_Rb.AddForce(dashDirection * m_DashDistance, ForceMode2D.Impulse);
        

        // Keep the dash active for the specified duration
        m_DashTimer = m_DashDurationTimer;
        while (m_DashTimer > 0f)
        {
            m_DashTimer -= Time.deltaTime;
            yield return null;
        }
        Debug.Log("Dash");

        // Reset the velocity after the dash to prevent the character from sliding
        m_Rb.velocity = Vector2.zero;

        m_IsDashing = false;

        // Enable movement abilities or controls after the dash ends
    }
/*
    private IEnumerator DashingWait(){

    }*/
}
