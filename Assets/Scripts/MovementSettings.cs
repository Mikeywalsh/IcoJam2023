using System;

[Serializable]
public class MovementSettings
{
    public float GravityStrength = 9.81f;
    public float HorizontalAcceleration = 10.0f; // In meters/second
    public float HorizontalDeceleration = 25.0f; // In meters/second
    public float MaxHorizontalRunSpeed = 4.0f; // In meters/second
    public float MaxHorizontalSprintSpeed = 5.5f; // In meters/second
    public float DashSpeed = 7.0f; // In meters/second
    public float JumpSpeed = 5.0f; // In meters/second
    public float JumpCooldownGround = 0.5f;
    public float DashCooldown = .9f;
    public float TotalDashDuration = .8f;
    public float MaxFallSpeedAir = 20.0f; // The max speed at which the player can fall in air
}