using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Trigger'a girildiğinde kod panelini açar, oyuncu hareketini kısıtlar.
/// Aynı zamanda ESC ile paneli kapatma özelliğini de burada yönetebilirsiniz.
/// </summary>
public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject codePanel;
    [SerializeField] private GameObject codePanelTrigger;
    [SerializeField] private CodeChecker codeChecker; // CodeChecker referansı
    [SerializeField] private bool destroyTriggerOnEnter = true; 
    // true ise tetikleyiciye girildiğinde objeyi yok ediyor,
    // false ise sadece SetActive(false) yapıyor.

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            codePanel.SetActive(true);

            // Oyuncu hareketi kilitle
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
            Animator animator = playerMovement.GetComponent<Animator>();
            if (playerMovement != null)
            {
                playerMovement.SetCodePanelState(true);
            }
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                // Oyuncu animasyonunu sıfırla.
                if (animator != null)
                {
                    animator.SetFloat("Speed", 0f);
                    animator.SetBool("IsJumping", false);
                    animator.SetBool("IsFalling", false);
                    animator.SetBool("IsDashing", false);
                    animator.SetBool("IsWallSliding", false);
                    animator.SetBool("IsWallLand", false);
                    animator.SetBool("IsGrounded", true);
                } 
            }

            // Kod paneli açıldığında input alanını seç ve odakla
            if (codeChecker != null)
            {
                codeChecker.FocusInputFieldPublic();
            }

            // Tetikleyiciyi yok et veya kapat
            if (destroyTriggerOnEnter)
            {
                Destroy(codePanelTrigger);
            }
            else
            {
                codePanelTrigger.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // Panel aktifse ESC ile kapatma
        if (codePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCodePanel();
        }
    }

    private void CloseCodePanel()
    {
        codePanel.SetActive(false);

        // Panel kapanınca oyuncuya hareket izni ver
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetCodePanelState(false);
            }
        }
    }
}
