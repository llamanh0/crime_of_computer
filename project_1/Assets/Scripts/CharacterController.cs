using System;
using System.Collections;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Değişkenler
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float cameraSmoothSpeed = 2f;
    public float dashDistance = 5f; // Dash sırasında kat edilecek mesafe
    public float dashDuration = 0.2f; // Dash süresi
    public float dashCooldown = 1f; // Dash tekrar kullanılabilir olana kadar bekleme süresi


    // Obje ve Referanslar
    private Rigidbody2D rb;
    public Camera mainCamera;
    private Vector2 movement;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Bools
    private bool isGrounded;
    private bool isLanding;
    private bool isDashing = false;
    private bool canDash = true;


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
            spriteRenderer.flipX = false; // Sağ
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true; // Sol
        }

        float speedValue = Mathf.Abs(movement.x) > 0.1f ? 1 : 0;
        animator.SetFloat("Speed", speedValue);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("IsJumping", true);
            isGrounded = false;
        }
         // Normal hareket (Dash yapılmadığında)
        if (!isDashing)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);

            // Animasyon parametrelerini güncelle
            animator.SetFloat("Speed", Mathf.Abs(moveX));

            // Dash kontrolü
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contactCount > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            if (!isGrounded)
            {
                isLanding = true;
                animator.SetBool("IsLanding", true);
                Invoke("FinishLanding", 0.1f); // ...,(animasyon süresi)
            }

            isGrounded = true;
            animator.SetBool("IsJumping", false);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Zeminle temas kesilince karakter havada
        if (collision.contactCount > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = false;
        }
    }
    private void FinishLanding()
    {
        isLanding = false;
        animator.SetBool("IsLanding", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer etkileşimde bulunulan obje bir CameraTrigger ise
        if (collision.gameObject.CompareTag("CameraTrigger"))
        {
            // Trigger'daki hedef pozisyonu al
            CameraTrigger trigger = collision.GetComponent<CameraTrigger>();
            if (trigger != null)
            {
                Vector3 targetPosition = trigger.targetPosition;

                // Kamerayı bu hedef pozisyona hareket ettir
                StopAllCoroutines(); // Önceki hareketleri durdur
                StartCoroutine(MoveCamera(targetPosition));
            }
        }
    }
    private System.Collections.IEnumerator MoveCamera(Vector3 targetPosition)
    {
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.1f)
        {
            // Kameranın pozisyonunu hedefe doğru yumuşak bir şekilde hareket ettir
            Vector3 newPosition = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraSmoothSpeed * Time.deltaTime);
            newPosition.z = mainCamera.transform.position.z; // Z ekseni sabit kalmalı
            mainCamera.transform.position = newPosition;

            yield return null; // Bir sonraki frame'i bekle
        }
    }
    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        // Dash yönünü belirle
        float dashDirection = spriteRenderer.flipX ? -1 : 1;

        // Başlangıç pozisyonunu al
        Vector2 startPosition = rb.position;

        // Hedef pozisyonu belirle
        Vector2 targetPosition = startPosition + new Vector2(dashDirection * dashDistance, 0);

        // Hareketi zamanla gerçekleştirme
        animator.SetTrigger("Dash");
        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            rb.MovePosition(Vector2.Lerp(startPosition, targetPosition, elapsedTime / dashDuration));
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Hedef pozisyona tam olarak yerleştir
        rb.MovePosition(targetPosition);
        animator.ResetTrigger("Dash");

        isDashing = false;

        // Dash cooldown süresi
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
