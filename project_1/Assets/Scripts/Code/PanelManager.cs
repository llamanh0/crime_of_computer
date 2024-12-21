using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject codePanel;
    [SerializeField] private GameObject codePanelTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            codePanel.SetActive(true);
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.SetCodePanelState(true);
            }
            codePanelTrigger.SetActive(false);
        }
    }

    /*
    private void Update()
    {
        if (codePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            codePanel.SetActive(false);
            // Kod ekranı kapandığında durumu geri ayarla
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
    */
}
