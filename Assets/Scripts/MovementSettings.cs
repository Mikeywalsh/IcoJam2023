using System;
using UnityEngine;

[Serializable]
public class MovementSettings
{
    [Header("Run")]
    public float HorizontalAcceleration = 10.0f;
    public float HorizontalDeceleration = 25.0f;
    public float MaxHorizontalRunSpeed = 4.0f;

    [Header("Jump")]
    public float JumpSpeed = 5.0f;
    public float JumpCooldownGround = 0.5f;
    
    [Header("Dash")]
    public float HorizontalDashSpeed = 5.5f;
    public float TotalDashDuration = .1f;
    public float DashCooldown = .9f;
    
    [Header("Fall")]
    public float GravityStrength = 9.81f;
    public float MaxFallSpeedAir = 20.0f;
}