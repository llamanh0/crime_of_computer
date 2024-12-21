using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject codePanel;
    [SerializeField] private GameObject codePanelTrigger;
    [SerializeField] private bool destroyTriggerOnEnter = true;
    // true ise tetikleyiciye girildiğinde yok edilecek,
    // false ise SetActive(false) yapılacak.

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            codePanel.SetActive(true);

            // Oyuncu movement kilitleniyor
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetCodePanelState(true);
            }

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

    // ESC ile paneli kapatma özelliği
    private void Update()
    {
        if (codePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCodePanel();
        }
    }

    private void CloseCodePanel()
    {
        codePanel.SetActive(false);

        // Panel kapanınca player hareketini geri ver
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