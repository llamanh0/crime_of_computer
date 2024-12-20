using System;
using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Karakter Hareket Değişkenleri
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float wallSlideSpeed = 2f;
    public float climbSpeed = 3f;
    private float horizontal;

    // Referanslar
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform groundCheck;

    // Katmanlar
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public LayerMask wallLayer;

    // Durumlar
    private bool isGrounded;
    private bool isJumping;
    private bool isFalling;
    private bool isWallSliding;
    private bool isWallJumping;
    private bool isDashing;
    private bool canDash = true;
    private bool isClimbing;
    private bool isFacingRight;

    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleDash();
    }

    private void HandleMovement()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        if (!isDashing && !isClimbing)
        {
            rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        }

        if (horizontal != 0)
        {
            spriteRenderer.flipX = horizontal < 0;
        }

        animator.SetBool("IsRunning", horizontal != 0 && isGrounded);
    }

    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 1f, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("IsJumping", true);
            isJumping = true;
        }

        if (rb.linearVelocity.y < 0 && !isGrounded)
        {
            animator.SetBool("IsFalling", true);
            isFalling = true;
        }
        else if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            isFalling = false;
        }
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isClimbing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        float dashDirection = spriteRenderer.flipX ? -1 : 1;

        Vector2 startPosition = rb.position;
        Vector2 targetPosition = startPosition + new Vector2(dashDirection * dashDistance, 0);

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, elapsedTime / dashDuration));
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contactCount > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            animator.SetTrigger("Land");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Climbable"))
        {
            isClimbing = true;
            animator.SetBool("IsClimbing", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Climbable"))
        {
            isClimbing = false;
            animator.SetBool("IsClimbing", false);
        }
    }
}
