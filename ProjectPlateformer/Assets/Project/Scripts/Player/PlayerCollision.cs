﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField]
    private LayerMask m_GroundMaskLayer;
    private int testint;
    private Bounds m_Bounds;

    private bool m_OnGroundCollision;
    //any wall collision left or right
    private bool m_OnWallCollision;
    private bool m_OnRightWallCollision;
    private bool m_OnLeftWallCollision;
    private int m_WallSide;

    [Header("Collision")]
    [Range(0.25f, 1f)]
    [SerializeField]
    private float m_CollisionRadius = 0.25f;
    
    private Vector2 m_BottomOffset, m_RightOffset, m_LeftOffset;
    private Color m_DebugColliderColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        m_Bounds = GetComponent<Collider2D>().bounds;
        m_BottomOffset.y = -m_Bounds.extents.y;
        m_LeftOffset.x = -m_Bounds.extents.x;
        m_RightOffset.x = m_Bounds.extents.x;
    }

    // Update is called once per frame
    void Update()
    {
        m_OnGroundCollision = Physics2D.OverlapCircle((Vector2)transform.position + m_BottomOffset, m_CollisionRadius, m_GroundMaskLayer);

        m_OnWallCollision = Physics2D.OverlapCircle((Vector2)transform.position + m_RightOffset, m_CollisionRadius, m_GroundMaskLayer) || Physics2D.OverlapCircle((Vector2)transform.position + m_LeftOffset, m_CollisionRadius, m_GroundMaskLayer);

        m_OnLeftWallCollision = Physics2D.OverlapCircle((Vector2)transform.position + m_RightOffset, m_CollisionRadius, m_GroundMaskLayer);
        m_OnRightWallCollision = Physics2D.OverlapCircle((Vector2)transform.position + m_LeftOffset, m_CollisionRadius, m_GroundMaskLayer);


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position + m_BottomOffset, m_CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + m_LeftOffset, m_CollisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + m_RightOffset, m_CollisionRadius);
    }
}