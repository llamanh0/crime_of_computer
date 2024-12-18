using System;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
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
            spriteRenderer.flipX = false; // Sağa bak
        }
        else if (movement.x < 0)
        {
            spriteRenderer.flipX = true; // Sola bak
        }

        float speedValue = Mathf.Abs(movement.x) > 0.1f ? 1 : 0;
        animator.SetFloat("Speed", speedValue);
        Debug.Log(speedValue);
    }

    void FixedUpdate()
    {
        // Karakteri hareket ettir
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }
}
