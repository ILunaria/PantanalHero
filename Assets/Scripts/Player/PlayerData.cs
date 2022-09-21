using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Movement Presets")]
    public float moveSpeed;
    public float acceleration;
    public float desacceleration;

    [Space(5)]

    [Header("Jump Presets")]
    public float jumpForce;
    public float onGroundGravity;
    public float fallGravityMultiplier;

    [Space(5)]

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Vector2 groundCheckSize;

    [Space(5)]

    [Header("Coyote Time")]
    public float coyoteTime;

    [Space(5)]

    [Header("Jump Buffering")]
    public float jumpBufferTime;
}
