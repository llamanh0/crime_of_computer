using System;
using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Değişkenler
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float cameraSmoothSpeed = 2f;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float climbSpeed = 3f;
    public float wallSlideSpeed = 2f;

    // Obje ve Referanslar
    private Rigidbody2D rb;
    public Camera mainCamera;
    private Vector2 movement;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Bools
    private bool isGrounded;
    private bool isFalling;
    private bool isLanding;
    private bool isWallLanding;
    private bool isDashing = false;
    private bool canDash = true;
    private bool isClimbing = false;
    private bool isWallSliding = false;

    // Tırmanma ve Duvar Kontrolü
    private Collider2D climbableSurface;
    public LayerMask wallLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        if (movement.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true;
        }

        animator.SetBool("IsIdle", movement.x == 0 && isGrounded);
        animator.SetBool("IsRunning", Mathf.Abs(movement.x) > 0.1f && isGrounded);

        // Düşme Animasyonu Kontrolü
        if (rb.linearVelocity.y < 0 && !isGrounded && !isWallSliding)
        {
            isFalling = true;
            animator.SetBool("IsFalling", true);
            animator.SetBool("IsJumping", false); // Jump animasyonu kapat
        }
        else
        {
            isFalling = false;
            animator.SetBool("IsFalling", false);
        }

        // Zıplama Kontrolü
        if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            PerformWallJump();
        }
        else if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false); // Düşme animasyonu kapat
            isGrounded = false;
        }

        // Dash Kontrolü
        if (!isDashing && !isClimbing && !isWallSliding)
        {
            rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(Dash());
            }
        }

        // Tırmanma Kontrolü
        if (isClimbing)
        {
            float verticalMovement = Input.GetAxisRaw("Vertical");
            rb.linearVelocity = new Vector2(0, verticalMovement * climbSpeed);
            animator.SetBool("IsClimbing", verticalMovement != 0);
        }

        CheckWallSlide();
    }

    private void FixedUpdate()
    {
        if (!isDashing && !isClimbing && !isWallSliding)
        {
            rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contactCount > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            if (!isGrounded)
            {
                isLanding = true;
                animator.SetTrigger("IsLanding");
                animator.SetBool("IsFalling", false); // Düşme animasyonunu kapat
            }

            isGrounded = true;
            animator.SetBool("IsJumping", false);
            animator.SetBool("Dash", false); // Dash animasyonunu kapat
        }
        else if (collision.contactCount > 0 && collision.contacts[0].normal.x != 0 && (wallLayer.value & 1 << collision.gameObject.layer) != 0)
        {
            isWallLanding = true;
            animator.SetTrigger("IsWallLanding");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Climbable"))
        {
            isClimbing = true;
            rb.gravityScale = 0;
            climbableSurface = collision;
            animator.SetBool("IsClimbing", true);
        }
        if (collision.gameObject.CompareTag("CameraTrigger"))
        {
            CameraTrigger trigger = collision.GetComponent<CameraTrigger>();
            if (trigger != null)
            {
                Vector3 targetPosition = trigger.targetPosition;
                StopAllCoroutines();
                StartCoroutine(MoveCamera(targetPosition));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == climbableSurface)
        {
            isClimbing = false;
            rb.gravityScale = 1;
            climbableSurface = null;
            animator.SetBool("IsClimbing", false);
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float dashDirection = spriteRenderer.flipX ? -1 : 1;
        Vector2 startPosition = rb.position;
        Vector2 targetPosition = startPosition + new Vector2(dashDirection * dashDistance, 0);

        animator.SetTrigger("Dash");
        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, elapsedTime / dashDuration));
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPosition);
        animator.ResetTrigger("Dash");

        isDashing = false;
        animator.SetBool("Dash", false); // Dash animasyonunu kapat

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private System.Collections.IEnumerator MoveCamera(Vector3 targetPosition)
    {
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.1f)
        {
            Vector3 newPosition = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraSmoothSpeed * Time.deltaTime);
            newPosition.z = mainCamera.transform.position.z;
            mainCamera.transform.position = newPosition;
            yield return null;
        }
    }

    private void PerformWallJump()
    {
        bool isTouchingWall = Physics2D.OverlapCircle(transform.position, 0.1f, wallLayer);
        if (isTouchingWall && isWallSliding)
        {
            float jumpDirection = spriteRenderer.flipX ? 1 : -1; // Ters yöne zıplamak için yönü belirle
            rb.linearVelocity = new Vector2(jumpDirection * moveSpeed, jumpForce);
            rb.gravityScale = 1; // Yerçekimi ayarını doğru yap

            // Parametreleri güncelle
            isWallSliding = false;
            animator.SetBool("IsWallSliding", false);
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false); // Düşme animasyonu kapat
        }
    }

    private void CheckWallSlide()
    {
        bool isTouchingWall = Physics2D.OverlapCircle(transform.position, 0.1f, wallLayer);
        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(0, -wallSlideSpeed);
            animator.SetBool("IsWallSliding", true);
        }
        else
        {
            isWallSliding = false;
            animator.SetBool("IsWallSliding", false);
        }
    }
}
