using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject codePanel;

    void Update()
    {
        if (codePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            codePanel.SetActive(false);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Oyuncu collider'a temas ettiğinde
        if (other.CompareTag("Player"))
        {
            codePanel.SetActive(true);
        }
    }
}