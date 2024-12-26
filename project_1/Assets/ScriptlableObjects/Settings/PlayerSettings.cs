// Assets/ScriptableObjects/Settings/PlayerSettings.cs
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Settings/PlayerSettings", order = 1)]
public class PlayerSettings : ScriptableObject
{
    [Header("Movement Settings")]
    public float speed = 8f;
    public float jumpingPower = 16f;

    [Header("Dash Settings")]
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    [Header("Wall Slide Settings")]
    public float wallSlidingSpeed = 2f;

    [Header("Side Climb Settings")]
    public float sideClimbSpeed = 2f;

    [Header("Wall Jump Settings")]
    public float wallJumpingTime = 0.2f;
    public float wallJumpingDuration = 0.4f;
    public Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [Header("Ground and Wall Check")]
    public float checkRadius = 0.2f;

    [Header("Advanced Jump Settings")]
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;
    public float variableJumpHeightMultiplier = 0.5f;

    [Header("Camera Settings")]
    public float cameraSmoothSpeed = 2f;
    public float cameraStopDistance = 0.1f;
}
