// Assets/Scripts/UI/CodePanelTrigger.cs
using UnityEngine;
using MyGame.Player; // PlayerMovement sınıfının bulunduğu namespace
using MyGame.Puzzles; // PuzzleData için gerekli olabilir

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

        [Header("Puzzle References")]
        [SerializeField] private PuzzleData currentPuzzle; // Şu anki bulmaca verisi (kullanılacaksa)

        private bool isPlayerNearby = false;
        private PlayerMovement playerMovement;

        private void Start()
        {
            // Sahnedeki PlayerMovement script'ini bulur
            var foundPlayers = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);
            if (foundPlayers.Length > 0)
            {
                playerMovement = foundPlayers[0];
            }
            else
            {
                Debug.LogError("CodePanelTrigger: PlayerMovement script'i sahnede bulunamadı.");
            }
        }

        private void Update()
        {
            // Oyuncu yakındaysa ve E tuşuna basıldıysa paneli aç
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

                if (playerMovement != null)
                {
                    playerMovement.SetCodePanelState(true);
                }

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

                if (playerMovement != null)
                {
                    playerMovement.SetCodePanelState(false);
                }

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
                // "Press E to interact" UI öğesi gösterebilirsiniz
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isPlayerNearby = false;
                // Etkileşim UI'sini gizleyebilirsiniz
            }
        }
    }
}
