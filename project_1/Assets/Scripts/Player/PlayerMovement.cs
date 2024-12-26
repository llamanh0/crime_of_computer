// Assets/Scripts/Player/PlayerMovement.cs
using UnityEngine;
using System.Collections;
using MyGame.Core.Utilities; // Singleton için gerekli namespace

namespace MyGame.Player
{
    /// <summary>
    /// Oyuncunun temel hareket, zıplama, duvar kayma, duvar zıplama ve dash gibi işlevlerini yönetir.
    /// Ek olarak, kamera tetikleyicisine girildiğinde MoveCamera korutinini başlatır.
    /// Kod paneli (isCodePanelActive) açık olduğunda input devre dışı kalır.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private TrailRenderer dashTrail;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform wallCheck;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask wallLayer;

        [Header("Settings")]
        [SerializeField] private PlayerSettings playerSettings;

        private float horizontalInput;
        private bool isFacingRight = true;
        private bool isWallSliding;
        private bool isWallJumping;
        private bool isSideClimbing;
        private bool isDashing;
        private bool canDash = true;
        private float wallJumpingCounter;
        private float wallJumpingDirection;
        private bool canDoubleJump;
        private float coyoteTimeCounter;
        private float jumpBufferCounter;

        private bool isCodePanelActive = false;
        private Coroutine cameraMoveCoroutine;

        private Rigidbody2D rb;
        private Animator animator;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            if (playerSettings == null)
            {
                Debug.LogError("PlayerSettings not assigned in PlayerMovement.");
            }
        }

        private void Update()
        {
            if (isDashing || isCodePanelActive) return;

            HandleInput();
            HandleWallSlide();
            HandleWallJump();
            FlipCharacter();
            UpdateAnimatorParameters();
        }

        private void FixedUpdate()
        {
            if (!isWallJumping && !isDashing && !isCodePanelActive)
            {
                MoveCharacter();
            }
        }

        private void HandleInput()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");

            // Coyote time
            if (IsGrounded())
            {
                coyoteTimeCounter = playerSettings.coyoteTime;
                canDoubleJump = true;
            }
            else
            {
                coyoteTimeCounter -= Time.fixedDeltaTime;
            }

            // Jump buffer
            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferCounter = playerSettings.jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.fixedDeltaTime;
            }

            // Jumping
            if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
            {
                Jump();
                jumpBufferCounter = 0f;
            }
            else if (jumpBufferCounter > 0f && canDoubleJump)
            {
                Jump();
                canDoubleJump = false;
                jumpBufferCounter = 0f;
            }

            // Variable jump height
            if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * playerSettings.variableJumpHeightMultiplier);
            }

            // Dash input
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(Dash());
            }
        }

        private void MoveCharacter()
        {
            rb.linearVelocity = new Vector2(horizontalInput * playerSettings.speed, rb.linearVelocity.y);
        }

        private void Jump()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, playerSettings.jumpingPower);
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(groundCheck.position, playerSettings.checkRadius, groundLayer);
        }

        private bool IsWalled()
        {
            return Physics2D.OverlapCircle(wallCheck.position, playerSettings.checkRadius, wallLayer);
        }

        private void HandleWallSlide()
        {
            if (IsWalled() && !IsGrounded() && horizontalInput != 0f)
            {
                isWallSliding = true;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -playerSettings.wallSlidingSpeed, float.MaxValue));
            }
            else
            {
                isWallSliding = false;
            }
        }

        private void HandleWallJump()
        {
            if (isWallSliding)
            {
                wallJumpingCounter = playerSettings.wallJumpingTime;
                isWallJumping = false;
                wallJumpingDirection = -Mathf.Sign(transform.localScale.x); // Facing directionın tersine
            }
            else
            {
                wallJumpingCounter -= Time.fixedDeltaTime;
            }

            if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
            {
                isWallJumping = true;
                rb.linearVelocity = new Vector2(wallJumpingDirection * playerSettings.wallJumpingPower.x, playerSettings.wallJumpingPower.y);
                wallJumpingCounter = 0f;

                FlipCharacterImmediately();
                Invoke(nameof(ResetWallJumping), playerSettings.wallJumpingDuration);
            }
        }

        private IEnumerator Dash()
        {
            canDash = false;
            isDashing = true;

            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;

            rb.linearVelocity = new Vector2(transform.localScale.x * playerSettings.dashingPower, 0f);

            if (dashTrail != null)
                dashTrail.emitting = true;

            yield return new WaitForSeconds(playerSettings.dashingTime);

            if (dashTrail != null)
                dashTrail.emitting = false;

            rb.gravityScale = originalGravity;
            isDashing = false;

            yield return new WaitForSeconds(playerSettings.dashingCooldown);
            canDash = true;
        }

        private void ResetWallJumping()
        {
            isWallJumping = false;
        }

        private void FlipCharacter()
        {
            if (isWallJumping) return;

            if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f))
            {
                FlipCharacterImmediately();
            }
        }

        private void FlipCharacterImmediately()
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

        private void OnDrawGizmos()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(groundCheck.position, playerSettings.checkRadius);
            }

            if (wallCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(wallCheck.position, playerSettings.checkRadius);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("CameraTrigger"))
            {
                CameraTrigger trigger = collision.GetComponent<CameraTrigger>();
                if (trigger != null)
                {
                    if (cameraMoveCoroutine != null)
                    {
                        StopCoroutine(cameraMoveCoroutine);
                    }
                    cameraMoveCoroutine = StartCoroutine(MoveCamera(trigger.targetPosition));
                }
            }
        }

        private IEnumerator MoveCamera(Vector3 targetPosition)
        {
            while (Vector3.Distance(mainCamera.transform.position, targetPosition) > playerSettings.cameraStopDistance)
            {
                Vector3 newPosition = Vector3.Lerp(mainCamera.transform.position, targetPosition, playerSettings.cameraSmoothSpeed * Time.deltaTime);
                newPosition.z = mainCamera.transform.position.z;
                mainCamera.transform.position = newPosition;
                yield return null;
            }
        }

        public void SetCodePanelState(bool isActive)
        {
            isCodePanelActive = isActive;
        }

        private void UpdateAnimatorParameters()
        {
            float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
            animator.SetFloat("Speed", horizontalSpeed);

            bool grounded = IsGrounded();
            bool isJumping = !grounded && rb.linearVelocity.y > 0.1f;
            bool isFalling = !grounded && rb.linearVelocity.y < -0.1f;

            animator.SetBool("IsJumping", isJumping);
            animator.SetBool("IsFalling", isFalling);
            bool isLanding = (!isFalling && grounded && rb.linearVelocity.y < 0f);
            animator.SetBool("IsLanding", isLanding);

            animator.SetBool("IsWallSliding", isWallSliding);
            animator.SetBool("IsSideClimbing", isSideClimbing);
            animator.SetBool("IsDashing", isDashing);
            animator.SetBool("IsGrounded", grounded);
        }
    }
}
