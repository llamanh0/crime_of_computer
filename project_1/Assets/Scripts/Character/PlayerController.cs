using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraSmoothSpeed = 2f;
    [SerializeField] private float cameraStopDistance = 0.1f; // Kamera hedefe yaklaştığında durma mesafesi

    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;

    [Header("Dash Settings")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer dashTrail; // 'tr' ismini 'dashTrail' yaptım, okunurluk için

    [Header("Wall Slide Settings")]
    [SerializeField] private float wallSlidingSpeed = 2f;

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallJumpingTime = 0.2f;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [Header("Ground and Wall Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float checkRadius = 0.2f;

    [Header("Advanced Jump Settings")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float variableJumpHeightMultiplier = 0.5f;

    // Hareket kontrolü
    private float horizontal;
    private bool isFacingRight = true;
    private bool isWallSliding;
    private bool isWallJumping;
    private bool isDashing;
    private bool canDash = true;
    private float wallJumpingCounter;
    private float wallJumpingDirection;
    private bool canDoubleJump;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    // Kod paneli aktifken hareketi engellemek için
    private bool isCodePanelActive = false;

    // Kamera korutini kontrolü
    private Coroutine cameraMoveCoroutine;

    // Referanslar
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Kod paneli açıksa veya dash halinde ise input'u devre dışı bırak
        if (isDashing || isCodePanelActive) return;

        HandleInput();
        HandleWallSlide();
        HandleWallJump();
        FlipCharacter();
    }

    private void FixedUpdate()
    {
        // Kod paneli veya wall jump / dash sırasında hareketi kapat
        if (!isWallJumping && !isDashing && !isCodePanelActive)
        {
            MoveCharacter();
        }
    }

    private void HandleInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        // Coyote Time mantığı
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            canDoubleJump = true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump Buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Tek/double jump kontrolü
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

        // Değişken zıplama yüksekliği
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * variableJumpHeightMultiplier);
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void MoveCharacter()
    {
        // 'linearVelocity' yerine 'velocity' kullanıyoruz
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, checkRadius, wallLayer);
    }

    private void HandleWallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            // Duvar kenarındayken düşüş hızını sınırla
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
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
            wallJumpingCounter = wallJumpingTime;
            isWallJumping = false;
            // Duvarın tersi yönü
            wallJumpingDirection = -transform.localScale.x;
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        // Duvar zıplaması
        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            FlipCharacterImmediately();
            Invoke(nameof(ResetWallJumping), wallJumpingDuration);
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Dash yönü
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);

        if (dashTrail) dashTrail.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        if (dashTrail) dashTrail.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void ResetWallJumping()
    {
        isWallJumping = false;
    }

    private void FlipCharacter()
    {
        // Yatay input < 0 ise sola, > 0 ise sağa bak
        if (!isWallJumping && ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f)))
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
        // Yer kontrolü
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);

        // Duvar kontrolü
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CameraTrigger"))
        {
            CameraTrigger trigger = collision.GetComponent<CameraTrigger>();
            if (trigger != null)
            {
                // Eski korutini durdur (daha kontrollü)
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
        // Kamera Lerp
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > cameraStopDistance)
        {
            Vector3 newPosition = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraSmoothSpeed * Time.deltaTime);
            newPosition.z = mainCamera.transform.position.z;
            mainCamera.transform.position = newPosition;
            yield return null;
        }
    }

    // Kod paneli aktif/pasif
    public void SetCodePanelState(bool isActive)
    {
        isCodePanelActive = isActive;
    }
}