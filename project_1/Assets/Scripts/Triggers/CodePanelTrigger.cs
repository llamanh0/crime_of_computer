// Assets/Scripts/UI/CodePanelTrigger.cs
using UnityEngine;
using MyGame.Managers;

namespace MyGame.UI
{
    /// <summary>
    /// Oyuncu belirli bir alana geldiğinde kod panelini açar ve karakteri idle animasyonuna geçirir.
    /// </summary>
    public class CodePanelTrigger : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject codePanel; // Açılacak kod paneli

        [Header("Player References")]
        [SerializeField] private Animator playerAnimator; // Oyuncu animasyon kontrolcüsü

        private bool isPlayerNearby = false;

        private void Update()
        {
            if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
            {
                OpenCodePanel();
            }
        }

        private void OpenCodePanel()
        {
            if (codePanel != null)
            {
                codePanel.SetActive(true);
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("isIdle", true); // Idle animasyonuna geç
                }
                // Oyunu duraklatma veya diğer gerekli işlemler
                Time.timeScale = 0f; // Oyun zamanını duraklatır
            }
            else
            {
                Debug.LogError("CodePanelTrigger: Code Panel referansı eksik!");
            }
        }

        /// <summary>
        /// CodePanel'ı kapatır ve oyunu devam ettirir.
        /// </summary>
        public void CloseCodePanel()
        {
            if (codePanel != null)
            {
                codePanel.SetActive(false);
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("isIdle", false); // Idle animasyonundan çıkar
                }
                // Oyunu devam ettirme
                Time.timeScale = 1f; // Oyun zamanını devam ettirir
            }
            else
            {
                Debug.LogError("CodePanelTrigger: Code Panel referansı eksik!");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isPlayerNearby = true;
                // Kullanıcıya etkileşim yapabileceğini göstermek için UI gösterebilirsiniz
                // Örneğin, "Press E to interact" mesajı
                // Öneri: bir UI elemanı gösterin
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isPlayerNearby = false;
                // Etkileşim UI'sini gizleyebilirsiniz
                // Öneri: gösterdiğiniz UI elemanını gizleyin
            }
        }
    }
}
