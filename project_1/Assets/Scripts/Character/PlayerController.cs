using UnityEngine;
using System.Collections;

/// <summary>
/// Oyuncunun temel hareket, zıplama, duvar kayma, duvar zıplama ve dash gibi işlevlerini yönetir.
/// Ek olarak, kamera tetikleyicisine girildiğinde MoveCamera korutinini başlatır.
/// Kod paneli (isCodePanelActive) açık olduğunda input devre dışı kalır.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraSmoothSpeed = 2f;
    [SerializeField] private float cameraStopDistance = 0.1f; // Hedefe yaklaşınca durma toleransı

    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;

    [Header("Dash Settings")]
    [SerializeField] private float dashingPower = 24f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer dashTrail;

    [Header("Wall Slide Settings")]
    [SerializeField] private float wallSlidingSpeed = 2f;

    [Header("Side Climb Settings")]
    [SerializeField] private float sideClimbSpeed = 2f; 

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

    private float horizontal;
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

    // Kod paneli açıkken input devre dışı bırakmak için
    private bool isCodePanelActive = false;

    // Kamera hareket korutini
    private Coroutine cameraMoveCoroutine;

    // Referans
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Kod paneli açıksa veya dash atıyorsak input kapansın
        if (isDashing || isCodePanelActive) return;

        HandleInput();
        HandleWallSlide();
        HandleWallJump();
        FlipCharacter();
        UpdateAnimatorParameters();
    }

    private void FixedUpdate()
    {
        // Duvar zıplaması veya dash anında yatay hareketi kapatıyoruz
        if (!isWallJumping && !isDashing && !isCodePanelActive)
        {
            MoveCharacter();
        }
    }

    /// <summary>
    /// Klavye inputlarını işleyerek coyote time, jump buffer, dash kontrollerini yapar.
    /// </summary>
    private void HandleInput()
    {
        // Yatay hareket
        horizontal = Input.GetAxisRaw("Horizontal");

        // Coyote time
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            canDoubleJump = true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Zıplama koşulları
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

        // Dash giriş
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    /// <summary>
    /// Karakteri yatay eksende hareket ettirir.
    /// </summary>
    private void MoveCharacter()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    /// <summary>
    /// Karakteri yukarı doğru zıplatır.
    /// </summary>
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
    }

    /// <summary>
    /// Zeminde olup olmadığımızı Physics2D.OverlapCircle ile kontrol eder.
    /// </summary>
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    /// <summary>
    /// Duvar kenarında olup olmadığımızı Physics2D.OverlapCircle ile kontrol eder.
    /// </summary>
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, checkRadius, wallLayer);
    }

    /// <summary>
    /// Duvar kayma kontrolü: yatay input varsa ve duvardaysak düşüş hızını sınırlıyoruz.
    /// </summary>
    private void HandleWallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    /// <summary>
    /// Duvara tırmanma kontrolü: yatay input varsa ve duvardaysak kullanıcının inputuna göre hareket veriyoruz.
    /// </summary>
    private void HandleSideClimb()
    {
        if (IsWalled() && !IsGrounded() && Input.GetKey(KeyCode.W))
        {
            isSideClimbing = true;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, sideClimbSpeed);
            // Flip logic? Belki duvara dönük olmayı koruyabilirsiniz
        }
        else
        {
            isSideClimbing = false;
        }
    }


    /// <summary>
    /// Duvar zıplaması: belirli bir zaman aralığında (wallJumpingTime) duvardan atlayabiliriz.
    /// </summary>
    private void HandleWallJump()
    {
        if (isWallSliding)
        {
            wallJumpingCounter = wallJumpingTime;
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x; // Duvarın tersi yönü
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            FlipCharacterImmediately();
            Invoke(nameof(ResetWallJumping), wallJumpingDuration);
        }
    }

    /// <summary>
    /// Dash korutini: Belirli bir süre gravity=0 yapıp yatay yönde hız veriyor.
    /// </summary>
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);

        if (dashTrail != null)
            dashTrail.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        if (dashTrail != null)
            dashTrail.emitting = false;

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void ResetWallJumping()
    {
        isWallJumping = false;
    }

    /// <summary>
    /// Karakter yönünü yatay input'a göre çevirir.
    /// </summary>
    private void FlipCharacter()
    {
        if (!isWallJumping && ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f)))
        {
            FlipCharacterImmediately();
        }
    }

    /// <summary>
    /// Karakteri anında sağ / sol döndürür.
    /// </summary>
    private void FlipCharacterImmediately()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        // Zemin kontrol çizimi
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);

        // Duvar kontrol çizimi
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
    }

    /// <summary>
    /// Kamera tetikleyicisine girildiğinde MoveCamera korutini başlatır.
    /// Mevcut bir korutin varsa önce durdurur.
    /// </summary>
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

    /// <summary>
    /// Kamera konumunu Lerp ile hedefe yaklaştırır.
    /// </summary>
    private IEnumerator MoveCamera(Vector3 targetPosition)
    {
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > cameraStopDistance)
        {
            Vector3 newPosition = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraSmoothSpeed * Time.deltaTime);
            newPosition.z = mainCamera.transform.position.z;
            mainCamera.transform.position = newPosition;
            yield return null;
        }
    }

    /// <summary>
    /// Kod paneli açıldığında hareketi kapatmak için kullanılır.
    /// </summary>
    /// <param name="isActive">true=hareket kapalı, false=hareket serbest</param>
    public void SetCodePanelState(bool isActive)
    {
        isCodePanelActive = isActive;
    }

    /// <summary>
    /// Animasyon güncellemelerini sağlamak için kullanılır.
    /// </summary>
    private void UpdateAnimatorParameters()
    {
        // 1) Speed parametresi (X eksenindeki hız veya x input)
        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat("Speed", horizontalSpeed);

        // 2) Zıplama & Düşme
        bool isGrounded = IsGrounded(); // Varsa bu fonksiyon
        bool isJumping = !isGrounded && rb.linearVelocity.y > 0.1f;
        bool isFalling = !isGrounded && rb.linearVelocity.y < -0.1f;

        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
        bool isLanding = (isFalling == false && isGrounded == true && rb.linearVelocity.y < 0f);
        animator.SetBool("IsLanding", isLanding);

        animator.SetBool("IsWallSliding", isWallSliding);
        animator.SetBool("IsSideClimbing", isSideClimbing);
        // **Wall Land** (isteğe bağlı; 
        // eğer “duvardan inince” küçük bir anim oynatacaksanız)
        // bool justLandedFromWall = isWallSliding == false && walled == false && grounded == true ...
        // animator.SetBool("IsWallLanding", justLandedFromWall);

        // 3) Dash parametresi
        animator.SetBool("IsDashing", isDashing);

        // Opsiyonel: isGrounded parametresi de ekleyebilirsiniz
        animator.SetBool("IsGrounded", isGrounded);
    }
}